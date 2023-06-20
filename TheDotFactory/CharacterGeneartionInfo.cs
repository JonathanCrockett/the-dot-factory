using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TheDotFactory
{
    // character generation information
    class CharacterDescriptor
    {
        private BitmapInfo bitmapInfo;

        public readonly FontDescriptor ParentFontInfo;
        public readonly char Character;
        public readonly Size SizeCharacter;
        public int OffsetInBytes { get; set; }          // offset into total array        

        private OutputConfiguration OutConfig { get { return ParentFontInfo.OutConfig; } }
        public Border OriginalBorder { get { return bitmapInfo.OriginalBorder; } }
        public Bitmap BitmapToGenerate { get { return bitmapInfo.BitmapToGenerate; } }    // the bitmap to generate into a string (flipped, trimmed - if applicable)
        public int DataLength{ get { return bitmapInfo.PagesLength; } }               // value of pages (vertical 8 bits), in serial order from top of bitmap
        public string Descriptor { get; private set; } // holding the datadescriptor string with visualizer
       
        public CharacterDescriptor(FontDescriptor parentFontInfo ) : this ( parentFontInfo, '\0') { }

        public CharacterDescriptor(FontDescriptor parentFontInfo, char character)
        {
            ParentFontInfo = parentFontInfo;
            Character = character;
            SizeCharacter = TextRenderer.MeasureText(Character.ToString(), ParentFontInfo.Font);
        }

        public void GenerateOriginal(Size size)
        {
            // create bitmap, sized to the correct size
            Bitmap Original = new Bitmap(size.Width, size.Height);

            Dictionary<Color, bool> color = new Dictionary<Color, bool>();
            color.Add(Color.FromArgb(255, Color.White), true);
            color.Add(Color.FromArgb(255, Color.Black), false);

            // create grahpics entity for drawing
            Graphics gfx = Graphics.FromImage(Original);

            // disable anti alias
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            // draw centered text
            Rectangle bitmapRect = new Rectangle(new Point(), size);

            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;

            // draw the character
            gfx.FillRectangle(Brushes.White, bitmapRect);
            gfx.DrawString(Character.ToString(), ParentFontInfo.Font, Brushes.Black, bitmapRect, drawFormat);

            bitmapInfo = new BitmapInfo(OutConfig, Original, color);
        }

        // generate the bitmap we will then use to convert to string (remove pad, flip)
        public bool GenerateManipulatetBitmap(Border tightestCommonBorder)
        {
            return bitmapInfo.GenerateManipulatetBitmap(tightestCommonBorder);
        }
        // create the page array
        public void GeneratePageArray()
        {
            bitmapInfo.GeneratePageArray();
        }
        // generate string from character info
        public void GenerateCharacterDataDescriptorAndVisulazer()
        {
            Descriptor = "";

            // according to config
            if (OutConfig.addCommentCharDescriptor)
            {
                // output character header
                Descriptor += GetCommentCharDescriptorHeader();
            }

            bitmapInfo.GenerateCharacterDataDescriptorAndVisulazer();
            Descriptor += bitmapInfo.Descriptor;
        }
        
        public override string ToString()
        {
            return Character.ToString();
        }

        public string GetCommentCharDescriptorHeader()
        {
            return string.Format("\t{0}@{1} '{2}' ({3} pixels wide){4}" + ParentFontInfo.OutConfig.nl,
                                                        ParentFontInfo.OutConfig.CommentStart,
                                                        OffsetInBytes,
                                                        Character,
                                                        bitmapInfo.Size.Width,
                                                        ParentFontInfo.OutConfig.CommentEnd);
        }

        public string GetBlockInfo()
        {
            Size s = bitmapInfo?.Size ?? new Size();

            // get the character descriptor string
            Func<OutputConfiguration.DescriptorFormat, int, string> GetCharacterDescString = ((descFormat, valueInBits) =>
             {
                 switch (descFormat)
                 {
                        // don't display
                        case OutputConfiguration.DescriptorFormat.DontDisplay:
                         return "";
                     case OutputConfiguration.DescriptorFormat.DisplayInBits:
                     case OutputConfiguration.DescriptorFormat.DisplayInBytes:
                            // add comma and return
                            return MyExtensions.ConvertValueByDescriptorFormat(descFormat, valueInBits) + ", ";
                     default:
                         throw new NotImplementedException();
                 }
             });

            return string.Format("\t{{{0}{1}{2}}},", GetCharacterDescString(OutConfig.descCharWidth , s.Width),
                                GetCharacterDescString(OutConfig.descCharHeight, s.Height),
                                OffsetInBytes)
                                .PadRight(15) +

             string.Format("{0}{1}{2}" + OutConfig.nl,                                
                                OutConfig.CommentStart,
                                Character == '\\' ? "\\ (backslash)" : Character.ToString(),
                                OutConfig.CommentEnd);
        }
    }
}
