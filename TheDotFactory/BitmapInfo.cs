using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TheDotFactory
{
    class BitmapInfo
    {
        private Bitmap Original;                                // the original bitmap
        byte[] Pages;                                           // value of pages (vertical 8 bits), in serial order from top of bitmap
        public Border OriginalBorder { get; private set; }
        public Bitmap BitmapToGenerate { get; private set; }    // the bitmap to generate into a string (flipped, trimmed - if applicable)
        
        public int PagesLength { get { return Pages.Length; } }
        public string[] Data { get; private set; }              // holding the datatabel of the character
        public string[] Visualizer { get; private set; }        // holding the visualizer string
        public string Descriptor { get; private set; }
        public Dictionary<Color, bool> ColorList { get; private set; }
        public Size Size { get; private set; }                  // size of the bitmap to generate 
        public OutputConfiguration OutConfig { get; private set; }

        public BitmapInfo(OutputConfiguration outConfig, Bitmap bmp, Dictionary<Color, bool> colorList)
        {
            OutConfig = outConfig;
            Original = bmp;
            OriginalBorder = Original.GetBorders(colorList.GetEnabledKeys<Color>());
            ColorList = colorList;
        }

        public BitmapInfo(OutputConfiguration outConfig, Bitmap bmp, Color borderColor)
        {
            OutConfig = outConfig;
            Original = bmp;
            OriginalBorder = Original.GetBorders(borderColor);
            ColorList = Original.GetColorList().Aggregate (new Dictionary<Color, bool>(), (dic, c) => { dic.Add(c, c.ToArgb() == borderColor.ToArgb()); return dic; } );

        }

        public bool GenerateManipulatetBitmap(Border tightestCommonBorder)
        {
            //
            // First, crop
            //
            Size minSize = new Size(OutConfig.spaceGenerationPixels, Original.Height);

            // get bitmap border - this sets the crop rectangle to per bitmap, essentially
            Border bitmapCropBorder = OriginalBorder;

            if (!bitmapCropBorder.IsValid() && minSize.Width == 0 && minSize.Height == 0)
            {
                // no data
                BitmapToGenerate = null;
                Size = new Size();
                // bitmap contains no data
                return false;
            }


            // check that width exceeds minimum
            if (bitmapCropBorder.Right - bitmapCropBorder.Left < 0)
            {
                // replace
                bitmapCropBorder.Left = 0;
                bitmapCropBorder.Right = minSize.Width - 1;
            }

            // check that height exceeds minimum
            if (bitmapCropBorder.Bottom - bitmapCropBorder.Top < 0)
            {
                // replace
                bitmapCropBorder.Top = 0;
                bitmapCropBorder.Bottom = minSize.Height - 1;
            }

            // should we crop hotizontally according to common
            switch(OutConfig.paddingRemovalHorizontal)
            { 
               case OutputConfiguration.PaddingRemoval.Fixed:
                    // cropped Y is according to common
                    bitmapCropBorder.Top = tightestCommonBorder.Top;
                    bitmapCropBorder.Bottom = tightestCommonBorder.Bottom;
                    break;
                // check if no horizontal crop is required
                case OutputConfiguration.PaddingRemoval.None:
                    // set y to actual max border of bitmap
                    bitmapCropBorder.Top = 0;
                    bitmapCropBorder.Bottom = Original.Height - 1;
                    break;
                case OutputConfiguration.PaddingRemoval.Tighest:
                    break;
                default: throw new NotImplementedException();
            }

            // should we crop vertically according to common
            switch(OutConfig.paddingRemovalVertical)
            {
                case OutputConfiguration.PaddingRemoval.Fixed:
                    // cropped X is according to common
                    bitmapCropBorder.Left = tightestCommonBorder.Left;
                    bitmapCropBorder.Right = tightestCommonBorder.Right;
                    break;
                // check if no vertical crop is required
                case OutputConfiguration.PaddingRemoval.None:
                    // set x to actual max border of bitmap
                    bitmapCropBorder.Left = 0;
                    bitmapCropBorder.Right = Original.Width - 1;
                    break;
                case OutputConfiguration.PaddingRemoval.Tighest:
                    break;
                default: throw new NotImplementedException();
            }

            // now copy the output bitmap, cropped as required, to a temporary bitmap
            Rectangle rect = new Rectangle(bitmapCropBorder.Left,
                                            bitmapCropBorder.Top,
                                            bitmapCropBorder.Right - bitmapCropBorder.Left + 1,
                                            bitmapCropBorder.Bottom - bitmapCropBorder.Top + 1);

            // clone the cropped bitmap into the generated one
            BitmapToGenerate = Original.Clone(rect, Original.PixelFormat);

            // flip the cropped bitmap
            BitmapToGenerate.RotateFlip(OutConfig.RotationFlip);

            Size = OutConfig.rotation.getAbsoluteCharacterDimensions(BitmapToGenerate.Size);

            // bitmap contains data
            return true;
        }

        // create the page array
        public void GeneratePageArray()
        {
            int[] pixels = BitmapToGenerate.ToArgbArray();
            int width = BitmapToGenerate.Width, height = BitmapToGenerate.Height;
            int black = Color.Black.ToArgb(), white = Color.White.ToArgb();
            Dictionary<int, List<byte>> dpages = new Dictionary<int, List<byte>>();
            Dictionary<int, bool> backColorListInt = ColorList.ToDictionary<KeyValuePair<Color, bool>, int, bool>(kvp => kvp.Key.ToArgb(), kvp => kvp.Value);
            bool ColumnMajor = OutConfig.bitLayout == OutputConfiguration.BitLayout.ColumnMajor;

            // create pages
            Pages = new byte[0];

            Func<int, int, int> getPixel = delegate (int x, int y)
            {
                return pixels[y * width + x];
            };

            Action<int> ConvertRow = row =>
            {
                dpages.Add(row, new List<byte>());
                // current byte value
                byte currentValue = 0, bitsRead = 0;

                // for each column
                for (int column = 0; column < width; ++column)
                {
                    // is pixel set?
                    if (!backColorListInt[getPixel(column, row)])
                    {
                        // set the appropriate bit in the page
                        if (OutConfig.byteOrderMsbFirst) currentValue |= (byte)(1 << (7 - bitsRead));
                        else currentValue |= (byte)(1 << bitsRead);
                    }

                    // increment number of bits read
                    // have we filled a page?
                    if (++bitsRead == 8)
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
            for (int row = 0; row < height; row++) ConvertRow(row);

            List<byte> tempPages = new List<byte>();
            for (int i = 0; i < dpages.Count; i++)
            {
                tempPages.AddRange(dpages[i]);
            }
            Pages = tempPages.ToArray();

            // transpose the pages if column major data is requested
            if (ColumnMajor)
            {
                Pages = Transpose(Pages, BitmapToGenerate.Width, BitmapToGenerate.Height, OutConfig.byteOrderMsbFirst);
            }
        }

        // builds a string array of the data in 'pages'
        private void GenerateDataString()
        {
            int width = BitmapToGenerate.Width;
            int height = BitmapToGenerate.Height;
            int colCount = (OutConfig.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? (width + 7) / 8 : width;
            int rowCount = (OutConfig.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? height : (height + 7) / 8;

            Data = new string[rowCount];
            StringBuilder sb = new StringBuilder();

            Data = Data.Select((s, row) =>
            {
                sb.Clear();
                // iterator over columns
                for (int col = 0; col != colCount; ++col)
                {
                    // get the byte to output
                    int page = Pages[row * colCount + col];

                    // add leading character
                    sb.Append(OutConfig.byteLeadingString);

                    // check format
                    switch(OutConfig.byteFormat)
                    {
                        case OutputConfiguration.ByteFormat.Hex:
                            // convert byte to hex
                            sb.Append(page.ToString("X").PadLeft(2, '0'));
                            break;
                        case OutputConfiguration.ByteFormat.Binary:
                            // convert byte to binary
                            sb.Append(Convert.ToString(page, 2).PadLeft(8, '0'));
                            break;
                        default:
                            throw new NotFiniteNumberException();
                    }

                    // add comma
                    sb.Append(", ");
                }

                return sb.ToString();
            }).ToArray();
        }

        // builds a string array visualization of 'pages'
        private void GenerateVisualizerString()
        {
            // the number of pages per row in 'pages'
            int width = BitmapToGenerate.Width;
            int height = BitmapToGenerate.Height;
            int colCount = (OutConfig.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? (width + 7) / 8 : width;
            int rowCount = (OutConfig.bitLayout == OutputConfiguration.BitLayout.RowMajor) ? height : (height + 7) / 8;

            Visualizer = new string[height];

            if (OutConfig.addCommentCharVisualizer)
            {
                for (int row = 0; row != height; ++row)
                {
                    // each row is started with a line comment
                    Visualizer[row] = OutConfig.CommentStart;

                    // iterator over columns
                    for (int col = 0; col != width; ++col)
                    {
                        // get the byte containing the bit we want
                        byte page = (OutConfig.bitLayout == OutputConfiguration.BitLayout.RowMajor)
                        ? Pages[row * colCount + (col / 8)]
                        : Pages[(row / 8) * colCount + col];

                        // make a mask to extract the bit we want
                        byte bitMask = (OutConfig.bitLayout == OutputConfiguration.BitLayout.RowMajor)
                        ? GetBitMask(OutConfig.byteOrderMsbFirst, 7 - (col % 8))
                        : GetBitMask(OutConfig.byteOrderMsbFirst, row % 8);

                        // check if bit is set
                        Visualizer[row] += (bitMask & page) != 0 ? OutConfig.bmpVisualizerChar : OutConfig.bmpVisualizerCharEmpty;
                    }

                    Visualizer[row] += OutConfig.CommentEnd;
                }
            }
        }

        // generate string from character info
        public void GenerateCharacterDataDescriptorAndVisulazer()
        {
            // generate the data rows
            GenerateDataString();
            // generate the visualizer
            GenerateVisualizerString();

            // build the result string
            StringBuilder resultString = new StringBuilder();

            // output row major data
            switch (OutConfig.bitLayout)
            {
                case OutputConfiguration.BitLayout.RowMajor:
                    // the visualizer is drawn after the data on the same rows, so they must have the same length
                    System.Diagnostics.Debug.Assert(Data.Length == Visualizer.Length);

                    // output the data and visualizer together
                    switch (OutConfig.lineWrap)
                    {
                        case OutputConfiguration.LineWrap.AtColumn:
                            // one line per row
                            for (int row = 0; row != Data.Length; ++row)
                            {
                                resultString.Append("\t").Append(Data[row]).Append(Visualizer[row]).Append(OutConfig.nl);
                            }
                            break;
                        case OutputConfiguration.LineWrap.AtBitmap:
                            // one line per bitmap                    
                            if (OutConfig.addCommentCharVisualizer)
                            {
                                for (int row = 0; row != Visualizer.Length; ++row)
                                {
                                    resultString.Append("\t").Append(Visualizer[row]).Append(OutConfig.nl);
                                }
                            }
                            resultString.Append("\t");
                            for (int row = 0; row < Data.Length; ++row)
                            {
                                resultString.Append(Data[row]);
                            }
                            resultString.Append(OutConfig.nl);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                // output column major data
                case OutputConfiguration.BitLayout.ColumnMajor:
                    // output the visualizer
                    for (int row = 0; row < Visualizer.Length; ++row)
                    {
                        resultString.Append("\t").Append(Visualizer[row]).Append(OutConfig.nl);
                    }

                    // output the data
                    switch(OutConfig.lineWrap)
                    {
                        case OutputConfiguration.LineWrap.AtColumn:
                            // one line per row
                            for (int row = 0; row < Pages.Length; ++row)
                            {
                                resultString.Append("\t").Append(Pages[row]).Append(OutConfig.nl);
                            }
                            break;
                        case OutputConfiguration.LineWrap.AtBitmap:
                            // one line per bitmap
                            resultString.Append("\t");
                            for (int row = 0; row < Pages.Length; ++row)
                            {
                                resultString.Append(Pages[row]);
                            }
                            resultString.Append(OutConfig.nl);
                        break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            Descriptor = resultString.ToString();
        }

        // generate an array of column major pages from row major pages
        public static byte[] Transpose(byte[] rowMajorPages, int width, int height, bool MsbFirst)
        {
            // column major data has a byte for each column representing 8 rows
            int rowMajorPagesPerRow = (width + 7) / 8;
            int colMajorPagesPerRow = width;
            int colMajorRowCount = (height + 7) / 8;

            // create an array of pages filled with zeros for the column major data
            List<byte> tempPages = new List<byte>(colMajorPagesPerRow * colMajorRowCount);

            // generate the column major data
            for (int row = 0; row != height; ++row)
            {
                for (int col = 0; col != width; ++col)
                {
                    // get the byte containing the bit we want
                    int srcIdx = row * rowMajorPagesPerRow + (col / 8);
                    int page = rowMajorPages[srcIdx];

                    // get the bit mask for the bit we want
                    byte bitMask = GetBitMask(MsbFirst, 7 - (col % 8));

                    // set the bit in the column major data
                    if ((page & bitMask) != 0)
                    {
                        int dstIdx = (row / 8) * colMajorPagesPerRow + col;

                        tempPages[dstIdx] = (byte)(tempPages[dstIdx] | GetBitMask(MsbFirst, row % 8));
                    }
                }
            }
            return tempPages.ToArray();
        }

        // return a bitMask to pick out the 'bitIndex'th bit allowing for byteOrder
        // MsbFirst: bitIndex = 0 = 0x01, bitIndex = 7 = 0x80
        // LsbFirst: bitIndex = 0 = 0x80, bitIndex = 7 = 0x01
        public static byte GetBitMask(bool MsbFirst, int bitIndex)
        {
            return (byte)(MsbFirst
                ? 0x01 << bitIndex
                : 0x80 >> bitIndex);
        }
    }
}