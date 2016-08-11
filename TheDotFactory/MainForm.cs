/*
 * Copyright 2009, Eran "Pavius" Duchan
 * This file is part of The Dot Factory.
 *
 * The Dot Factory is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License as published by the Free 
 * Software Foundation, either version 3 of the License, or (at your option) 
 * any later version. The Dot Factory is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more 
 * details. You should have received a copy of the GNU General Public License along 
 * with The Dot Factory. If not, see http://www.gnu.org/licenses/.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace TheDotFactory
{
    public partial class MainForm : Form
    {
        #region formatting strings
        public const string HeaderStringColumnStart = "\t0b";
        public const string HeaderStringColumnMid = ", 0b";
        public const string BitString1 = "1";
        public const string BitString0 = "0";
        private static String nl = Environment.NewLine;
        #endregion

        static bool RunInParallel = true;

        // current loaded bitmap
        private Bitmap m_currentLoadedBitmap = null;

        // output configuration
        public OutputConfigurationManager m_outputConfigurationManager = new OutputConfigurationManager();

        // output configuration
        private OutputConfiguration m_outputConfig;

        Dictionary<Color, bool> colorList = new Dictionary<Color, bool>();

        #region classes
        // info per font
        public class FontInfo
        {
            private OutputConfiguration outConfig;

            public int charHeight { get; private set; }
            public char startChar { get; private set; }
            public char endChar { get; private set; }
            public CharacterGenerationInfo[] characters { get; private set; }
            public Font font { get; private set; }
            public int codePage { get { return CodePage.CodePage; } }
            public CodePageInfo CodePage { get; private set; }

            public char[] Chars { get; private set; }

            public static FontInfo GenerateFromFont(Font font, OutputConfiguration config, char[] Characters)
            {
                FontInfo fontInfo = new FontInfo();
                Dictionary<Color, bool> colorList = new Dictionary<Color, bool>(2);

                colorList.Add(Color.FromArgb(255, Color.White), true);
                colorList.Add(Color.FromArgb(255, Color.Black), false);

                fontInfo.font = font;
                fontInfo.outConfig = config;
                fontInfo.Chars = Characters;
                fontInfo.CodePage = new CodePageInfo(config.CodePage);

                //
                // init char infos
                //
                fontInfo.characters = new CharacterGenerationInfo[fontInfo.Chars.Length];

                for (int charIdx = 0; charIdx < fontInfo.Chars.Length; ++charIdx)
                {
                    // create char info entity
                    fontInfo.characters[charIdx] = new CharacterGenerationInfo();

                    // point back to the font
                    fontInfo.characters[charIdx].ParentFontInfo = fontInfo;

                    // set the character
                    fontInfo.characters[charIdx].character = fontInfo.Chars[charIdx];
                }

                //
                // Find the widest bitmap size we are going to draw
                //
                Rectangle largestBitmap = getLargestBitmapFromCharInfo(fontInfo.characters);

                //
                // create bitmaps per characater
                //

                // iterate over characters
                for (int charIdx = 0; charIdx < fontInfo.Chars.Length; ++charIdx)
                {
                    // generate the original bitmap for the character
                    fontInfo.characters[charIdx].bitmapOriginal = CharacterGenerationInfo.ConvertCharacterToBitmap(fontInfo.Chars[charIdx],
                                             font, largestBitmap.Size);

                    // save
                    //fontInfo.characters[charIdx].bitmapOriginal.Save(String.Format("C:/bms/{0}.bmp", fontInfo.characters[charIdx].character));
                }

                //
                // iterate through all bitmaps and find the tightest common border. only perform
                // this if the configuration specifies
                //

                // this will contain the values of the tightest border around the characters
                Border tightestCommonBorder = Border.Default;

                // only perform if padding type specifies
                if (fontInfo.outConfig.paddingRemovalHorizontal == OutputConfiguration.PaddingRemoval.Fixed ||
                    fontInfo.outConfig.paddingRemovalVertical == OutputConfiguration.PaddingRemoval.Fixed)
                {
                    // find the common tightest border
                    findTightestCommonBitmapBorder(fontInfo.characters, ref tightestCommonBorder, false, colorList);
                }

                //
                // iterate thruogh all bitmaps and generate the bitmap we will convert to string
                // this means performing all manipulation (pad remove, flip)
                //

                // iterate over characters
                for (int charIdx = 0; charIdx < fontInfo.Chars.Length; ++charIdx)
                {
                    // generate the original bitmap for the character
                    manipulateBitmap(fontInfo.characters[charIdx].bitmapOriginal,
                                     tightestCommonBorder,
                                     out fontInfo.characters[charIdx].bitmapToGenerate,
                                     fontInfo.outConfig.spaceGenerationPixels,
                                     fontInfo.characters[charIdx].bitmapOriginal.Height,
                                     false,
                                     colorList,
                                     fontInfo.outConfig);

                    // for debugging
                    //fontInfo.characters[charIdx].bitmapToGenerate.Save(string.Format(@"C:\bms\{0}_cropped.bmp", fontInfo.characters[charIdx].character));
                }

                //
                // iterate through all characters and create the page array
                //

                // iterate over characters
                for (int charIdx = 0; charIdx < fontInfo.Chars.Length; ++charIdx)
                {
                    // check if bitmap exists
                    if (fontInfo.characters[charIdx].bitmapToGenerate != null)
                    {
                        // create the page array for the character
                        fontInfo.characters[charIdx].pages = CharacterGenerationInfo.ConvertBitmapToPageArray(fontInfo.characters[charIdx].bitmapToGenerate, 
                            fontInfo.outConfig.bitLayout == OutputConfiguration.BitLayout.ColumnMajor, 
                            fontInfo.outConfig.byteOrder == OutputConfiguration.ByteOrder.MsbFirst);
                    }
                }

                // populate font info
                fontInfo.populateFontInfoFromCharacters();

                return fontInfo;
            }

            // generate the strings
            public void GenerateStringsFromFontInfo(ref string resultTextSource, ref string resultTextHeader)
            {
                //
                // Character bitmaps
                //
                ConcurrentDictionary<int, string> resultTextSourceP = new ConcurrentDictionary<int, string>();

                CodePageInfo cpi = new CodePageInfo(outConfig.CodePage);

                Func<int, string> func = delegate (int charindex)
                {
                    string s = "";
                    // skip empty bitmaps
                    if (characters[charindex].bitmapToGenerate == null) return "";

                    // according to config
                    if (outConfig.commentCharDescriptor)
                    {
                        // output character header
                        s += String.Format("\t{0}@{1} '{2}' ({3} pixels wide){4}" + nl,
                                                            outConfig.CommentStart,
                                                            characters[charindex].offsetInBytes,
                                                            characters[charindex].character,
                                                            characters[charindex].width,
                                                            outConfig.CommentEnd);
                    }

                    // now add letter array
                    CharacterGenerationInfo charInfo = characters[charindex];
                    Bitmap bitmap = characters[charindex].bitmapToGenerate;
                    s += generateStringFromPageArray(bitmap.Width, bitmap.Height, charInfo.pages, outConfig);

                    // space out
                    if (charindex != characters.Length - 1 && outConfig.commentCharDescriptor)
                    {
                        // space between chars
                        s += nl;
                    }
                    return s;
                };

                Action<int> funcP = delegate (int charindex)
                {
                    resultTextSourceP.GetOrAdd(charindex, func(charindex));
                };

                // according to config
                if (outConfig.commentVariableName)
                {
                    // add source header
                    resultTextSource += String.Format("{0}Character bitmaps for {1} {2}pt{3}" + nl,
                                                        outConfig.CommentStart, font.Name,
                                                        Math.Round(font.Size), outConfig.CommentEnd);
                }

                // get bitmap name
                string charBitmapVarName = String.Format(outConfig.varNfBitmaps, getFontName(font)) + "[]";

                // header var
                //resultTextHeader += String.Format("extern {0};" + nl, charBitmapVarName);

                // source var
                resultTextSource += String.Format("{0} = " + nl + "{{" + nl, charBitmapVarName);

                // iterate through letters

                if (RunInParallel)
                {
                    Parallel.For(0, characters.Length, funcP);

                    resultTextSource += resultTextSourceP.AsParallel().OrderBy(p => p.Key).Select(p => p.Value).Aggregate((current, next) => current + next); // .AsSequential().Select(p => p.Value).ToArray().Aggregate((current, next) => current + next);
                    
                    //for (int i = 0; i < characters.Length; i++)
                    //    resultTextSource += resultTextSourceP[i];
                }
                else
                {
                    for (int charIdx = 0; charIdx < characters.Length; ++charIdx)
                    {
                        resultTextSource += func(charIdx);
                    }
                }

                // space out
                resultTextSource += "};" + nl + nl;

                //
                // Charater descriptor
                //

                // whether or not block lookup was generated
                bool blockLookupGenerated = false;

                // generate the lookup array
                generateCharacterDescriptorArray(ref resultTextSource, ref resultTextHeader, ref blockLookupGenerated);

                //
                // Font descriptor
                //

                // according to config
                if (outConfig.commentVariableName)
                {
                    // result string
                    resultTextSource += String.Format("{0}Font information for {1} {2}pt{3}" + nl,
                                                        outConfig.CommentStart,
                                                        font.Name, Math.Round(font.Size),
                                                        outConfig.CommentEnd);
                }

                // character name
                string fontInfoVarName = String.Format(outConfig.varNfFontInfo, getFontName(font));

                // add character array for header
                resultTextHeader += String.Format("extern {0};" + nl, fontInfoVarName);

                // the font character height
                string fontCharHeightString = "", spaceCharacterPixelWidthString = "";

                // get character height sstring - displayed according to output configuration
                if (outConfig.descFontHeight != OutputConfiguration.DescriptorFormat.DontDisplay)
                {
                    // convert the value
                    fontCharHeightString = String.Format("\t{0}, {1} Character height{2}" + nl,
                                                  convertValueByDescriptorFormat(outConfig.descFontHeight, charHeight),
                                                  outConfig.CommentStart,
                                                  outConfig.CommentEnd);
                }

                string fontCodePage = "";
                if (outConfig.addCodePage)
                {
                    fontCodePage = string.Format("\t{0}, {1} CodePage {3}{2}" + nl,
                        outConfig.CodePage,
                        outConfig.CommentStart,
                        outConfig.CommentEnd,
                        CodePageInfo.GetCodepageName(outConfig.CodePage));
                }

                // get space char width, if it is up to driver to generate
                if (!outConfig.generateSpaceCharacterBitmap)
                {
                    // convert the value
                    spaceCharacterPixelWidthString = String.Format("\t{0}, {1} Width, in pixels, of space character{2}" + nl,
                                                                    outConfig.spaceGenerationPixels,
                                                                    outConfig.CommentStart,
                                                                    outConfig.CommentEnd);
                }

                // font info
                resultTextSource += String.Format("{2} =" + nl + "{{" + nl +
                                                  "{3}" +
                                                  "\t{4}, {0} Start character '{9}'{1}" + nl +
                                                  "\t{5}, {0} End character '{10}'{1}" + nl +
                                                  "{6}" +
                                                  "{7}" +
                                                  "\t{8}, {0} Character bitmap array{1}" + nl +
                                                  "{11}" +
                                                  "}};" + nl,
                                                   outConfig.CommentStart,
                                                  outConfig.CommentEnd,
                                                  fontInfoVarName,
                                                  fontCharHeightString,
                                                  getCharacterDisplayString(cpi, startChar),
                                                  getCharacterDisplayString(cpi, endChar),
                                                  spaceCharacterPixelWidthString,
                                                  getFontInfoDescriptorsString(blockLookupGenerated, outConfig, font),
                                                  getVariableNameFromExpression(String.Format(outConfig.varNfBitmaps, getFontName(font))),
                                                  startChar,
                                                  endChar,
                                                  fontCodePage);

                // add the appropriate entity to the header
                if (blockLookupGenerated)
                {
                    // add block lookup to header
                    resultTextHeader += String.Format("extern const FONT_CHAR_INFO_LOOKUP {0}[];" + nl, getCharacterDescriptorArrayLookupDisplayString(font));
                }
                else
                {
                    // add block lookup to header
                    resultTextHeader += String.Format("extern {0}[];" + nl, String.Format(outConfig.varNfCharInfo, getFontName(font)));
                }
            }

            // get font info from string
            private void populateFontInfoFromCharacters()
            {
                // do nothing if no chars defined
                if (characters.Length == 0) return;

                // total offset
                int charByteOffset = 0;

                CodePageInfo cpi = new CodePageInfo(outConfig.CodePage);

                // set start char
                startChar = CodePageInfo.GetLastValidCharacter(outConfig.CodePage);
                endChar = CodePageInfo.GetFirstValidCharacter(outConfig.CodePage);

                // the fixed absolute character height
                // int fixedAbsoluteCharHeight;
                this.charHeight = getAbsoluteCharacterDimensions(characters[0].bitmapToGenerate).Height;

                // iterate through letter string
                for (int charIdx = 0; charIdx < characters.Length; ++charIdx)
                {
                    // skip empty bitmaps
                    if (characters[charIdx].bitmapToGenerate == null) continue;

                    // get char
                    char currentChar = characters[charIdx].character;

                    // is this character smaller than start char?
                    if (cpi.GetCharacterDifferance(currentChar, startChar) > 0) startChar = currentChar;

                    // is this character bigger than end char?
                    if (cpi.GetCharacterDifferance(currentChar, endChar) < 0) endChar = currentChar;

                    // populate number of rows
                    characters[charIdx].Size = getAbsoluteCharacterDimensions(characters[charIdx].bitmapToGenerate);

                    // populate offset of character
                    characters[charIdx].offsetInBytes = charByteOffset;

                    // increment byte offset
                    charByteOffset += characters[charIdx].pages.Length;
                }
            }

            // get absolute height/width of characters
            private Size getAbsoluteCharacterDimensions(Bitmap charBitmap)
            {
                // check if bitmap exists, otherwise set as zero
                if (charBitmap == null)
                {
                    return new Size();
                }
                else
                {
                    // get the absolute font character height. Depends on rotation
                    if (outConfig.rotation == OutputConfiguration.Rotation.RotateZero ||
                        outConfig.rotation == OutputConfiguration.Rotation.RotateOneEighty)
                    {
                        // if char is not rotated or rotated 180deg, its height is the actual height
                        return charBitmap.Size;
                    }
                    else
                    {
                        // if char is rotated by 90 or 270, its height is the width of the rotated bitmap
                        return new Size(charBitmap.Height, charBitmap.Width);
                    }
                }
            }

            // get the descriptors
            private static string getFontInfoDescriptorsString(bool blockLookupGenerated, OutputConfiguration outConfig, Font font)
            {
                string descriptorString = "";

                // if a lookup arrays are required, point to it
                if (outConfig.generateLookupBlocks)
                {
                    // add to string
                    descriptorString += String.Format("\t{0}, {1} Character block lookup{2}" + nl,
                                                        blockLookupGenerated ? getCharacterDescriptorArrayLookupDisplayString(font) : "NULL",
                                                        outConfig.CommentStart, outConfig.CommentEnd);

                    // add to string
                    descriptorString += String.Format("\t{0}, {1} Character descriptor array{2}" + nl,
                                                        blockLookupGenerated ? "NULL" : getVariableNameFromExpression(String.Format(outConfig.varNfCharInfo, getFontName(font))),
                                                        outConfig.CommentStart, outConfig.CommentEnd);
                }
                else
                {
                    // add descriptor array
                    descriptorString += String.Format("\t{0}, {1} Character descriptor array{2}" + nl,
                                                        getVariableNameFromExpression(String.Format(outConfig.varNfCharInfo, getFontName(font))),
                                                        outConfig.CommentStart, outConfig.CommentEnd);
                }

                // return the string
                return descriptorString;
            }

           
            #region Lookup
            // genereate a list of blocks describing the characters
            private CharacterDescriptorArrayBlock[] generateCharacterDescriptorBlockList()
            {
                char currentCharacter, previousCharacter = '\0';
                List<CharacterDescriptorArrayBlock> characterBlockList = new List<CharacterDescriptorArrayBlock>();
                // initialize first block
                CharacterDescriptorArrayBlock characterBlock = null;

                CodePageInfo cpi = new CodePageInfo(outConfig.CodePage);

                // get the difference between two characters required to create a new group
                int differenceBetweenCharsForNewGroup = outConfig.generateLookupBlocks ?
                        outConfig.lookupBlocksNewAfterCharCount : int.MaxValue;

                // iterate over characters, saving previous character each time
                for (int charIndex = 0; charIndex < characters.Length; ++charIndex)
                {
                    // get character
                    currentCharacter = characters[charIndex].character;

                    // check if this character is too far from the previous character and it isn't the first char
                    if (cpi.GetCharacterDifferance(previousCharacter, currentCharacter) < differenceBetweenCharsForNewGroup && previousCharacter != '\0')
                    {
                        // it may not be far enough to generate a new group but it still may be non-sequential
                        // in this case we need to generate place holders
                        int previousCharacterOffset = cpi.GetOffsetFromCharacter(previousCharacter);
                        int currentCharacterOffset = cpi.GetOffsetFromCharacter(currentCharacter);

                        for (int sequentialCharIndex = previousCharacterOffset + 1;
                                sequentialCharIndex < currentCharacterOffset;
                                ++sequentialCharIndex)
                        {
                            // add the character placeholder to the current char block
                            characterBlock.charDescArrayAddCharacter(
                                this,
                                cpi.GetCharacterFromOffset(sequentialCharIndex),
                                0, 0, 0);
                        }

                        // fall through and add to current block
                    }
                    else
                    {
                        // done with current block, add to list (null is for first character which hasn't
                        // created a group yet)
                        if (characterBlock != null) characterBlockList.Add(characterBlock);

                        // create new block
                        characterBlock = new CharacterDescriptorArrayBlock();
                    }

                    // add to current block
                    characterBlock.charDescArrayAddCharacter(this, currentCharacter,
                                              characters[charIndex].width,
                                              characters[charIndex].height,
                                              characters[charIndex].offsetInBytes);

                    // save previous char
                    previousCharacter = currentCharacter;
                }

                // done; add current block to list
                characterBlockList.Add(characterBlock);

                return characterBlockList.ToArray();
            }

            // generate lookup array
            private void generateCharacterDescriptorArray(ref string resultTextSource,
                                                            ref string resultTextHeader,
                                                            ref bool blockLookupGenerated )
            {
                // check if required by configuration
                if (outConfig.generateLookupArray)
                {
                    CharacterDescriptorArrayBlock[] characterBlockList;

                    // populate list of blocks
                    characterBlockList = generateCharacterDescriptorBlockList();

                    // generate strings from block list
                    generateStringsFromCharacterDescriptorBlockList(characterBlockList, ref resultTextSource,
                                                                    ref resultTextHeader, ref blockLookupGenerated);
                }
            }

            // generate source/header strings from a block list
            private void generateStringsFromCharacterDescriptorBlockList(CharacterDescriptorArrayBlock[] characterBlockList,
                                                                         ref string resultTextSource,
                                                                         ref string resultTextHeader,
                                                                         ref bool blockLookupGenerated)
            {
                // get wheter there are multiple block lsits
                bool multipleDescBlocksExist = characterBlockList.Length > 1;

                CodePageInfo cpi = new CodePageInfo(outConfig.CodePage);

                // set whether we'll generate lookups
                blockLookupGenerated = multipleDescBlocksExist;

                //
                // Generate descriptor arrays
                //

                // iterate over blocks
                foreach (CharacterDescriptorArrayBlock block in characterBlockList)
                {
                    // according to config
                    if (outConfig.commentVariableName)
                    {
                        string blockNumberString = String.Format("(block #{0})", Array.IndexOf(characterBlockList, block));

                        // result string
                        resultTextSource += String.Format("{0}Character descriptors for {1} {2}pt{3}{4}" + nl,
                                                            outConfig.CommentStart, font.Name,
                                                            Math.Round(font.Size), multipleDescBlocksExist ? blockNumberString : "",
                                                            outConfig.CommentEnd);

                        // describe character array
                        resultTextSource += String.Format("{0}{{ {1}{2}[Offset into {3}CharBitmaps in bytes] }}{4}" + nl,
                                                            outConfig.CommentStart,
                                                            getCharacterDescName("width", outConfig.descCharWidth),
                                                            getCharacterDescName("height", outConfig.descCharHeight),
                                                            getFontName(font),
                                                            outConfig.CommentEnd);
                    }

                    // output block header
                    resultTextSource += String.Format("{0} = " + nl + "{{" + nl, charDescArrayGetBlockName(Array.IndexOf(characterBlockList, block), true, multipleDescBlocksExist));

                    // iterate characters
                    //foreach (CharacterDescriptorArrayBlock.Character character in block.characters)
                    resultTextSource += block.characters.AsParallel().AsOrdered().Select( character =>
                    {
                        // add character
                         return String.Format("\t{{{0}{1}{2}}}, \t\t{3}{4}{5}" + nl,
                                                        getCharacterDescString(outConfig.descCharWidth, character.width),
                                                        getCharacterDescString(outConfig.descCharHeight, character.height),
                                                        character.offset,
                                                        outConfig.CommentStart,
                                                        character.character == '\\' ? "\\ (backslash)" : new string(character.character, 1),
                                                        outConfig.CommentEnd + " ");
                    }).Aggregate((current, next) => current + next);

                    // terminate current block
                    resultTextSource += "};" + nl + nl;
                }

                //
                // Generate block lookup 
                //

                // if there is more than one block, we need to generate a block lookup
                if (multipleDescBlocksExist)
                {
                    // start with comment, if required
                    if (outConfig.commentVariableName)
                    {
                        // result string
                        resultTextSource += String.Format("{0}Block lookup array for {1} {2}pt {3}" + nl,
                                                            outConfig.CommentStart, font.Name,
                                                            Math.Round(font.Size), outConfig.CommentEnd);

                        // describe character array
                        resultTextSource += String.Format("{0}{{ start character, end character, ptr to descriptor block array }}{1}" + nl,
                                                            outConfig.CommentStart,
                                                            outConfig.CommentEnd);
                    }

                    // format the block lookup header
                    resultTextSource += String.Format("const FONT_CHAR_INFO_LOOKUP {0}[] = " + nl + "{{" + nl,
                                                        getCharacterDescriptorArrayLookupDisplayString(font));

                    // iterate
                    foreach (CharacterDescriptorArrayBlock block in characterBlockList)
                    {
                        // get first/last chars
                        CharacterDescriptorArrayBlock.Character firstChar = (CharacterDescriptorArrayBlock.Character)block.characters[0],
                                                                lastChar = (CharacterDescriptorArrayBlock.Character)block.characters[block.characters.Count - 1];

                        // create current block description
                        resultTextSource += String.Format("\t{{{0}, {1}, &{2}}}," + nl,
                                                                    getCharacterDisplayString(cpi, firstChar.character),
                                                                    getCharacterDisplayString(cpi, lastChar.character),
                                                                    charDescArrayGetBlockName(Array.IndexOf(characterBlockList, block), false, true));
                    }

                    // terminate block lookup
                    resultTextSource += "};" + nl + nl;
                }
            }

            // get character descriptor array block name
            private string charDescArrayGetBlockName(int currentBlockIndex,
                                                     bool includeTypeDefinition,
                                                     bool includeBlockIndex)
            {
                // get block id
                string blockIdString = String.Format("Block{0}", currentBlockIndex);

                // variable name
                string variableName = String.Format(outConfig.varNfCharInfo, getFontName(font));

                // remove type unless required
                if (!includeTypeDefinition) variableName = getVariableNameFromExpression(variableName);

                // return the block name
                return String.Format("{0}{1}{2}",
                                        variableName,
                                        includeBlockIndex ? blockIdString : "",
                                        includeTypeDefinition ? "[]" : "");
            }
            private static string getCharacterDescriptorArrayLookupDisplayString(Font font)
            {
                // return the string
                return String.Format("{0}BlockLookup", getFontName(font));
            }


            // holds a range of chars
            public class CharacterDescriptorArrayBlock
            {
                public CharacterDescriptorArrayBlock()
                {
                    characters = new List<Character>();
                }
                // characters
                public List<Character> characters;

                // holds a range of chars
                public class Character
                {
                    public FontInfo font;
                    public char character;
                    public int height;
                    public int width;
                    public int offset;
                }

                // add a character to the current char descriptor array
                public void charDescArrayAddCharacter(FontInfo fontInfo,
                                                       char character,
                                                       int width, int height, int offset)
                {
                    // create character descriptor
                    CharacterDescriptorArrayBlock.Character charDescriptor = new CharacterDescriptorArrayBlock.Character();
                    charDescriptor.character = character;
                    charDescriptor.font = fontInfo;
                    charDescriptor.height = height;
                    charDescriptor.width = width;
                    charDescriptor.offset = offset;

                    // shove this character to the descriptor block
                    characters.Add(charDescriptor);
                }

                // add a character to the current char descriptor array
                private static void charDescArrayAddCharacter(CharacterDescriptorArrayBlock desciptorBlock,
                                                       FontInfo fontInfo,
                                                       char character,
                                                       int width, int height, int offset)
                {
                    // create character descriptor
                    CharacterDescriptorArrayBlock.Character charDescriptor = new CharacterDescriptorArrayBlock.Character();
                    charDescriptor.character = character;
                    charDescriptor.font = fontInfo;
                    charDescriptor.height = height;
                    charDescriptor.width = width;
                    charDescriptor.offset = offset;

                    // shove this character to the descriptor block
                    desciptorBlock.characters.Add(charDescriptor);
                }
            }
            #endregion
        }

        // to allow mapping string/value
        class ComboBoxItem
        {
            public string name;
            public string value;

            // ctor
            public ComboBoxItem(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            // override ToString() function
            public override string ToString()
            {
                // use name
                return this.name;
            }
        }

        // a bitmap border conta
        struct Border
        {
            public static readonly Border Default = new Border() { Bottom = 0, Right = 0, Top = int.MaxValue, Left = int.MaxValue };

            public int Bottom;
            public int Right;
            public int Top;
            public int Left;
            public int All
            {
                get
                {
                    if (Bottom == Right
                          && Bottom == Top
                          && Bottom == Left) return Bottom;
                    else return -1;
                }
                set
                {
                    Bottom = Right = Left = Top = value;
                }
            }

            public Border(Size s)
            {
                Bottom = 0;
                Right = 0;
                Top = s.Height;
                Left = s.Width;
            }

            public override string ToString()
            {
                return string.Format("Bottom = {0} Right = {1} Top = {2} Left = {3}", Bottom, Right, Top, Left);
            }
        }

        // character generation information
        public class CharacterGenerationInfo
        {
            // pointer the font info
            public FontInfo ParentFontInfo;

            // the character
            public char character;

            // the original bitmap
            public Bitmap bitmapOriginal;

            // the bitmap to generate into a string (flipped, trimmed - if applicable)
            public Bitmap bitmapToGenerate;

            // value of pages (vertical 8 bits), in serial order from top of bitmap
            public byte[] pages;

            // character size
            public int width;
            public int height;
            public Size Size
            {
                get { return new Size(width, height); }
                set { width = value.Width; height = value.Height; }
            }

            // offset into total array
            public int offsetInBytes;

            // convert a letter to bitmap
            public static Bitmap ConvertCharacterToBitmap(char character, Font font, Size size)
            {
                // get the string
                string letterString = character.ToString();

                // create bitmap, sized to the correct size
                Bitmap outputBitmap = new Bitmap(size.Width, size.Height);

                // create grahpics entity for drawing
                Graphics gfx = Graphics.FromImage(outputBitmap);

                // disable anti alias
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                // draw centered text
                Rectangle bitmapRect = new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height);

                // Set format of string.
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;

                // draw the character
                gfx.FillRectangle(Brushes.White, bitmapRect);
                gfx.DrawString(letterString, font, Brushes.Black, bitmapRect, drawFormat);

                return outputBitmap;
            }

            // create the page array
            public static byte[] ConvertBitmapToPageArray(Bitmap bitmap, bool ColumnMajor, bool MsbFirst)
            {
                // create pages
                byte[] pages = new byte[0];
                int[] pixels = BitmapToArray(bitmap);
                int width = bitmap.Width, height = bitmap.Height;
                ConcurrentDictionary<int, List<byte>> dpages = new ConcurrentDictionary<int, List<byte>>();

                Func<int, int, Color> getPixel = delegate (int x, int y)
                {
                    return Color.FromArgb(pixels[y * width + x]);
                };

                Action<int> func = delegate (int row)
                {
                    dpages.GetOrAdd(row, new List<byte>());
                    // current byte value
                    byte currentValue = 0, bitsRead = 0;

                    // for each column
                    for (int column = 0; column < width; ++column)
                    {
                        // is pixel set?
                        if (!(getPixel(column, row) == Color.FromArgb(255, Color.White)))
                        {
                            // set the appropriate bit in the page
                            if (MsbFirst) currentValue |= (byte)(1 << (7 - bitsRead));
                            else currentValue |= (byte)(1 << bitsRead);
                        }

                        // increment number of bits read
                        ++bitsRead;

                        // have we filled a page?
                        if (bitsRead == 8)
                        {
                            // add byte to page array
                            dpages[row].Add(currentValue);

                            // zero out current value
                            currentValue = 0;

                            // zero out bits read
                            bitsRead = 0;
                        }
                    }

                    // if we have bits left, add it as is
                    if (bitsRead != 0) dpages[row].Add(currentValue);
                };

                // for each row
                if (RunInParallel) Parallel.For(0, height, func);
                else for (int row = 0; row < height; row++) func(row);

                List<byte> tempPages = new List<byte>();
                for (int i = 0; i < dpages.Count; i++)
                {
                    tempPages.AddRange(dpages[i]);
                }
                pages = tempPages.ToArray();

                // transpose the pages if column major data is requested
                if (ColumnMajor)
                {
                    pages = transposePageArray(bitmap.Width, bitmap.Height, pages, MsbFirst);
                }
                return pages;
            }

            public override string ToString()
            {
                return character.ToString();
            }
        }

        // generate an array of column major pages from row major pages
        private static byte[] transposePageArray(int width, int height, byte[] rowMajorPages, bool MsbFirst)
        {
            // column major data has a byte for each column representing 8 rows
            int rowMajorPagesPerRow = (width + 7) / 8;
            int colMajorPagesPerRow = width;
            int colMajorRowCount = (height + 7) / 8;

            // create an array of pages filled with zeros for the column major data
            List<byte> tempPages = new List<byte>(colMajorPagesPerRow * colMajorRowCount);

            // generate the column major data
            //Parallel.For(0, height, delegate (int row)
            for (int row = 0; row != height; ++row)
            {
                for (int col = 0; col != width; ++col)
                {
                    // get the byte containing the bit we want
                    int srcIdx = row * rowMajorPagesPerRow + (col / 8);
                    int page = rowMajorPages[srcIdx];

                    // get the bit mask for the bit we want
                    int bitMask = getBitMask(MsbFirst, 7 - (col % 8));

                    // set the bit in the column major data
                    if ((page & bitMask) != 0)
                    {
                        int dstIdx = (row / 8) * colMajorPagesPerRow + col;

                        tempPages[dstIdx] = (byte)(tempPages[dstIdx] | getBitMask(MsbFirst, row % 8));
                    }
                }
            }
            //);
            return tempPages.ToArray();
        }

         #endregion

        public MainForm()
        {
            InitializeComponent();

            // set UI properties that the designer does not set correctly
            // designer sets MinSize values before initializing the splitter distance which causes an exception
            splitContainer1.SplitterDistance = 340;
            splitContainer1.Panel1MinSize = 287;
            splitContainer1.Panel2MinSize = 260;
        }

        #region event handler

        private void Form1_Load(object sender, EventArgs e)
        {
            // use double buffering
            DoubleBuffered = true;

            // set version
            Text = String.Format("The Dot Factory v.{0}", Application.ProductVersion);

            // set input box
            txtInputText.Text = Properties.Settings.Default.InputText;

            // load font
            fontDlgInputFont.Font = Properties.Settings.Default.InputFont;

            // load configurations from file
            m_outputConfigurationManager.loadFromFile("OutputConfigs.xml");

            // update the dropdown
            m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfiguration);

            // get saved output config index
            int lastUsedOutputConfigurationIndex = Properties.Settings.Default.OutputConfigIndex;

            // load recently used preset
            if (lastUsedOutputConfigurationIndex >= 0 &&
                lastUsedOutputConfigurationIndex < cbxOutputConfiguration.Items.Count)
            {
                // last used
                cbxOutputConfiguration.SelectedIndex = lastUsedOutputConfigurationIndex;

                // load selected configuration
                m_outputConfig = m_outputConfigurationManager.configurationGetAtIndex(lastUsedOutputConfigurationIndex);
            }
            else
            {
                // there's no saved configuration. get the working copy (default)
                m_outputConfig = m_outputConfigurationManager.workingOutputConfiguration;
            }

            // set checkbox stuff
            populateTextInsertCheckbox();

            // apply font to all appropriate places
            updateSelectedFont(Properties.Settings.Default.InputFont);
        }

        // force a redraw on size changed
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Refresh();
        }

        private void btnFontSelect_Click(object sender, EventArgs e)
        {
            // set focus somewhere else
            label1.Focus();

            // open font chooser dialog
            if (fontDlgInputFont.ShowDialog(this) == DialogResult.OK)
            {
                updateSelectedFont(fontDlgInputFont.Font);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // set focus somewhere else
            label1.Focus();

            // save default input text
            Properties.Settings.Default.InputText = txtInputText.Text;
            Properties.Settings.Default.Save();

            // will hold the resutl string            
            string resultStringSource = "";
            string resultStringHeader = "";

            // check which tab is active
            if (tcInput.SelectedTab.Text == "Text")
            {
                // generate output text
                generateOutputForFont(fontDlgInputFont.Font, ref resultStringSource, ref resultStringHeader);
            }
            else if (tcInput.SelectedTab.Text == "Image")
            {
                // generate output bitmap
                generateOutputForImage(ref m_currentLoadedBitmap, ref resultStringSource, ref resultStringHeader);
            }
            else throw new Exception("Unknowen tabpage");

            txtOutputTextSource.Text = resultStringSource;
            txtOutputTextHeader.Text = resultStringHeader;
        }

        private void btnBitmapLoad_Click(object sender, EventArgs e)
        {
            // set filter
            dlgOpenFile.Filter = string.Format("Image Files ({0})|{0}", "*.jpg; *.jpeg; *.gif; *.bmp; *.png");

            // open the dialog
            if (dlgOpenFile.ShowDialog() != DialogResult.Cancel)
            {
                if (m_currentLoadedBitmap != null) m_currentLoadedBitmap.Dispose();
                m_currentLoadedBitmap = ChangePixelFormat(new Bitmap(dlgOpenFile.FileName), PixelFormat.Format32bppArgb);

                // try to open the bitmap
                pbxBitmap.Image = m_currentLoadedBitmap;
                pbxBitmap.Size = m_currentLoadedBitmap.Size;
                // set the path
                txtImagePath.Text = dlgOpenFile.FileName;

                // guess a name
                txtImageName.Text = Path.GetFileNameWithoutExtension(dlgOpenFile.FileName);

                //
                colorList = GetColorListFromImage(m_currentLoadedBitmap).ToDictionary<Color, Color, bool>(x => x, x => false);
                colorList[colorList.ElementAt(0).Key] = true;

                dataGridViewBackgroundColor.RowCount = colorList.Count;
                dataGridViewBackgroundColor.Refresh();

                /*listViewColor.VirtualListSize = colorList.Count;
                listViewColor.Refresh();
                */
                // Set picterbox background
                pbxBitmap.BackColor = Color.FromArgb(255, GuessBackColor());
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // close self
            Close();
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            // no focus
            label1.Focus();
        }

        private void btnInsertText_Click(object sender, EventArgs e)
        {
            // no focus
            label1.Focus();

            // insert text
            txtInputText.Text += ((ComboBoxItem)cbxTextInsert.SelectedItem).value;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // about
            AboutForm about = new AboutForm();
            about.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // show teh about form
            about.Show();
        }

        private void btnOutputConfig_Click(object sender, EventArgs e)
        {
            // no focus
            label1.Focus();

            // get it
            OutputConfigurationForm outputConfigForm = new OutputConfigurationForm(ref m_outputConfigurationManager);

            // get the oc
            int selectedConfigurationIndex = outputConfigForm.getOutputConfiguration(cbxOutputConfiguration.SelectedIndex);

            // update the dropdown
            m_outputConfigurationManager.comboBoxPopulate(cbxOutputConfiguration);

            // get working configuration
            m_outputConfig = m_outputConfigurationManager.workingOutputConfiguration;

            // set selected index
            cbxOutputConfiguration.SelectedIndex = selectedConfigurationIndex;
        }

        private void cbxOutputConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if any configuration selected
            if (cbxOutputConfiguration.SelectedIndex != -1)
            {
                // get the configuration
                m_outputConfig = m_outputConfigurationManager.configurationGetAtIndex(cbxOutputConfiguration.SelectedIndex);
            }

            // save selected index for next time
            Properties.Settings.Default.OutputConfigIndex = cbxOutputConfiguration.SelectedIndex;

            // save
            Properties.Settings.Default.Save();
        }

        private void tsmCopySource_Click(object sender, EventArgs e)
        {
            // copy if any text
            if (txtOutputTextSource.Text != "")
            {
                // copy
                Clipboard.SetText(txtOutputTextSource.Text);
            }
        }

        private void tsmCopyHeader_Click(object sender, EventArgs e)
        {
            // copy if any text
            if (txtOutputTextHeader.Text != "")
            {
                // copy
                Clipboard.SetText(txtOutputTextHeader.Text);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zero out file name
            dlgSaveAs.FileName = "";

            // try to prompt
            if (dlgSaveAs.ShowDialog() != DialogResult.Cancel)
            {
                // get the file name
                string moduleName = dlgSaveAs.FileName;

                // save the text
                txtOutputTextSource.SaveToFile(String.Format("{0}.c", moduleName), Encoding.UTF8);
                txtOutputTextHeader.SaveToFile(String.Format("{0}.h", moduleName), Encoding.UTF8);
            }
        }

        private void buttonImageColorInvert_Click(object sender, EventArgs e)
        {
            colorList = colorList.ToDictionary(p => p.Key, p => !p.Value);
            dataGridViewBackgroundColor.Refresh();
            //listViewColor.Refresh();
        }

        private void buttonImageColorAuto_Click(object sender, EventArgs e)
        {
            float min = 1, max = 0, limit = 0;

            if (colorList.Count <= 0) return;

            max = colorList.Max(x => x.Key.GetBrightness());
            min = colorList.Min(x => x.Key.GetBrightness());

            limit = (max - min) / 2 + min;

            colorList = colorList.ToDictionary(p => p.Key, p => p.Key.GetBrightness() >= limit);

            dataGridViewBackgroundColor.Refresh();
            //listViewColor.Refresh();
        }

        #region dataGridViewBackgroundColor
        private void dataGridViewBackgroundColor_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
                e.Value = colorList.ElementAt(e.RowIndex).Value;
            else if (e.ColumnIndex == 1)
            {
                e.Value = colorList.ElementAt(e.RowIndex).Key;
                dataGridViewBackgroundColor[e.ColumnIndex, e.RowIndex].Style.BackColor = (Color)e.Value;
            }
        }

        private void dataGridViewBackgroundColor_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                colorList[dataGridViewBackgroundColor[1, e.RowIndex].Style.BackColor]
                    = (bool)dataGridViewBackgroundColor[0, e.RowIndex].EditedFormattedValue;

            }
        }

        private void dataGridViewBackgroundColor_CurrentCellDirtyStateChanged_1(object sender, EventArgs e)
        {
            dataGridViewBackgroundColor.EndEdit();
        }

        #endregion
        #endregion

        #region form helper funktions
        // update input font
        private void updateSelectedFont(Font fnt)
        {
            // set text name in the text box
            txtInputFont.Text = fnt.Name;

            // add to text
            txtInputFont.Text += " " + Math.Round(fnt.Size) + "pts";

            // check if bold
            if (fnt.Bold)
            {
                // add to text
                txtInputFont.Text += " / Bold";
            }

            // check if italic
            if (fnt.Italic)
            {
                // add to text
                txtInputFont.Text += " / Italic";
            }

            // set the font in the text box
            txtInputText.Font = (Font)fnt.Clone();

            // save into settings
            Properties.Settings.Default.InputFont = fnt;
            Properties.Settings.Default.Save();
        }

        // populate preformatted text
        private void populateTextInsertCheckbox()
        {
            string allEnglish = "", numbers = "", letters = "", uppercaseLetters = "", lowercaseLetters = "", symbols = "";

            // generate characters
            for (char character = ' '; character < 127; ++character)
            {
                // add to all
                allEnglish += character;

                // classify letter
                if (Char.IsNumber(character)) numbers += character;
                else if (Char.IsSymbol(character)) symbols += character;
                else if (Char.IsLetter(character) && Char.IsLower(character)) { letters += character; lowercaseLetters += character; }
                else if (Char.IsLetter(character) && !Char.IsLower(character)) { letters += character; uppercaseLetters += character; }
            }

            string allEuropean = allEnglish, extraPunctuations = "", extraSymbols = "", extraNumbers = "";

            for (char character = (char)129; character <= 255; ++character)
            {
                if (Char.IsLetter(character)) allEuropean += character;
                if (Char.IsPunctuation(character)) extraPunctuations += character;
                if (Char.IsSymbol(character)) extraSymbols += character;
                if (Char.IsNumber(character)) extraNumbers += character;
            }

            // add items
            cbxTextInsert.Items.Add(new ComboBoxItem("All European", allEuropean));
            cbxTextInsert.Items.Add(new ComboBoxItem("All English(ASCCI)", allEnglish));
            cbxTextInsert.Items.Add(new ComboBoxItem("Numbers (0-9)", numbers));
            cbxTextInsert.Items.Add(new ComboBoxItem("Letters (A-z)", letters));
            cbxTextInsert.Items.Add(new ComboBoxItem("Lowercase letters (a-z)", lowercaseLetters));
            cbxTextInsert.Items.Add(new ComboBoxItem("Uppercase letters (A-Z)", uppercaseLetters));
            cbxTextInsert.Items.Add(new ComboBoxItem("Extra Punctuations", extraPunctuations));
            cbxTextInsert.Items.Add(new ComboBoxItem("Extra Symbols", extraSymbols));
            cbxTextInsert.Items.Add(new ComboBoxItem("Extra Numbers", extraNumbers));

            foreach (string s in CodePageInfo.GetEncoderNameList())
            {
                if(s.ToLower() != "us-ascii"
                    //&& s.ToLower() != "utf-16"
                    )
                    cbxTextInsert.Items.Add(new ComboBoxItem(s, CodePageInfo.GetAllValidCharacter(s)));
            }

            // use first
            cbxTextInsert.SelectedIndex = 0;
        }
        #endregion

        private static int[] BitmapToArray(Bitmap bmp)
        {
            int[] Pixels;
            BitmapData bd;
            Bitmap copy;

            if (bmp == null) throw new ArgumentNullException("bmp");
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
            {
                copy = ChangePixelFormat(bmp, PixelFormat.Format32bppArgb);
            }
            else copy = bmp;

            bd = copy.LockBits(new Rectangle(0, 0, copy.Width, copy.Height), ImageLockMode.ReadOnly, copy.PixelFormat);

            Pixels = new int[copy.Width * copy.Height];

            // Copy data from pointer to array
            Marshal.Copy(bd.Scan0, Pixels, 0, Pixels.Length);

            copy.UnlockBits(bd);

            return Pixels;
        }

        /// <summary>
        /// Changes the pixelformat of a given bitmap into any of the GDI+ supported formats.
        /// </summary>
        /// <param name="oldBmp">Die Bitmap die verändert werden soll.</param>
        /// <param name="NewFormat">Das neu anzuwendende Pixelformat.</param>
        /// <returns>Die Bitmap mit dem neuen PixelFormat</returns>
        private static Bitmap ChangePixelFormat(Bitmap oldBmp, PixelFormat NewFormat)
        {
            return (oldBmp.Clone(new Rectangle(0, 0, oldBmp.Width, oldBmp.Height), NewFormat));
        }

        private Color[] GetColorListFromImage(Bitmap bmp)
        {
            return BitmapToArray(bmp).Distinct().Select<int, Color>(x => Color.FromArgb(x)).ToArray();
        }

        private Color GuessBackColor()
        {
            List<Color> trans = new List<Color>();

            foreach (KeyValuePair<Color, bool> c in colorList)
            {
                if (c.Key.A == 0) trans.Add(c.Key);
            }

            if (trans.Count == 1)
            {
                return trans[0];
            }

            return Color.White;
        }

        #region Image
        // generate the required output for image
        private void generateOutputForImage(ref Bitmap bitmapOriginal, ref string resultTextSource, ref string resultTextHeader)
        {
            // the name of the bitmap
            string imageName = scrubVariableName(txtImageName.Text);

            // check if bitmap is assigned
            if (m_currentLoadedBitmap != null)
            {
                //
                // Bitmap manipulation
                //

                // get bitmap border
                Border bitmapBorder = Border.Default;
                getBitmapBorder(bitmapOriginal, ref bitmapBorder, true, colorList);

                // manipulate the bitmap
                Bitmap bitmapManipulated;

                // try to manipulate the bitmap
                if (!manipulateBitmap(bitmapOriginal, bitmapBorder, out bitmapManipulated, 0, 0, true, colorList, m_outputConfig))
                {
                    // show error
                    MessageBox.Show("No black pixels found in bitmap (currently only monochrome bitmaps supported)",
                                    "Can't convert bitmap",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    // stop here, failed to manipulate the bitmap for whatever reason
                    return;
                }

                // for debugging
                // bitmapManipulated.Save(String.Format("C:/bms/manip.bmp"));

                // according to config
                if (m_outputConfig.commentVariableName)
                {
                    // add source file header
                    resultTextSource += String.Format("{0}" + nl + "{1} Image data for {2}" + nl + "{3}" + nl + nl,
                                                        m_outputConfig.CommentStart, m_outputConfig.CommentBlockMiddle, imageName,
                                                        m_outputConfig.CommentBlockEnd);

                    // add header file header
                    resultTextHeader += String.Format("{0}Bitmap info for {1}{2}" + nl,
                                                        m_outputConfig.CommentStart, imageName,
                                                        m_outputConfig.CommentEnd);
                }

                // bitmap varname
                string bitmapVarName = String.Format(m_outputConfig.varNfImageBitmap, imageName) + "[]";

                // add to header
                //resultTextHeader += String.Format("extern {0};" + nl, bitmapVarName);

                // add to source
                resultTextSource += String.Format("{0} =" + nl + "{{" + nl, bitmapVarName);

                //
                // Bitmap to string
                //

                // page array
                byte[] pages;

                // first convert to pages
                pages = CharacterGenerationInfo.ConvertBitmapToPageArray(
                    bitmapManipulated, 
                    m_outputConfig.bitLayout == OutputConfiguration.BitLayout.ColumnMajor,
                    m_outputConfig.byteOrder == OutputConfiguration.ByteOrder.MsbFirst);

                // assign pages for fully populated 8 bits
                int pagesPerRow = convertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat.DisplayInBytes, bitmapManipulated.Width);

                // now convert to string
                resultTextSource += generateStringFromPageArray(bitmapManipulated.Width, bitmapManipulated.Height, pages, m_outputConfig);

                // close
                resultTextSource += String.Format("}};" + nl + nl);

                // according to config
                if (m_outputConfig.commentVariableName)
                {
                    // set sizes comment
                    resultTextSource += String.Format("{0}Bitmap sizes for {1}{2}" + nl,
                                                        m_outputConfig.CommentStart, imageName, m_outputConfig.CommentEnd);
                }

                int imageWidth;
                string imageWidthComment;
                // display width in bytes?
                if (m_outputConfig.descImgWidth == OutputConfiguration.DescriptorFormat.DisplayInBytes)
                {
                    // in pages
                    imageWidth = pagesPerRow;
                    imageWidthComment = "Image width in bytes (pages)";
                }
                else
                {
                    // in pixels
                    imageWidth = bitmapManipulated.Width;
                    imageWidthComment = "Image width in pixels";
                }

                int imageHeight;
                string imageHeightComment;
                // display height in bytes?
                if (m_outputConfig.descImgHeight == OutputConfiguration.DescriptorFormat.DisplayInBytes)
                {
                    // in pages
                    imageHeight = convertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat.DisplayInBytes, bitmapManipulated.Height);
                    imageHeightComment = "Image height in bytes (pages)";
                }
                else
                {
                    // in pixels
                    imageHeight = bitmapManipulated.Height;
                    imageHeightComment = "Image height in pixels";
                }

                // get var name
                string imageInfoVarName = String.Format(m_outputConfig.varNfImageInfo, imageName);

                // image info header
                resultTextHeader += String.Format("extern {0};" + nl, imageInfoVarName);

                // image info source
                resultTextSource += String.Format("{2} =" + nl + "{{" + nl +
                                                  "\t{3}, {0} {4}{1}" + nl +
                                                  "\t{5}, {0} {6}{1}" + nl +
                                                  "\t{7}, {0} Image bitmap array{1}" + nl +
                                                  "}};" + nl,
                                                  m_outputConfig.CommentStart,
                                                  m_outputConfig.CommentEnd,
                                                  imageInfoVarName,
                                                  imageWidth,
                                                  imageWidthComment,
                                                  imageHeight,
                                                  imageHeightComment,
                                                  getVariableNameFromExpression(bitmapVarName));

            }
        }

        // get rotate flip type according to config
        private static RotateFlipType getOutputRotateFlipType(bool flipX, bool flipY, OutputConfiguration.Rotation rot)
        {
            // zero degree rotation
            if (rot == OutputConfiguration.Rotation.RotateZero)
            {
                // return according to flip
                if (!flipX && !flipY) return RotateFlipType.RotateNoneFlipNone;
                if (flipX && !flipY) return RotateFlipType.RotateNoneFlipX;
                if (!flipX && flipY) return RotateFlipType.RotateNoneFlipY;
                if (flipX && flipY) return RotateFlipType.RotateNoneFlipXY;
            }

            // 90 degree rotation
            if (rot == OutputConfiguration.Rotation.RotateNinety)
            {
                // return according to flip
                if (!flipX && !flipY) return RotateFlipType.Rotate90FlipNone;
                if (flipX && !flipY) return RotateFlipType.Rotate90FlipX;
                if (!flipX && flipY) return RotateFlipType.Rotate90FlipY;
                if (flipX && flipY) return RotateFlipType.Rotate90FlipXY;
            }

            // 180 degree rotation
            if (rot == OutputConfiguration.Rotation.RotateOneEighty)
            {
                // return according to flip
                if (!flipX && !flipY) return RotateFlipType.Rotate180FlipNone;
                if (flipX && !flipY) return RotateFlipType.Rotate180FlipX;
                if (!flipX && flipY) return RotateFlipType.Rotate180FlipY;
                if (flipX && flipY) return RotateFlipType.Rotate180FlipXY;
            }

            // 270 degree rotation
            if (rot == OutputConfiguration.Rotation.RotateTwoSeventy)
            {
                // return according to flip
                if (!flipX && !flipY) return RotateFlipType.Rotate270FlipNone;
                if (flipX && !flipY) return RotateFlipType.Rotate270FlipX;
                if (!flipX && flipY) return RotateFlipType.Rotate270FlipY;
                if (flipX && flipY) return RotateFlipType.Rotate270FlipXY;
            }

            // unknown case, but just return no flip
            return RotateFlipType.RotateNoneFlipNone;
        }
        #endregion

        #region Font
        // generate the required output for text
        private void generateOutputForFont(Font font, ref string resultTextSource, ref string resultTextHeader)
        {
            char[] charactersToGenerate;

            // get the characters we need to generate from the input text, removing duplicates
            charactersToGenerate = getCharactersToGenerate(txtInputText.Text, m_outputConfig.CodePage, m_outputConfig.generateSpaceCharacterBitmap);

            // do nothing if no chars defined
            if (charactersToGenerate.Length == 0) return;

            // according to config
            if (m_outputConfig.commentVariableName)
            {
                // add source file header
                resultTextSource += String.Format("{0}" + nl + "{1} Font data for {2} {3}pt" + nl + "{4}" + nl + nl,
                                                    m_outputConfig.CommentStart, m_outputConfig.CommentBlockMiddle, font.Name, Math.Round(font.Size),
                                                    m_outputConfig.CommentBlockEnd);

                // add header file header
                resultTextHeader += String.Format("{0}Font data for {1} {2}pt{3}" + nl,
                                                    m_outputConfig.CommentStart, font.Name, Math.Round(font.Size),
                                                    m_outputConfig.CommentEnd);
            }

            // populate the font info
            FontInfo fontInfo = FontInfo.GenerateFromFont(font, m_outputConfig, charactersToGenerate);

            // We now have all information required per font and per character. 
            // time to generate the string
            fontInfo.GenerateStringsFromFontInfo(ref resultTextSource, ref resultTextHeader);
        }

        // get the characters we need to generate
        private static char[] getCharactersToGenerate(string inputText, int codePage, bool generateSpace)
        {

            CodePageInfo cpi = new CodePageInfo(codePage);
            //
            // Expand and remove all ranges from the input text (look for << x - y >>
            //

            // expand the ranges into the input text
            inputText = expandAndRemoveCharacterRanges(inputText, codePage);

            //
            // iterate through the inputted text and shove to sorted string, removing all duplicates
            //
            ConcurrentBag<char> characterList = new ConcurrentBag<char>();
            ConcurrentBag<char> CodePageCharacterList = new ConcurrentBag<char>(cpi.GetAllValidCharacter());

            // iterate over the characters in the textbox
            Parallel.For(0, inputText.Length,
                delegate (int charIndex)
                {
                    // get the char
                    char insertionCandidateChar = inputText[charIndex];

                    // insert the char, if not space ()
                    // check if space character
                    if (insertionCandidateChar == ' ' && !generateSpace)
                    {
                        // skip - space is not encoded rather generated dynamically by the driver
                    }
                    // dont generate newlines or tab
                    else if (insertionCandidateChar == '\n' || insertionCandidateChar == '\r' || insertionCandidateChar == '\t')
                    {
                        // no such characters
                    }
                    // not in list, add
                    else characterList.Add(insertionCandidateChar);
                });
            // remove dublicats and sort
            // remove all charaters not includet in codepage and return
            return CodePageCharacterList
                .AsParallel()
                .Intersect(characterList.AsParallel())
                .Distinct()
                .OrderBy(p => cpi.GetOffsetFromCharacter(p))
                .ToArray();
        }

        // get widest bitmap
        private static Rectangle getLargestBitmapFromCharInfo(CharacterGenerationInfo[] charInfoArray)
        {
            if (RunInParallel)
            {
                ConcurrentBag<int> height = new ConcurrentBag<int>();
                ConcurrentBag<int> width = new ConcurrentBag<int>();

                Action<int> act = delegate (int charIndex)
                {
                    // get the string of the characer
                    string letterString = charInfoArray[charIndex].character.ToString();

                    // measure the size of teh character in pixels
                    Size stringSize = TextRenderer.MeasureText(letterString, charInfoArray[charIndex].ParentFontInfo.font);

                    height.Add(stringSize.Height);
                    width.Add(stringSize.Width);
                };

                Parallel.For(0, charInfoArray.Length, act);
                return new Rectangle(0, 0, width.AsParallel().Max(), height.AsParallel().Max());
            }
            else
            {
                // largest rect
                Rectangle largestRect = new Rectangle(0, 0, 0, 0);

                // iterate through chars
                for (int charIdx = 0; charIdx < charInfoArray.Length; ++charIdx)
                {
                    // get the string of the characer
                    string letterString = charInfoArray[charIdx].character.ToString();

                    // measure the size of teh character in pixels
                    Size stringSize = TextRenderer.MeasureText(letterString, charInfoArray[charIdx].ParentFontInfo.font);

                    // check if larger
                    largestRect.Height = Math.Max(largestRect.Height, stringSize.Height);
                    largestRect.Width = Math.Max(largestRect.Width, stringSize.Width);
                }

                // return largest
                return largestRect;
            }
        }
 
        // expand and remove character ranges ( look for << x - y >> )
        private static string expandAndRemoveCharacterRanges(string s, int codePage)
        {
            // create the search pattern
            //String searchPattern = @"<<.*-.*>>";
            String searchPattern = @"<<(?<rangeStart>.*?)-(?<rangeEnd>.*?)>>";

            // create the regex
            Regex regex = new Regex(searchPattern, RegexOptions.Multiline);

            // get matches
            MatchCollection regexMatches = regex.Matches(s.Replace(" ", ""));

            // holds the number of characters removed
            int charactersRemoved = 0;

            CodePageInfo cp = new CodePageInfo(codePage);

            // for each match
            foreach (Match regexMatch in regexMatches)
            {
                // get range start and end
                int rangeStart = 0, rangeEnd = 0;

                // try to parse ranges
                if (characterRangePointParse(regexMatch.Groups["rangeStart"].Value, ref rangeStart) &&
                    characterRangePointParse(regexMatch.Groups["rangeEnd"].Value, ref rangeEnd))
                {
                    // remove this from the string
                    s = s.Remove(regexMatch.Index - charactersRemoved, regexMatch.Length);

                    // save the number of chars removed so that we can fixup index (the index
                    // of the match changes as we remove characters)
                    charactersRemoved += regexMatch.Length;

                    // create a string from these values
                    for (int charIndex = rangeStart; charIndex <= rangeEnd; ++charIndex)
                    {
                        // shove this character to a encodet char container
                        char unicodeChar = cp.GetCharacterFromOffset(charIndex);

                        // add this to the string
                        s += unicodeChar;
                    }
                }
            }

            return s;
        }
       
        // iterate through the original bitmaps and find the tightest common border
        private static void findTightestCommonBitmapBorder(CharacterGenerationInfo[] charInfoArray,
                                                    ref Border tightestBorder, bool image, Dictionary<Color, bool> colorList)
        {
            // iterate through bitmaps
            for (int charIdx = 0; charIdx < charInfoArray.Length; ++charIdx)
            {
                // create a border
                Border bitmapBorder = Border.Default;

                // get the bitmaps border
                getBitmapBorder(charInfoArray[charIdx].bitmapOriginal, ref bitmapBorder, image, colorList);

                // check if we need to loosen up the tightest border
                tightestBorder.Left = Math.Min(bitmapBorder.Left, tightestBorder.Left);
                tightestBorder.Top = Math.Min(bitmapBorder.Top, tightestBorder.Top);
                tightestBorder.Right = Math.Max(bitmapBorder.Right, tightestBorder.Right);
                tightestBorder.Bottom = Math.Max(bitmapBorder.Bottom, tightestBorder.Bottom);
            }
        }

        // try to parse character range
        private static bool characterRangePointParse(string rangePointString, ref int rangePoint)
        {
            // trim the string
            rangePointString = rangePointString.Trim();

            // try to convert
            try
            {
                // check if 0x is start of range
                if (rangePointString.Substring(0, 2) == "0x")
                {
                    // remove 0x
                    rangePointString = rangePointString.Substring(2, rangePointString.Length - 2);

                    // do the parse
                    rangePoint = Int32.Parse(rangePointString, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    // do the parse
                    rangePoint = Int32.Parse(rangePointString);
                }
            }
            catch (Exception exc)
            {
                if (exc is ArgumentException ||
                    exc is ArgumentNullException ||
                    exc is FormatException ||
                    exc is OverflowException)
                {
                    // error converting
                    return false;
                }
                else throw;
            }

            // success
            return true;
        }

        // get the character descriptor string
        private static string getCharacterDescString(OutputConfiguration.DescriptorFormat descFormat, int valueInBits)
        {
            // don't display
            if (descFormat == OutputConfiguration.DescriptorFormat.DontDisplay) return "";

            // add comma and return
            return String.Format("{0}, ", convertValueByDescriptorFormat(descFormat, valueInBits));
        }

        // get the character descriptor string
        private static string getCharacterDescName(string name, OutputConfiguration.DescriptorFormat descFormat)
        {
            // don't display
            if (descFormat == OutputConfiguration.DescriptorFormat.DontDisplay) return "";

            // create result string
            string descFormatName = "";

            // set value
            if (descFormat == OutputConfiguration.DescriptorFormat.DisplayInBits) descFormatName = "bits";
            if (descFormat == OutputConfiguration.DescriptorFormat.DisplayInBytes) descFormatName = "bytes";

            // add comma and return
            return String.Format("[Char {0} in {1}], ", name, descFormatName);
        }

        // get the display string for a character 
        private static string getCharacterDisplayString(CodePageInfo codePage, char character)
        {
            // return string
            return codePage.GetOffsetFromCharacter(character).ToString();
        }

        // convert bits to bytes according to desc format
        private static int convertValueByDescriptorFormat(OutputConfiguration.DescriptorFormat descFormat, int valueInBits)
        {
            // according to format
            if (descFormat == OutputConfiguration.DescriptorFormat.DisplayInBytes)
            {
                // get value in bytes
                int valueInBytes = valueInBits / 8;
                if (valueInBits % 8 != 0) valueInBytes++;

                // set into string
                return valueInBytes;
            }
            else
            {
                // no conversion required
                return valueInBits;
            }
        }
     
        #endregion

        #region fontbitmap

        #endregion

        
        // get the bitmaps border - that is where the black parts start
        private static bool getBitmapBorder(Bitmap bitmap, ref Border border, bool image, Dictionary<Color, bool> colorList)
        {
            int[] pixel = BitmapToArray(bitmap);
            Border b = border;
            int width = bitmap.Width, height = bitmap.Height;
            Func<Color, bool> isBackGroundColor = delegate (Color c) { return false; };

            Func<int, int, Color> getPixel = delegate (int x, int y)
            {
                return Color.FromArgb(pixel[y * width + x]);
            };

            // returns whether a bitmap column is empty (empty means all is back color)
            Func<int, bool> columnIsEmpty = delegate (int column)
            {
                // for each row in the column
                for (int row = 0; row < height; ++row)
                {
                    // is the pixel black?
                    if (!isBackGroundColor(getPixel(column, row)))
                    {
                        // found. column is not empty
                        return false;
                    }
                }

                // column is empty
                return true;
            };

            // returns whether a bitmap row is empty (empty means all is back color)
            Func<int, bool> rowIsEmpty = delegate (int row)
            {
                // for each column in the row
                for (int column = 0; column < width; ++column)
                {
                    // is the pixel black?
                    if (!isBackGroundColor(getPixel(column, row)))
                    {
                        // found. row is not empty
                        return false;
                    }
                }

                // row is empty
                return true;
            };

            if (RunInParallel)
            {
                ConcurrentDictionary<Color, bool> colorListP = new ConcurrentDictionary<Color, bool>();
                if (image)
                {
                    colorListP = new ConcurrentDictionary<Color, bool>(colorList);
                }

                Func<Color, bool> isBackGroundColorImageP = delegate (Color c)
                {
                    return colorListP[c];
                };
                Func<Color, bool> isBackGroundColorTextP = delegate (Color c)
                {
                    return Color.White.ToArgb() == c.ToArgb();
                };

                if (image) isBackGroundColor = isBackGroundColorImageP;
                else isBackGroundColor = isBackGroundColorTextP;

                Action findBorderLeftP = delegate ()
                {
                    ParallelLoopResult plrLeft = Parallel.For(0, width, (c, state) =>
                    {
                        if (!columnIsEmpty(c)) state.Break();
                    });
                    if (plrLeft.LowestBreakIteration != null) b.Left = (int)plrLeft.LowestBreakIteration;
                    else b.Left = width;
                };
                Action findBorderRightP = delegate ()
                {
                    ParallelLoopResult plrReight = Parallel.For(0, width, (c, state) =>
                    {
                        if (!columnIsEmpty(width - c - 1)) state.Break();
                    });
                    if (plrReight.LowestBreakIteration != null) b.Right = width - (int)plrReight.LowestBreakIteration - 1;
                    else b.Right = 0;
                };
                Action findBorderTopP = delegate ()
                {
                    ParallelLoopResult plrTop = Parallel.For(0, height, (c, state) =>
                    {
                        if (!rowIsEmpty(c)) state.Break();
                    });
                    if (plrTop.LowestBreakIteration != null) b.Top = (int)plrTop.LowestBreakIteration;
                    else b.Top = height;
                };
                Action findBorderBottomP = delegate ()
                {
                    ParallelLoopResult plrBottom = Parallel.For(0, height, (c, state) =>
                    {
                        if (!rowIsEmpty(height - c - 1)) state.Break();
                    });
                    if (plrBottom.LowestBreakIteration != null) b.Bottom = height - (int)plrBottom.LowestBreakIteration - 1;
                    else b.Bottom = 0;
                };

                Parallel.Invoke(new Action[]
                {
                    findBorderLeftP,
                    findBorderRightP,
                    findBorderTopP,
                    findBorderBottomP,
                });
            }
            else
            {
                Func<Color, bool> isBackGroundColorImageS = delegate (Color c)
                {
                    return colorList[c];
                };
                Func<Color, bool> isBackGroundColorTextS = delegate (Color c)
                {
                    return Color.White.ToArgb() == c.ToArgb();
                };

                if (image) isBackGroundColor = isBackGroundColorImageS;
                else isBackGroundColor = isBackGroundColorTextS;

                Action findBorderLeftS = delegate ()
                {
                    for (b.Left = 0; b.Left < width; ++b.Left)
                    {
                        if (!columnIsEmpty(b.Left)) break;
                    }
                };
                Action findBorderRightS = delegate ()
                {
                    for (b.Right = width - 1; b.Right >= 0; --b.Right)
                    {
                        if (!columnIsEmpty(b.Right)) break;
                    }
                };
                Action findBorderTopS = delegate ()
                {
                    for (b.Top = 0; b.Top < height; ++b.Top)
                    {
                        if (!rowIsEmpty(b.Top)) break;
                    }
                };
                Action findBorderBottomS = delegate ()
                {
                    for (b.Bottom = height - 1; b.Bottom >= 0; --b.Bottom)
                    {
                        if (!rowIsEmpty(b.Bottom)) break;
                    }
                };

                findBorderLeftS();
                findBorderRightS();
                findBorderTopS();
                findBorderBottomS();
            }

            border = b;
            // check if the bitmap contains any black pixels
            if (border.Right == -1)
            {
                // no pixels were found
                return false;
            }
            else
            {
                // at least one black pixel was found
                return true;
            }
        }

        // generate the bitmap we will then use to convert to string (remove pad, flip)
        private static bool manipulateBitmap(Bitmap bitmapOriginal,
                                      Border tightestCommonBorder,
                                      out Bitmap bitmapManipulated,
                                      int minWidth, int minHeight,
                                      bool image,
                                      Dictionary<Color, bool> colorList,
                                      OutputConfiguration config)
        {
            //
            // First, crop
            //

            // get bitmap border - this sets teh crop rectangle to per bitmap, essentially
            Border bitmapCropBorder = Border.Default;
            if (getBitmapBorder(bitmapOriginal, ref bitmapCropBorder, image, colorList) == false && minWidth == 0 && minHeight == 0)
            {
                // no data
                bitmapManipulated = null;

                // bitmap contains no data
                return false;
            }

            // check that width exceeds minimum
            if (bitmapCropBorder.Right - bitmapCropBorder.Left + 1 < 0)
            {
                // replace
                bitmapCropBorder.Left = 0;
                bitmapCropBorder.Right = minWidth - 1;
            }

            // check that height exceeds minimum
            if (bitmapCropBorder.Bottom - bitmapCropBorder.Top + 1 < 0)
            {
                // replace
                bitmapCropBorder.Top = 0;
                bitmapCropBorder.Bottom = minHeight - 1;
            }

            // should we crop hotizontally according to common
            if (config.paddingRemovalHorizontal == OutputConfiguration.PaddingRemoval.Fixed)
            {
                // cropped Y is according to common
                bitmapCropBorder.Top = tightestCommonBorder.Top;
                bitmapCropBorder.Bottom = tightestCommonBorder.Bottom;
            }
            // check if no horizontal crop is required
            else if (config.paddingRemovalHorizontal == OutputConfiguration.PaddingRemoval.None)
            {
                // set y to actual max border of bitmap
                bitmapCropBorder.Top = 0;
                bitmapCropBorder.Bottom = bitmapOriginal.Height - 1;
            }

            // should we crop vertically according to common
            if (config.paddingRemovalVertical == OutputConfiguration.PaddingRemoval.Fixed)
            {
                // cropped X is according to common
                bitmapCropBorder.Left = tightestCommonBorder.Left;
                bitmapCropBorder.Right = tightestCommonBorder.Right;
            }
            // check if no vertical crop is required
            else if (config.paddingRemovalVertical == OutputConfiguration.PaddingRemoval.None)
            {
                // set x to actual max border of bitmap
                bitmapCropBorder.Left = 0;
                bitmapCropBorder.Right = bitmapOriginal.Width - 1;
            }

            // now copy the output bitmap, cropped as required, to a temporary bitmap
            Rectangle rect = new Rectangle(bitmapCropBorder.Left,
                                            bitmapCropBorder.Top,
                                            bitmapCropBorder.Right - bitmapCropBorder.Left + 1,
                                            bitmapCropBorder.Bottom - bitmapCropBorder.Top + 1);

            // clone the cropped bitmap into the generated one
            bitmapManipulated = bitmapOriginal.Clone(rect, bitmapOriginal.PixelFormat);

            // get rotate type
            RotateFlipType flipType = getOutputRotateFlipType(config.flipHorizontal, config.flipVertical, config.rotation);

            // flip the cropped bitmap
            bitmapManipulated.RotateFlip(flipType);

            // bitmap contains data
            return true;
        }

        
        // generate string from character info
        private static string generateStringFromPageArray(int width, int height, byte[] pages, OutputConfiguration config)
        {
            // generate the data rows
            string[] data;
            data = generateData(width, height, pages, config);

            // generate the visualizer
            string[] visualizer;
            visualizer = generateVisualizer(width, height, pages, config);

            // build the result string
            StringBuilder resultString = new StringBuilder();

            // output row major data
            if (config.bitLayout == OutputConfiguration.BitLayout.RowMajor)
            {
                // the visualizer is drawn after the data on the same rows, so they must have the same length
                System.Diagnostics.Debug.Assert(data.Length == visualizer.Length);

                // output the data and visualizer together
                if (config.lineWrap == OutputConfiguration.LineWrap.AtColumn)
                {
                    // one line per row
                    for (int row = 0; row != data.Length; ++row)
                    {
                        resultString.Append("\t").Append(data[row]).Append(visualizer[row]).Append(nl);
                    }
                }
                else if (config.lineWrap == OutputConfiguration.LineWrap.AtBitmap)
                {
                    // one line per bitmap                    
                    for (int row = 0; row != visualizer.Length; ++row)
                    {
                        resultString.Append("\t").Append(visualizer[row]).Append(nl);
                    }
                    resultString.Append("\t");
                    for (int row = 0; row != data.Length; ++row)
                    {
                        resultString.Append(data[row]);
                    }
                    resultString.Append(nl);
                }
            }
            // output column major data
            else if (config.bitLayout == OutputConfiguration.BitLayout.ColumnMajor)
            {
                // output the visualizer
                for (int row = 0; row != visualizer.Length; ++row)
                {
                    resultString.Append("\t").Append(visualizer[row]).Append(nl);
                }

                // output the data
                if (config.lineWrap == OutputConfiguration.LineWrap.AtColumn)
                {
                    // one line per row
                    for (int row = 0; row != data.Length; ++row)
                    {
                        resultString.Append("\t").Append(data[row]).Append(nl);
                    }
                }
                else if (config.lineWrap == OutputConfiguration.LineWrap.AtBitmap)
                {
                    // one line per bitmap
                    resultString.Append("\t");
                    for (int row = 0; row != data.Length; ++row)
                    {
                        resultString.Append(data[row]);
                    }
                    resultString.Append(nl);
                }
            }

            // return the result
            return resultString.ToString();
        }

        // builds a string array of the data in 'pages'
        private static string[] generateData(int width, int height, byte[] pages, OutputConfiguration config)
        {
            string[] data;
            int colCount = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? (width + 7) / 8 : width;
            int rowCount = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? height : (height + 7) / 8;

            data = new string[rowCount];

            Action<int> func = delegate (int row)
            {
                data[row] = "";

                // iterator over columns
                for (int col = 0; col != colCount; ++col)
                {
                    // get the byte to output
                    int page = pages[row * colCount + col];

                    // add leading character
                    data[row] += config.byteLeadingString;

                    // check format
                    if (config.byteFormat == OutputConfiguration.ByteFormat.Hex)
                    {
                        // convert byte to hex
                        data[row] += page.ToString("X").PadLeft(2, '0');
                    }
                    else
                    {
                        // convert byte to binary
                        data[row] += Convert.ToString(page, 2).PadLeft(8, '0');
                    }

                    // add comma
                    data[row] += ", ";
                }
            };

            // iterator over rows
            if (RunInParallel) Parallel.For(0, rowCount, func);
            else for (int row = 0; row != rowCount; ++row) func(row);

            return data;
        }

        // builds a string array visualization of 'pages'
        private static string[] generateVisualizer(int width, int height, byte[] pages, OutputConfiguration config)
        {
            // the number of pages per row in 'pages'
            int colCount = (config.bitLayout== OutputConfiguration.BitLayout.RowMajor) ? (width + 7) / 8 : width;
            int rowCount = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? height : (height + 7) / 8;
            bool MsbFirst = config.byteOrder == OutputConfiguration.ByteOrder.MsbFirst;
            string[] visualizer;

            if (RunInParallel)
            {
                ConcurrentDictionary<int, string> visualizerLines = new ConcurrentDictionary<int, string>();

                Parallel.For(0, rowCount, delegate (int row)
                {
                    // each row is started with a line comment
                    string s = "// ";

                    // iterator over columns
                    for (int col = 0; col != width; ++col)
                    {
                        // get the byte containing the bit we want
                        int page = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor)
                        ? pages[row * colCount + (col / 8)]
                        : pages[(row / 8) * colCount + col];

                        // make a mask to extract the bit we want
                        int bitMask = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor)
                        ? getBitMask(MsbFirst, 7 - (col % 8))
                        : getBitMask(MsbFirst, row % 8);

                        // check if bit is set
                        s += (bitMask & page) != 0 ? config.bmpVisualizerChar : config.bmpVisualizerCharEmpty;
                    }
                    visualizerLines.GetOrAdd(row, s);
                });

                visualizer = new string[rowCount];
                Parallel.ForEach(visualizerLines, delegate(KeyValuePair<int, string> kvp)
                {
                    visualizer[kvp.Key] = kvp.Value;
                });
            }
            else
            {
                visualizer = new string[height];

                for (int row = 0; row != height; ++row)
                {
                    // each row is started with a line comment
                    visualizer[row] = "// ";

                    // iterator over columns
                    for (int col = 0; col != width; ++col)
                    {
                        // get the byte containing the bit we want
                        int page = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor)
                        ? (byte)pages[row * colCount + (col / 8)]
                        : (byte)pages[(row / 8) * colCount + col];

                        // make a mask to extract the bit we want
                        int bitMask = (config.bitLayout == OutputConfiguration.BitLayout.RowMajor)
                        ? getBitMask(MsbFirst,  7 - (col % 8))
                        : getBitMask(MsbFirst,  row % 8);

                        // check if bit is set
                        visualizer[row] += (bitMask & page) != 0 ? config.bmpVisualizerChar : config.bmpVisualizerCharEmpty;
                    }
                }
            }

            // for debugging
            //foreach (var s in visualizer)
            //  System.Diagnostics.Debug.WriteLine(s);

            return visualizer;
        }

        // return a bitMask to pick out the 'bitIndex'th bit allowing for byteOrder
        // MsbFirst: bitIndex = 0 = 0x01, bitIndex = 7 = 0x80
        // LsbFirst: bitIndex = 0 = 0x80, bitIndex = 7 = 0x01
        private static int getBitMask(bool MsbFirst, int bitIndex)
        {
            return MsbFirst
                ? 0x01 << bitIndex
                : 0x80 >> bitIndex;
        }

        // make 'name' suitable as a variable name, starting with '_'
        // or a letter and containing only letters, digits, and '_'
        private static string scrubVariableName(string name)
        {
            // scrub invalid characters from the font name
            StringBuilder outName = new StringBuilder();
            foreach (char ch in name)
            {
                if (Char.IsLetterOrDigit(ch) || ch == '_')
                    outName.Append(ch);
            }

            // prepend '_' if the first character is a number
            if (Char.IsDigit(outName[0]))
                outName.Insert(0, '_');

            // convert the first character to lower case
            outName[0] = Char.ToLower(outName[0]);

            // return name
            return outName.ToString();
        }

        // get the font name and format it
        private static string getFontName(Font font)
        {
            return scrubVariableName(font.Name + "_" + Math.Round(font.Size) + "pt");
        }


        // get only the variable name from an expression in a specific format
        // e.g. input: const far unsigned int my_font[] = ; 
        //      output: my_font[]
        private static string getVariableNameFromExpression(string expression)
        {
            // iterator
            int charIndex = 0;

            // invalid format string
            const string invalidFormatString = "##Invalid format##";

            //
            // Strip array parenthesis
            //

            // search for '[number, zero or more] '
            const string arrayRegexString = @"\[[0-9]*\]";

            // modify the expression
            expression = Regex.Replace(expression, arrayRegexString, "");

            //
            // Find the string between '=' and a space, trimming spaces from end
            //

            // start at the end and look for the letter or number
            for (charIndex = expression.Length - 1; charIndex != 1; --charIndex)
            {
                // find the last character of the variable name
                if (expression[charIndex] != '=' && expression[charIndex] != ' ') break;
            }

            // check that its valid
            if (charIndex == 0) return invalidFormatString;

            // save this index
            int lastVariableNameCharIndex = charIndex;

            // continue looking for a space
            for (charIndex = lastVariableNameCharIndex; charIndex != 0; --charIndex)
            {
                // find the last character of the variable name
                if (expression[charIndex] == ' ') break;
            }

            // check that its valid
            if (charIndex == 0) return invalidFormatString;

            // save this index as well
            int firstVariableNameCharIndex = charIndex + 1;

            // return the substring
            return expression.Substring(firstVariableNameCharIndex, lastVariableNameCharIndex - firstVariableNameCharIndex + 1);
        }
    }
}