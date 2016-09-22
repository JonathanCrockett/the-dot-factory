using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TheDotFactory
{
    // info per font
    class FontDescriptor
    {
        public OutputConfiguration OutConfig { get; private set; }

        private int FixedAbsolutCharHeight { get; set; }
        private char FirstChar { get; set; }
        private char LastChar { get; set; }
        private CharacterDescriptor[] Characters { get; set; }
        public Font Font { get; private set; }
        private int CodePageCode { get { return CodePageInfo.CodePage; } }
        public CodePageInfo CodePageInfo { get; private set; }
        public string TextSource { get; private set; }
        public string TextHeader { get; private set; }

        public FontDescriptor(OutputConfiguration config, char[] characters, Font font)
        {
            Font = font;
            OutConfig = config;
            CodePageInfo = new CodePageInfo(config.CodePage);

            //
            // init char infos
            //
            Characters = characters.Select(c => new CharacterDescriptor(this, c)).ToArray();

            //
            // Find the widest bitmap size we are going to draw
            //
            Size largestBitmap = Characters.Aggregate(new Size(),
                    (size, chi) =>
                    {
                        return new Size(
                            chi.SizeCharacter.Width > size.Width ? chi.SizeCharacter.Width : size.Width,
                            chi.SizeCharacter.Height > size.Height ? chi.SizeCharacter.Height : size.Height);
                    });

            //
            // create bitmaps per characater
            //
            Characters.ToList().ForEach(c => c.GenerateOriginal(largestBitmap));

            //
            // iterate through all bitmaps and find the tightest common border. only perform
            // this if the configuration specifies
            //

            // this will contain the values of the tightest border around the characters
            Border tightestCommonBorder = Border.Empty;

            // only perform if padding type specifies
            if (OutConfig.paddingRemovalHorizontal == OutputConfiguration.PaddingRemoval.Fixed ||
                OutConfig.paddingRemovalVertical == OutputConfiguration.PaddingRemoval.Fixed)
            {
                // find the common tightest border
                tightestCommonBorder = Characters.ToList().
                Select<CharacterDescriptor, Border>(c => c.OriginalBorder)
                .Aggregate<Border, Border>(Border.Empty,
                    (p1, p2) =>
                        new Border(
                                p1.Left < p2.Left ? p1.Left : p2.Left,
                                p1.Top < p2.Top ? p1.Top : p2.Top,
                                p1.Right > p2.Right ? p1.Right : p2.Right,
                                p1.Bottom > p2.Bottom ? p1.Bottom : p2.Bottom)
                            );
            }

            //
            // iterate thruogh all bitmaps and generate the bitmap we will convert to string
            // this means performing all manipulation (pad remove, flip)
            //

            // iterate over characters
            Characters.ToList().ForEach(c =>
            {
                // check if bitmap exists
                if (c.GenerateManipulatetBitmap(tightestCommonBorder))
                {
                    // create the page array for the character
                    c.GeneratePageArray();
                }
            });

            // populate font info
            populateFontInfoFromCharacters();

            GenerateStringsFromFontInfo();
        }
        
        // generate the strings
        private void GenerateStringsFromFontInfo()
        {
            StringBuilder sourceText = new StringBuilder();
            StringBuilder headerText = new StringBuilder();

            if (OutConfig.addCommentVariableName)
            {
                // add source file header
                sourceText.AppendFormat("{0}" + OutConfig.nl + "{1} Font data for {2}" + OutConfig.nl + "{3}" + OutConfig.nl + OutConfig.nl,
                                                    OutConfig.CommentStart,
                                                    OutConfig.CommentBlockMiddle,
                                                    getFontName(Font, false),
                                                    OutConfig.CommentBlockEnd);

                // add source header
                sourceText.AppendFormat("{0}Character bitmaps for {1} {2}" + OutConfig.nl,
                                                    OutConfig.CommentStart,
                                                    getFontName(Font, false),
                                                    OutConfig.CommentEnd);

                // add header file header
                headerText.AppendFormat("{0}Font data for {1} {2}" + OutConfig.nl,
                                                    OutConfig.CommentStart,
                                                    getFontName(Font, false),
                                                    OutConfig.CommentEnd);    
            }

            // get bitmap name
            string charBitmapVarName = String.Format(OutConfig.varNfBitmaps, getFontName(Font, true)) + "[]";

            // source var
            sourceText.AppendFormat("{0} = " + OutConfig.nl + "{{" + OutConfig.nl, charBitmapVarName);

            Characters.ToList().ForEach(chi => chi.GenerateCharacterDataDescriptorAndVisulazer());
            Characters.Aggregate(sourceText, (sb, chi) =>
            {
                // skip empty bitmaps
                if (chi.BitmapToGenerate == null) return sb;

                // now add letter array
                sourceText.Append(chi.Descriptor);
                // space out
                if (OutConfig.addCommentCharDescriptor && chi.Character != chi.ParentFontInfo.LastChar)
                {
                    // space between chars
                    sb.Append(OutConfig.nl);
                }

                return sb;
            });

            // space out
            sourceText.Append("};" + OutConfig.nl + OutConfig.nl);

            //
            // Charater descriptor
            //
            // whether or not block lookup was generated
            bool blockLookupGenerated = false;

            // check if required by configuration
            if (OutConfig.generateLookupArray)
            {
                // generate the lookup array
                blockLookupGenerated = generateStringsFromCharacterDescriptorBlockList(sourceText, headerText);
            }
            //
            // Font descriptor
            //

            // according to config
            if (OutConfig.addCommentVariableName)
            {
                // result string
                sourceText.AppendFormat("{0}Font information for {1} {2}" + OutConfig.nl,
                                                    OutConfig.CommentStart,
                                                    getFontName(Font, false),
                                                    OutConfig.CommentEnd);
            }

            // character name
            string fontInfoVarName = String.Format(OutConfig.varNfFontInfo, getFontName(Font, true));

            // add character array for header
            headerText.AppendFormat("extern {0};" + OutConfig.nl, fontInfoVarName);

            // the font character height
            string fontCharHeightString = "";

            // get character height sstring - displayed according to output configuration
            if (OutConfig.descFontHeight != OutputConfiguration.DescriptorFormat.DontDisplay)
            {
                // convert the value
                fontCharHeightString = String.Format("\t{0}, {1} Character height{2}" + OutConfig.nl,
                                              OutConfig.descFontHeight.ConvertValueByDescriptorFormat(FixedAbsolutCharHeight),
                                              OutConfig.CommentStart,
                                              OutConfig.CommentEnd);
            }

            string fontCodePage = "";
            if (OutConfig.addCodePage)
            {
                fontCodePage = string.Format("\t{0}, {1} CodePage {3}{2}" + OutConfig.nl,
                    OutConfig.CodePage,
                    OutConfig.CommentStart,
                    OutConfig.CommentEnd,
                    CodePageInfo.GetCodepageName(OutConfig.CodePage));
            }

            string spaceCharacterPixelWidthString = "";
            // get space char width, if it is up to driver to generate
            if (!OutConfig.generateSpaceCharacterBitmap)
            {
                // convert the value
                spaceCharacterPixelWidthString = String.Format("\t{0}, {1} Width, in pixels, of space character{2}" + OutConfig.nl,
                                                                OutConfig.spaceGenerationPixels,
                                                                OutConfig.CommentStart,
                                                                OutConfig.CommentEnd);
            }

            // font info
            sourceText.AppendFormat("{2} =" + OutConfig.nl + "{{" + OutConfig.nl +
                                              "{3}" +
                                              "\t{4}, {0} First character '{9}'{1}" + OutConfig.nl +
                                              "\t{5}, {0} Last character '{10}'{1}" + OutConfig.nl +
                                              "{6}" +
                                              "{7}" +
                                              "\t{8}, {0} Character bitmap array{1}" + OutConfig.nl +
                                              "{11}" +
                                              "}};" + OutConfig.nl,
                                               OutConfig.CommentStart,
                                              OutConfig.CommentEnd,
                                              fontInfoVarName,
                                              fontCharHeightString,
                                              getCharacterDisplayString(CodePageInfo, FirstChar),
                                              getCharacterDisplayString(CodePageInfo, LastChar),
                                              spaceCharacterPixelWidthString,
                                              getFontInfoDescriptorsString(blockLookupGenerated, OutConfig, Font),
                                              String.Format(OutConfig.varNfBitmaps, getFontName(Font, true)).GetVariableNameFromExpression(),
                                              FirstChar,
                                              LastChar,
                                              fontCodePage);

            // add the appropriate entity to the header
            if (blockLookupGenerated)
            {
                // add block lookup to header
                headerText.AppendFormat("extern const FONT_CHAR_INFO_LOOKUP {0}[];" + OutConfig.nl, getCharacterDescriptorArrayLookupDisplayString(Font));
            }
            else
            {
                // add block lookup to header
                headerText.AppendFormat("extern {0}[];" + OutConfig.nl, String.Format(OutConfig.varNfCharInfo, getFontName(Font, true)));
            }

            TextSource = sourceText.ToString();
            TextHeader = headerText.ToString();
        }

        // get font info from string
        private void populateFontInfoFromCharacters()
        {
            // do nothing if no chars defined
            if (Characters.Length == 0) return;

            // total offset
            int charByteOffset = 0;

            // set start char
            FirstChar = CodePageInfo.GetLastValidCharacter();
            LastChar = CodePageInfo.GetFirstValidCharacter();

            // the fixed absolute character height
            this.FixedAbsolutCharHeight = OutConfig.rotation.getAbsoluteCharacterDimensions(Characters[0].BitmapToGenerate.Size).Height;

            // iterate through letter string
            for (int charIdx = 0; charIdx < Characters.Length; ++charIdx)
            {
                // skip empty bitmaps
                if (Characters[charIdx].BitmapToGenerate == null) continue;

                // get char
                char currentChar = Characters[charIdx].Character;

                // is this character smaller than start char?
                if (CodePageInfo.GetCharacterDifferance(currentChar, FirstChar) > 0) FirstChar = currentChar;

                // is this character bigger than end char?
                if (CodePageInfo.GetCharacterDifferance(currentChar, LastChar) < 0) LastChar = currentChar;

                // populate offset of character
                Characters[charIdx].OffsetInBytes = charByteOffset;

                // increment byte offset
                charByteOffset += Characters[charIdx].DataLength;
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
                descriptorString += String.Format("\t{0}, {1} Character block lookup{2}" + outConfig.nl,
                                                    blockLookupGenerated ? getCharacterDescriptorArrayLookupDisplayString(font) : "NULL",
                                                    outConfig.CommentStart, outConfig.CommentEnd);

                // add to string
                descriptorString += String.Format("\t{0}, {1} Character descriptor array{2}" + outConfig.nl,
                                                    blockLookupGenerated ? "NULL" : String.Format(outConfig.varNfCharInfo, getFontName(font, true)).GetVariableNameFromExpression(),
                                                    outConfig.CommentStart, outConfig.CommentEnd);
            }
            else
            {
                // add descriptor array
                descriptorString += String.Format("\t{0}, {1} Character descriptor array{2}" + outConfig.nl,
                                                    String.Format(outConfig.varNfCharInfo, getFontName(font, true)).GetVariableNameFromExpression(),
                                                    outConfig.CommentStart, outConfig.CommentEnd);
            }

            // return the string
            return descriptorString;
        }

        // get the font name and format it
        public static string getFontName(Font font, bool variabelName = false)
        {
            string space = (variabelName) ? "_" : " ";
            string s;

            s = string.Format("{0}{2}{1}pt", font.Name, Math.Round(font.Size), space);

            if(font.Style != FontStyle.Regular)
            {
                s += space + font.Style.ToString();
                if(variabelName) s = s.Replace(", ", "_");
            }

            return (variabelName) ? s.ScrubVariableName() : s;
        }

        #region Lookup
        // genereate a list of blocks describing the characters
        private List<CharacterDescriptor>[] generateCharacterDescriptorBlockList()
        {
            char currentCharacter, previousCharacter = '\0';
            List<List<CharacterDescriptor>> characterBlockList = new List<List<CharacterDescriptor>>();
            // initialize first block
            List<CharacterDescriptor> characterBlock = null;
            CodePageInfo cpi = new CodePageInfo(OutConfig.CodePage);

            // get the difference between two characters required to create a new group
            int differenceBetweenCharsForNewGroup = OutConfig.generateLookupBlocks ?
                    OutConfig.lookupBlocksNewAfterCharCount : int.MaxValue;

            // iterate over characters, saving previous character each time
            for (int charIndex = 0; charIndex < Characters.Length; ++charIndex)
            {
                // get character
                currentCharacter = Characters[charIndex].Character;

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
                        characterBlock.Add(new CharacterDescriptor(this));
                    }
                    // fall through and add to current block
                }
                else
                {
                    // done with current block, add to list (null is for first character which hasn't
                    // created a group yet)
                    if (characterBlock != null) characterBlockList.Add(characterBlock);

                    // create new block
                    characterBlock = new List<CharacterDescriptor>();
                }

                // add to current block
                characterBlock.Add(Characters[charIndex]);

                // save previous char
                previousCharacter = currentCharacter;
            }

            // done; add current block to list
            characterBlockList.Add(characterBlock);

            return characterBlockList.ToArray();
        }

        // generate source/header strings from a block list
        private bool generateStringsFromCharacterDescriptorBlockList(StringBuilder textSource,
                                                                     StringBuilder textHeader)
        {
            // populate list of blocks
            List<CharacterDescriptor>[] characterBlockList = generateCharacterDescriptorBlockList();

            // get wheter there are multiple block lsits
            bool multipleDescBlocksExist = characterBlockList.Length > 1;

            //
            // Generate descriptor arrays
            //

            // iterate over blocks
            foreach (List<CharacterDescriptor> block in characterBlockList)
            {
                // according to config
                if (OutConfig.addCommentVariableName)
                {
                    string blockNumberString = String.Format("(block #{0})", Array.IndexOf(characterBlockList, block));

                    // result string
                    textSource.AppendFormat("{0}Character descriptors for {1}{2}{3}" + OutConfig.nl,
                                                        OutConfig.CommentStart,
                                                        getFontName(Font, false), 
                                                        multipleDescBlocksExist ? blockNumberString : "",
                                                        OutConfig.CommentEnd);

                    // describe character array
                    textSource.AppendFormat("{0}{{ {1}{2}[Offset into {3}CharBitmaps in bytes] }}{4}" + OutConfig.nl,
                                                        OutConfig.CommentStart,
                                                        getCharacterDescName("width", OutConfig.descCharWidth),
                                                        getCharacterDescName("height", OutConfig.descCharHeight),
                                                        getFontName(Font, true),
                                                        OutConfig.CommentEnd);
                }

                // output block header
                textSource.AppendFormat("{0} = " + OutConfig.nl + "{{" + OutConfig.nl, charDescArrayGetBlockName(Array.IndexOf(characterBlockList, block), true, multipleDescBlocksExist));

                // iterate characters
                block.Aggregate(textSource, (s, chi) => s.Append(chi.GetBlockInfo()));

                // terminate current block
                textSource.Append("};" + OutConfig.nl + OutConfig.nl);
            }

            //
            // Generate block lookup 
            //

            // if there is more than one block, we need to generate a block lookup
            if (multipleDescBlocksExist)
            {
                // start with comment, if required
                if (OutConfig.addCommentVariableName)
                {
                    // result string
                    textSource.AppendFormat("{0}Block lookup array for {1} {2}" + OutConfig.nl,
                                                        OutConfig.CommentStart,
                                                        getFontName(Font, false),
                                                        OutConfig.CommentEnd);

                    // describe character array
                    textSource.AppendFormat("{0}{{ start character, end character, ptr to descriptor block array }}{1}" + OutConfig.nl,
                                                        OutConfig.CommentStart,
                                                        OutConfig.CommentEnd);
                }

                // format the block lookup header
                textSource.AppendFormat("const FONT_CHAR_INFO_LOOKUP {0}[] = " + OutConfig.nl + "{{" + OutConfig.nl,
                                                    getCharacterDescriptorArrayLookupDisplayString(Font));

                // iterate
                foreach (List<CharacterDescriptor> block in characterBlockList)
                {
                    // get first/last chars
                    CharacterDescriptor firstChar = block.First();
                    CharacterDescriptor lastChar = block.Last();

                    // create current block description
                    textSource.AppendFormat("\t{{{0}, {1}, &{2}}}," + OutConfig.nl,
                                                                getCharacterDisplayString(CodePageInfo, firstChar.Character),
                                                                getCharacterDisplayString(CodePageInfo, lastChar.Character),
                                                                charDescArrayGetBlockName(Array.IndexOf(characterBlockList, block), false, true));
                }

                // terminate block lookup
                textSource.Append("};" + OutConfig.nl + OutConfig.nl);
            }

            return multipleDescBlocksExist;
        }

        // get character descriptor array block name
        private string charDescArrayGetBlockName(int currentBlockIndex,
                                                 bool includeTypeDefinition,
                                                 bool includeBlockIndex)
        {
            // get block id
            string blockIdString = String.Format("Block{0}", currentBlockIndex);

            // variable name
            string variableName = String.Format(OutConfig.varNfCharInfo, getFontName(Font, true));

            // remove type unless required
            if (!includeTypeDefinition) variableName = variableName.GetVariableNameFromExpression();

            // return the block name
            return String.Format("{0}{1}{2}",
                                    variableName,
                                    includeBlockIndex ? blockIdString : "",
                                    includeTypeDefinition ? "[]" : "");
        }

        // get the character descriptor string
        private static string getCharacterDescName(string name, OutputConfiguration.DescriptorFormat descFormat)
        {
            // create result string
            const string format = "[Char {0} in {1}], ";
            switch (descFormat)
            {
                case OutputConfiguration.DescriptorFormat.DontDisplay:
                    // don't display
                    return "";
                case OutputConfiguration.DescriptorFormat.DisplayInBits:
                    // set value
                    return String.Format(format, name, "bits");
                case OutputConfiguration.DescriptorFormat.DisplayInBytes:
                    // set value
                    return String.Format(format, name, "bytes");
                default:
                    throw new NotImplementedException();
            }
        }

        private static string getCharacterDescriptorArrayLookupDisplayString(Font font)
        {
            // return the string
            return String.Format("{0}BlockLookup", getFontName(font, true));
        }

        // get the display string for a character 
        private static string getCharacterDisplayString(CodePageInfo codePage, char character)
        {
            // return string
            return codePage.GetOffsetFromCharacter(character).ToString();
        }
        #endregion
    }
}
