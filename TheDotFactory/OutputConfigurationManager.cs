using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;

namespace TheDotFactory
{
    // an output configuration preset
    [Serializable]
    [XmlInclude(typeof(Rotation))]
    public class OutputConfiguration
    {
        [XmlAttribute("space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public const string Space = "preserve";

        #region string constanst
        public const string CommentStartCPP = "// ";
        public const string CommentBlockEndCPP = "// ";
        public const string CommentBlockMiddleCPP = "// ";
        public const string CommentEndCPP = "";

        public const string CommentStartC = "/* ";
        public const string CommentBlockEndC = "*/";
        public const string CommentBlockMiddleC = "** ";
        public const string CommentEndC = "*/";

        public const string CommentStartPython = "# ";
        public const string CommentBlockEndPython = "# ";
        public const string CommentBlockMiddlePython = "# ";
        public const string CommentEndPython = "";

        public readonly string nl = Environment.NewLine;
        #endregion

        #region enums constans
        // padding removal type
        public enum PaddingRemoval
        {
            None,               // no padding removal
            Tighest,            // remove padding as much as possible, per bitmap
            Fixed               // remove padding as much as the bitmap with least padding
        }

        // Line wrap
        public enum LineWrap
        {
            AtColumn,           // After each column
            AtBitmap            // After each bitmap
        }

        // Comment style
        public enum CommentStyle
        {
            C,                  // c style comments - /* */
            Cpp,                // C++ style - //
            Python,             // Python style - #
        }

        // Bit Layout
        public enum BitLayout
        {
            RowMajor,     // '|' = 0x80,0x80,0x80  '_' = 0x00,0x00,0xFF
            ColumnMajor,  // '|' = 0xFF,0x00,0x00  '_' = 0x80,0x80,0x80
        }

        // Byte format
        public enum ByteFormat:int
        {
            Binary,     // Binary
            Hex         // Hex
        }

        // rotation
        [Serializable]
        public sealed class Rotation
        {
            public static readonly Rotation Zero = new Rotation(rotationEnm.RotateZero);
            public static readonly Rotation Ninety = new Rotation(rotationEnm.RotateNinety);
            public static readonly Rotation OneEighty = new Rotation(rotationEnm.RotateOneEighty);
            public static readonly Rotation TwoSeventy = new Rotation(rotationEnm.RotateTwoSeventy);

            private rotationEnm value;

            private enum rotationEnm:byte
            {
                RotateZero,
                RotateNinety,
                RotateOneEighty,
                RotateTwoSeventy
            }

            public Rotation() { }

            private Rotation(rotationEnm r) { value = r; }

            // rotation display string
            private static readonly string[] RotationDisplayString =
            {
                "0°",
                "90°",
                "180°",
                "270°"
            };

            public override string ToString()
            {
                return RotationDisplayString[(byte)value];
            }

            public static Rotation Parse(String s)
            {
                int index = Array.IndexOf<string>(RotationDisplayString, s);
                if (index <= -1) throw new ArgumentException();
                else if (index > (int)rotationEnm.RotateTwoSeventy) throw new ArgumentOutOfRangeException();
                else return (Rotation)index;
            }

            public static string[] GetNames()
            {
                return RotationDisplayString;
            }

            public static explicit operator int(Rotation r)
            {
                return (int)r.value;
            }

            public static explicit operator Rotation(int value)
            {
                return new Rotation((rotationEnm)value);
            }

            // get rotate flip type according to config
            public static RotateFlipType GetRotateFlipType(Rotation rot, bool flipX, bool flipY)
            {
                switch (rot.value)
                {
                    case rotationEnm.RotateZero:
                        // return according to flip
                        if (!flipX && !flipY) return RotateFlipType.RotateNoneFlipNone;
                        else if (flipX && !flipY) return RotateFlipType.RotateNoneFlipX;
                        else if (!flipX && flipY) return RotateFlipType.RotateNoneFlipY;
                        else// if (flipX && flipY)
                        return RotateFlipType.RotateNoneFlipXY;
                    case rotationEnm.RotateNinety:
                        // return according to flip
                        if (!flipX && !flipY) return RotateFlipType.Rotate90FlipNone;
                        else if (flipX && !flipY) return RotateFlipType.Rotate90FlipX;
                        else if (!flipX && flipY) return RotateFlipType.Rotate90FlipY;
                        else// if (flipX && flipY) 
                            return RotateFlipType.Rotate90FlipXY;
                    case rotationEnm.RotateOneEighty:
                        // return according to flip
                        if (!flipX && !flipY) return RotateFlipType.Rotate180FlipNone;
                        else if (flipX && !flipY) return RotateFlipType.Rotate180FlipX;
                        else if (!flipX && flipY) return RotateFlipType.Rotate180FlipY;
                        else //if (flipX && flipY)
                            return RotateFlipType.Rotate180FlipXY;
                    case rotationEnm.RotateTwoSeventy:
                        // return according to flip
                        if (!flipX && !flipY) return RotateFlipType.Rotate270FlipNone;
                        else if (flipX && !flipY) return RotateFlipType.Rotate270FlipX;
                        else if (!flipX && flipY) return RotateFlipType.Rotate270FlipY;
                        else //if (flipX && flipY) 
                            return RotateFlipType.Rotate270FlipXY;
                    default:
                        throw new NotImplementedException();
                }
            }

            public RotateFlipType GetRotateFlipType(bool flipHorizontal, bool flipVertical) 
            {
                return GetRotateFlipType(this, flipHorizontal, flipVertical);
            }

            // get absolute height/width of characters according to rotation
            public Size getAbsoluteCharacterDimensions(Size size)
            {
                // get the absolute font character height. Depends on rotation
                if (value == rotationEnm.RotateZero ||
                    value == rotationEnm.RotateOneEighty)
                {
                    // if char is not rotated or rotated 180deg, its height is the actual height
                    return size;
                }
                else
                {
                    // if char is rotated by 90 or 270, its height is the width of the rotated bitmap
                    return new Size(size.Height, size.Width);
                }
            }
        }
        
        // rotation
        public enum DescriptorFormat
        {
            DontDisplay,
            DisplayInBits,
            DisplayInBytes
        }

        // rotation display string
        public static readonly string[] DescriptorFormatDisplayString = new string[]
        {
            "Don't display",
            "In bits",
            "In bytes"
        };

        // leading strings
        public const string ByteLeadingStringBinary = "0b";
        public const string ByteLeadingStringHex = "0x";

        public static readonly string[] CommentVisualizerChar = new string[]
            {
                "# ",
                "█ ",
                "O_",
            };

        #endregion

        // clone self
        public OutputConfiguration clone() { return (OutputConfiguration)this.MemberwiseClone(); }

        #region data
        // comments
        public bool addCommentVariableName = true;
        public bool addCommentCharVisualizer = true;
        public bool addCommentCharDescriptor = true;
        public CommentStyle commentStyle = CommentStyle.Cpp;
        public char bmpVisualizerChar = '#';
        public char bmpVisualizerCharEmpty = ' ';

        // rotation
        public Rotation rotation = Rotation.Zero;
        [XmlIgnore]
        public RotateFlipType RotationFlip
        {
            get
            {
                return rotation.GetRotateFlipType(flipHorizontal, flipVertical);
            }
        }

        // flip
        public bool flipHorizontal = false;
        public bool flipVertical = false;

        // padding removal
        public PaddingRemoval paddingRemovalHorizontal = PaddingRemoval.Fixed;
        public PaddingRemoval paddingRemovalVertical = PaddingRemoval.Tighest;

        // line wrap
        public LineWrap lineWrap = LineWrap.AtColumn;

        // byte
        public BitLayout bitLayout = BitLayout.RowMajor;
        public bool byteOrderMsbFirst = true;
        public ByteFormat byteFormat = ByteFormat.Hex;
        public string byteLeadingString = ByteLeadingStringHex;

        // descriptors
        public bool generateLookupArray = true;
        public DescriptorFormat descCharWidth = DescriptorFormat.DisplayInBits;
        public DescriptorFormat descCharHeight = DescriptorFormat.DontDisplay;
        public DescriptorFormat descFontHeight = DescriptorFormat.DisplayInBytes;
        public bool generateLookupBlocks = false;
        public int lookupBlocksNewAfterCharCount = 80;
        public DescriptorFormat descImgWidth = DescriptorFormat.DisplayInBytes;
        public DescriptorFormat descImgHeight = DescriptorFormat.DisplayInBits;
        public bool addCodePage = true;

        // space generation
        public bool generateSpaceCharacterBitmap = false;
        public int spaceGenerationPixels = 2;

        // variable formats
        public string varNfBitmaps = "const uint_8 {0}Bitmaps";
        public string varNfCharInfo = "const FONT_CHAR_INFO {0}Descriptors";
        public string varNfFontInfo = "const FONT_INFO {0}FontInfo";
        public string varNfImageBitmap = "const uint_8 {0}Bitmap";
        public string varNfImageInfo = "const IMAGE_INFO {0}ImageInfo";

        // display name
        public string displayName = "";

        //
        public int CodePage = 1200;             // UTF-16
        #endregion

        public static char CommentVisualizerCharDefault
        {
            get { return CommentVisualizerChar[0][0]; }
        }
        public static char CommendVisualizerCharEmptyDefault
        {
            get { return CommentVisualizerChar[0][1]; }
        }

        public string CommentStart
        {
            get
            {
                switch(commentStyle)
                {
                    case CommentStyle.C: return CommentStartC;
                    case CommentStyle.Cpp: return CommentStartCPP;
                    case CommentStyle.Python: return CommentStartPython;
                    default: throw new NotImplementedException();
                }
            }
        }
        public string CommentBlockEnd
        {
            get
            {
                switch (commentStyle)
                {
                    case CommentStyle.C: return CommentBlockEndC;
                    case CommentStyle.Cpp: return CommentBlockEndCPP;
                    case CommentStyle.Python: return CommentBlockEndPython;
                    default: throw new NotImplementedException();
                }
            }
        }
        public string CommentBlockMiddle
        {
            get
            {
                switch (commentStyle)
                {
                    case CommentStyle.C: return CommentBlockMiddleC;
                    case CommentStyle.Cpp: return CommentBlockMiddleCPP;
                    case CommentStyle.Python: return CommentBlockMiddlePython;
                    default: throw new NotImplementedException();
                }
            }
        }
        public string CommentEnd
        {
            get
            {
                switch (commentStyle)
                {
                    case CommentStyle.C: return CommentEndC;
                    case CommentStyle.Cpp: return CommentEndCPP;
                    case CommentStyle.Python: return CommentEndPython;
                    default: throw new NotImplementedException();
                }
            }
        }
    }

    // the output configuration manager
    public class OutputConfigurationManager
    {
        // add a configuration
        public int configurationAdd(ref OutputConfiguration configToAdd)
        {
            // add to list
            m_outputConfigurationList.Add(configToAdd);

            // return the index of the new item
            return m_outputConfigurationList.Count - 1;
        }

        // delete a configuration
        public void configurationDelete(int configIdxToRemove)
        {
            // check if in bounds
            if (configIdxToRemove >= 0 && configIdxToRemove < configurationCountGet())
            {
                // delete it
                m_outputConfigurationList.RemoveAt(configIdxToRemove);
            }
        }
        
        // get number of configurations
        public int configurationCountGet()
        {
            // get number of items
            return m_outputConfigurationList.Count;
        }

        // get configuration at index
        public OutputConfiguration configurationGetAtIndex(int index)
        {
            // return the configuration
            return m_outputConfigurationList[index];
        }

        // save to file
        public void saveToFile(string fileName)
        {
            // create serailizer and text writer
            XmlSerializer serializer = new XmlSerializer(m_outputConfigurationList.GetType());
            TextWriter textWriter = new StreamWriter(fileName);
            
            // serialize to xml
            serializer.Serialize(textWriter, m_outputConfigurationList);
            
            // close and flush the stream
            textWriter.Close();
        }

        // load from file
        public void loadFromFile(string fileName)
        {
            // create serailizer and text writer
            XmlSerializer serializer;
            try
            {
                serializer = new XmlSerializer(m_outputConfigurationList.GetType());
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                System.Diagnostics.Debugger.Break();
                throw;
            }


            // catch exceptions (especially file not found)
            try
            {
                // read text
                TextReader textReader = new StreamReader(fileName);

                // serialize to xml
                m_outputConfigurationList = (List<OutputConfiguration>)serializer.Deserialize(textReader);

                // close and flush the stream
                textReader.Close();
            }
            catch (IOException)  { }
            catch (InvalidOperationException) { }
            catch (Exception  exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        // populate the cbx
        public void comboBoxPopulate(ComboBox combobox)
        {
            // clear all items
            combobox.Items.Clear();

            // iterate through items
            foreach (OutputConfiguration oc in m_outputConfigurationList)
            {
                // get the name
                combobox.Items.Add(oc.displayName);
            }
        }

        // a working copy configuration, used for when there are no presets and 
        // during editing
        public OutputConfiguration workingOutputConfiguration = new OutputConfiguration();

        // the output configuration
        private List<OutputConfiguration> m_outputConfigurationList = new List<OutputConfiguration>();
    }
}
