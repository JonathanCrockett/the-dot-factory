using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheDotFactory
{
    public class CodePageInfo
    {
        public readonly Encoding encoding;
        public int CodePage { get { return encoding.CodePage;  } }

        public CodePageInfo(int codepage)
        {
            encoding = GetEncoding(codepage
        }

        public CodePageInfo(string codepage) : this ( GetCodepage(codepage))
        {

        }

        private static byte[] Range(byte min, byte max)
        {
            byte[] array = new byte[max - min];
            for (int i = 0; i < array.Length; i++)
                array[i] = (byte)(min + i);
            return array;
        }

        private static char[] Normalize(char[] s)
        {
            return s.Select(r => { return r < ' ' ? ' ' : r; }).Distinct().ToArray();
        }

        #region encoder decoder
        #region codepage arrays
        private static readonly char[] codePage437 =
        {
            '\0','☺','☻','♥','♦','♣','♠','•','◘','○','◙','♂','♀','♪','♫','☼',
            '►','◄','↕','‼','¶','§','▬','↨','↑','↓','→','←','∟','↔','▲','▼',
            ' ','!','"','#','$','%','&','\'','(',')','*','+',',','-','.','/',
            '0','1','2','3','4','5','6','7','8','9',':',';','<','=','>','?',
            '@','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
            'P','Q','R','S','T','U','V','W','X','Y','Z','[','\\',']','^','_',
            '`','a','b','c','d','e','f','g','h','i','j','k','l','m','n','o',
            'p','q','r','s','t','u','v','w','x','y','z','{','|','}','~','⌂',
            'Ç','ü','é','â','ä','à','å','ç','ê','ë','è','ï','î','ì','Ä','Å',
            'É','æ','Æ','ô','ö','ò','û','ù','ÿ','Ö','Ü','¢','£','¥','\u20A7','ƒ',
            'á','í','ó','ú','ñ','Ñ','ª','º','¿','⌐','¬','½','¼','¡','«','»',

            '░','▒','▓','│','┤','╡','╢','╖','╕','╣','║','╗','╝','╜','╛','┐',
            '└','┴','┬','├','─','┼','╞','╟','╚','╔','╩','╦','╠','═','╬','╧',
            '╨','╤','╥','╙','╘','╒','╓','╫','╪','┘','┌','█','▄','▌','▐','▀',
            'α','ß','Γ','π','Σ','σ','µ','τ','Φ','Θ','Ω','δ','∞','φ','ε','∩',
            '≡','±','≥','≤','⌠','⌡','÷','≈','°','∙','·','√','ⁿ','²','■',' '/*NBSP*/
        };

        public static char[] CodePage437
        {
            get { return codePage437; }
        }
        public static char[] CodePage850
        {
            get
            {
                char[] temp = CodePage437;

                temp[0x9B] = 'ø'; temp[0x9D] = 'Ø'; temp[0x9E] = '×';
                temp[0xA9] = '®';
                temp[0xB5] = 'Á'; temp[0xB6] = 'Â'; temp[0xB7] = 'À';
                temp[0xB8] = '©'; temp[0xBD] = '¢'; temp[0xBE] = '¥';
                temp[0xC6] = 'ã'; temp[0xC7] = 'Ã';
                temp[0xCF] = '¤';
                temp[0xD0] = 'ð'; temp[0xD1] = 'Ð'; temp[0xD2] = 'Ê'; temp[0xD3] = 'Ë'; temp[0xD4] = 'È';
                temp[0xD5] = 'ı'; temp[0xD6] = 'Í'; temp[0xD7] = 'Î';
                temp[0xD8] = 'Ï'; temp[0xDD] = '¦'; temp[0xDE] = 'Ì';
                temp[0xE0] = 'Ó'; temp[0xE2] = 'Ô'; temp[0xE3] = 'Ò'; temp[0xE4] = 'õ';
                temp[0xE5] = 'Õ'; temp[0xE7] = 'þ';
                temp[0xE8] = 'Þ'; temp[0xE9] = 'Ú'; temp[0xEA] = 'Û'; temp[0xEB] = 'Ù';
                temp[0xEC] = 'ý'; temp[0xED] = 'Ý'; temp[0xEE] = '¯'; temp[0xEF] = '´';
                temp[0xF0] = '­'/*SHY*/; temp[0xF2] = '‗'; temp[0xF3] = '¾'; temp[0xF4] = '¶';
                temp[0xF5] = '§'; temp[0xF7] = '¸';
                temp[0xF9] = '¨'; temp[0xFB] = '¹'; temp[0xFC] = '³';

                return temp;
            }
        }
        public static char[] CodePage852
        {
            get
            {
                char[] temp = CodePage850;

                temp[0x85] = 'ů'; temp[0x86] = 'ć';
                temp[0x88] = 'ł'; temp[0x8A] = 'Ő'; temp[0x8B] = 'ő'; temp[0x8D] = 'Ź'; temp[0x8F] = 'Ć';
                temp[0x91] = 'Ĺ'; temp[0x92] = 'ĺ'; temp[0x95] = 'Ľ'; temp[0x96] = 'ľ'; temp[0x97] = 'Ś';
                temp[0x98] = 'ś'; temp[0x9B] = 'Ť'; temp[0x9C] = 'ť'; temp[0x96] = 'Ł'; temp[0x9F] = 'č';
                temp[0xA4] = 'Ą'; temp[0xA5] = 'ą'; temp[0xA6] = 'Ž'; temp[0xA7] = 'ž';
                temp[0xA8] = 'Ę'; temp[0xA9] = 'ę'; temp[0xAB] = 'ź'; temp[0xAC] = 'Č'; temp[0xAD] = 'ş';
                temp[0xb7] = 'Ě';
                temp[0xA8] = 'Ş'; temp[0xBD] = 'Ż'; temp[0xBE] = 'ż';
                temp[0xC6] = 'Ă'; temp[0xC7] = 'ă';
                temp[0xD2] = 'Ď'; temp[0xD4] = 'ď'; temp[0xD4] = 'Ň';
                temp[0xD8] = 'ě'; temp[0xDD] = 'Ţ'; temp[0xDE] = 'Ů';
                temp[0xE3] = 'Ń'; temp[0xE4] = 'ń'; temp[0xE5] = 'ň'; temp[0xE6] = 'Š'; temp[0xE7] = 'š';
                temp[0xE8] = 'Ŕ'; temp[0xEA] = 'ŕ'; temp[0xE8] = 'Ű'; temp[0xE8] = 'ţ';
                temp[0xF1] = '˝'; temp[0xF2] = '˛'; temp[0xF3] = 'ˇ'; temp[0xF4] = '˘';
                temp[0xFA] = '˙'; temp[0xFB] = 'ű'; temp[0xFC] = 'Ř'; temp[0xFD] = 'ř';

                return temp;
            }
        }
        public static char[] CodePage858
        {
            get
            {
                char[] temp = CodePage850;
                temp[0xd5] = '€';
                return temp;
            }
        }

        #endregion

        #region Encoding lists
        public static Encoding CP437Encoding = new CodePageEncoding(437, CodePage437);
        public static Encoding CP850Encoding = new CodePageEncoding(850, CodePage850);
        public static Encoding CP852Encoding = new CodePageEncoding(852, CodePage852);
        public static Encoding CP858Encoding = new CodePageEncoding(858, CodePage858);
        public static Encoding Windows1252Encoding = Encoding.GetEncoding(1252);
        public static Encoding ISO8859_1Encoding = Encoding.GetEncoding(28591);

        public static Encoding[] EncodingList = {
            Encoding.ASCII,
            //Encoding.UTF7,
            //Encoding.UTF8,
            Encoding.Unicode,
            //Encoding.UTF32,
            //Encoding.BigEndianUnicode,
            Windows1252Encoding,
            ISO8859_1Encoding,
            CP437Encoding,
            CP850Encoding,
            CP852Encoding,
            CP858Encoding};
        #endregion

        public static Encoding GetEncoding(int codePage)
        {
            return EncodingList.ToList().Find(p => p.CodePage == codePage);
        }

        private class CodePageEncoding : Encoding
        {
            private int _codePage;
            public override int CodePage { get { return _codePage; } }

            public override bool IsSingleByte { get {  return true; } }

            public override string HeaderName
            {
                get
                {
                    return Encoding.GetEncoding(CodePage).HeaderName;
                }
            }

            public override string BodyName
            {
                get
                {
                    return Encoding.GetEncoding(CodePage).BodyName;
                }
            }

            public override string EncodingName
            {
                get
                {
                    return Encoding.GetEncoding(CodePage).EncodingName;
                }
            }

            public override string WebName
            {
                get
                {
                    return Encoding.GetEncoding(CodePage).WebName;
                }
            }

            public override bool IsBrowserDisplay { get { return false; } }

            public override bool IsBrowserSave { get { return false; } }

            readonly Encoder Encoder;
            readonly Decoder Decoder;

            public CodePageEncoding(int codePage, char[] character)
            {
                _codePage = codePage;
                Encoder = new CodePageEncoder(character);
                Decoder = new CodePageDecoder(character);
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return count;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                return Decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                return Encoder.GetBytes(chars, charIndex, charCount, bytes, byteIndex, false);
            }

            public override int GetByteCount(char[] chars, int index, int count)
            {
                 return count;
            }

            public override int GetMaxByteCount(int charCount)
            {
                return charCount;
            }

            public override int GetMaxCharCount(int byteCount)
            {
                 return byteCount;
            }

            public override Decoder GetDecoder()
            {
                return Decoder;
            }

            public override Encoder GetEncoder()
            {
                return Encoder;
            }

            
        }

        private class CodePageEncoder : Encoder
        {
            private readonly byte[] codePageReverse;
            private readonly char[] codePage;

            public CodePageEncoder(char[] codePage)
            {
                this.codePage = (char[])codePage.Clone();
                List<char> distictCodepage = codePage.Distinct().ToList();
                codePageReverse = new byte[codePage.Max()].Select<byte, byte>((b, index) =>
                    {
                        if (distictCodepage.Contains((char)index))
                            return (byte)distictCodepage.IndexOf((char)index);
                        else
                            return 21;
                    }).ToArray();
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
            {
                int writenBytes = 0;
                for( ; charIndex < chars.Length && byteIndex < bytes.Length; charIndex++, byteIndex++, charCount--, writenBytes++)
                {
                    bytes[byteIndex] = codePageReverse[chars[charIndex]];
                }
                return writenBytes;
            }

            public override int GetByteCount(char[] chars, int index, int count, bool flush)
            {
                return count;
            }
        }

        private class CodePageDecoder : Decoder
        {
            private readonly char[] codePage;

            public CodePageDecoder(char[] codePage)
            {
                this.codePage = (char[])codePage.Clone();
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return count;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                int writenChars = 0;
                for (; byteIndex < bytes.Length && charIndex < chars.Length; byteIndex++, charIndex++, byteCount--, writenChars++)
                {
                    if (bytes[byteIndex] < codePage.Length)
                        chars[charIndex] = codePage[bytes[byteIndex]];
                    else
                        chars[charIndex] = (char)21;
                }
                return writenChars;
            }
        }
        #endregion

        public char GetCharacterFromOffset(int offest)
        {
            if (encoding.IsSingleByte)
            {
                return encoding.GetChars(new byte[] { (byte)(offest & 0xff) })[0];
            }
            else
            {
                return encoding.GetChars(new byte[] { (byte)(offest >> 8), (byte)(offest & 0xff) })[0];
            }
        }

        public int GetOffsetFromCharacter(char c)
        {
            byte[] a;
            int offset;

            a = encoding.GetBytes(new char[] { c });
            offset = a[0];
            if (a.Length >= 2) offset += a[1] << 8;

            return offset;
        }

        public char[] GetAllValidCharacter()
        {
            int max = 255;
            List<char> list;

            if (encoding.CodePage == 20127)
                max = 127;
            else if (encoding.CodePage == 1200) // utf-16
                max = 65535;

            list = new List<char>(max);

            switch (encoding.CodePage)
            {
                case 437: list.AddRange(codePage437); break;
                case 850: list.AddRange(CodePage850); break;
                case 852: list.AddRange(CodePage852); break;
                case 858: list.AddRange(CodePage858); break;
                case 1200:
                    int i = 0;
                    while (i < max)
                    {
                        if (i < 0xD800 || i > 0xDFFF) list.Add((char)i);
                        i++;
                    }
                    break;
                default:
                    list.AddRange(encoding.GetChars(Range(0, (byte)max)));
                    break;
            }

            return Normalize(list.ToArray());
        }


        public char GetLastValidCharacter()
        {
            return GetAllValidCharacter().Last();
        }

        public char GetFirstValidCharacter()
        {
            return GetAllValidCharacter()[0];
        }


        public int GetCharacterDifferance(char a, char b)
        {
            int offset_a, offset_b;

            offset_a = GetOffsetFromCharacter(a);

            offset_b = GetOffsetFromCharacter(b);

            return offset_b - offset_a;
        }


        public static string[] GetEncoderNameList()
        {
            return EncodingList.Select(e => GetCodepageName(e.CodePage)).ToArray();
        }
        
        public static string GetCodepageName(int codepage)
        {
            return Encoding.GetEncoding(codepage).HeaderName;
        }

        public static int GetCodepage(string codepage)
        {
            return Encoding.GetEncoding(codepage).CodePage;
        }
    }
}
