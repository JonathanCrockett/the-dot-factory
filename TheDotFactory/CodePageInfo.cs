using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDotFactory
{
    public class CodePageInfo
    {
        private CodePageEncoding encoding;
        public int CodePage { get; private set; }

        public CodePageInfo(int codepage)
        {
            CodePage = codepage;
            encoding = new CodePageEncoding(codepage);            
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

        #region codepage arrays
        private readonly static char[] codePage437 = new char[]
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
            'É','æ','Æ','ô','ö','ò','û','ù','ÿ','Ö','Ü','¢','£','¥','§','ƒ',
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
                char[] result = new char[256];

                Array.Copy(CodePage437, 0, result, 0, 256);

                result[0x9B] = 'ø'; result[0x9D] = 'Ø'; result[0x9E] = '×';
                result[0xA9] = '®';
                result[0xB5] = 'Á'; result[0xB6] = 'Â'; result[0xB7] = 'À';
                result[0xB8] = '©'; result[0xBD] = '¢'; result[0xBE] = '¥';
                result[0xC6] = 'ã'; result[0xC7] = 'Ã';
                result[0xCF] = '¤';
                result[0xD0] = 'ð'; result[0xD1] = 'Ð'; result[0xD2] = 'Ê'; result[0xD3] = 'Ë'; result[0xD4] = 'È';
                result[0xD5] = 'ı'; result[0xD6] = 'Í'; result[0xD7] = 'Î';
                result[0xD8] = 'Ï'; result[0xDD] = '¦'; result[0xDE] = 'Ì';
                result[0xE0] = 'Ó'; result[0xE2] = 'Ô'; result[0xE3] = 'Ò'; result[0xE4] = 'õ';
                result[0xE5] = 'Õ'; result[0xE7] = 'þ';
                result[0xE8] = 'Þ'; result[0xE9] = 'Ú'; result[0xEA] = 'Û'; result[0xEB] = 'Ù';
                result[0xEC] = 'ý'; result[0xED] = 'Ý'; result[0xEE] = '¯'; result[0xEF] = '´';
                result[0xF0] = '­'/*SHY*/; result[0xF2] = '‗'; result[0xF3] = '¾'; result[0xF4] = '¶';
                result[0xF5] = '§'; result[0xF7] = '¸';
                result[0xF9] = '¨'; result[0xFB] = '¹'; result[0xFC] = '³';

                return result;
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

        #region encoder decoder
        public static Encoding CP437Encoding = new CodePageEncoding(437);
        public static Encoding CP850Encoding = new CodePageEncoding(850);
        public static Encoding CP852Encoding = new CodePageEncoding(852);
        public static Encoding CP858Encoding = new CodePageEncoding(858);
        public static Encoding Windows1252Encoding = Encoding.GetEncoding(1252);
        public static Encoding ISO8859_1Encoding = Encoding.GetEncoding(28591);

        /*
        private class CodePageEncodingProvider : EncodingProvider
        {
            public CodePageEncodingProvider()
            {

            }

            //
            // Summary:
            //     Returns the encoding with the specified name.
            //
            // Parameters:
            //   name:
            //     The name of the requested encoding.
            //
            // Returns:
            //     The encoding that is associated with the specified name, or null if this System.Text.EncodingProvider
            //     cannot return a valid encoding that corresponds to name.
            public override Encoding GetEncoding(string name)
            {
                return GetEncoding(GetCodepage(name));
            }
            //
            // Summary:
            //     Returns the encoding associated with the specified code page identifier.
            //
            // Parameters:
            //   codepage:
            //     The code page identifier of the requested encoding.
            //
            // Returns:
            //     The encoding that is associated with the specified code page, or null if this
            //     System.Text.EncodingProvider cannot return a valid encoding that corresponds
            //     to codepage.
            public override Encoding GetEncoding(int codepage)
            {
                switch (codepage)
                {
                    case 437: return CP437Encoding;
                    case 850: return CP850Encoding;
                    case 852: return CP852Encoding;
                    case 858: return CP858Encoding;
                    case 1200: return Encoding.Unicode;
                    case 1252: return Windows1252Encoding;
                    case 20127: return Encoding.ASCII;
                    case 28591: return ISO8859_1Encoding;
                    default: throw new NotSupportedException();
                }
            }
        }
        */

        private class CodePageEncoding : Encoding
        {
            private readonly int codePage;

            public override int CodePage { get { return codePage; } }

            public override bool IsSingleByte
            {
                get
                {
                    return true;
                }
            }

            Encoder encoder;
            Decoder decoder;
            Encoding encoding;

            public CodePageEncoding(int CodePage)
            {
                codePage = CodePage;
                switch(codePage)
                {
                    case 437:
                        encoder = new CodePageEncoder(CodePage437);
                        decoder = new CodePageDecoder(CodePage437);
                        break;
                    case 850:
                        encoder = new CodePageEncoder(CodePage850);
                        decoder = new CodePageDecoder(CodePage850);
                        break;
                    case 852:
                        encoder = new CodePageEncoder(CodePage852);
                        decoder = new CodePageDecoder(CodePage852);
                        break;
                    case 858:
                        encoder = new CodePageEncoder(CodePage858);
                        decoder = new CodePageDecoder(CodePage858);
                        break;
                    default:
                        {
                            encoding = Encoding.GetEncoding(codePage);
                            decoder = encoding.GetDecoder();
                            encoder = encoding.GetEncoder();
                            break;
                        }
                }
            }

            public CodePageEncoding(int CodePage, char[] character)
            {
                codePage = CodePage;
                encoder = new CodePageEncoder(character);
                decoder = new CodePageDecoder(character);
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                if (encoding != null)
                    return encoding.GetCharCount(bytes, index, count);
                else
                    return count;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                if (encoding != null)
                    return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
                else
                    return decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                if (encoding != null)
                    return encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
                else
                    return encoder.GetBytes(chars, charIndex, charCount, bytes, byteIndex, false);
            }

            public override int GetByteCount(char[] chars, int index, int count)
            {
                if (encoding != null)
                    return encoding.GetByteCount(chars, index, count);
                else
                    return count;
            }

            public override int GetMaxByteCount(int charCount)
            {
                if (encoding != null)
                    return encoding.GetMaxByteCount(charCount);
                else
                    return charCount;
            }

            public override int GetMaxCharCount(int byteCount)
            {
                if (encoding != null)
                    return encoding.GetMaxCharCount(byteCount);
                else
                    return byteCount;
            }
        }

        private class CodePageEncoder : Encoder
        {
            private readonly Dictionary<char, byte> codePageReverse;
            private readonly char[] codePage;

            public CodePageEncoder(char[] codePage)
            {
                this.codePage = (char[])codePage.Clone();
                codePageReverse = new Dictionary<char, byte>();

                for(int i = 0; i < codePage.Length; i++)
                {
                    if(!codePageReverse.ContainsKey(codePage[i]))
                        codePageReverse.Add(codePage[i], (byte)i);
                }
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
            {
                int writenBytes = 0;
                for( ; charIndex < chars.Length && byteIndex < bytes.Length; charIndex++, byteIndex++, charCount--, writenBytes++)
                {
                    if(codePageReverse.ContainsKey(chars[charIndex]))
                    {
                        bytes[byteIndex] = codePageReverse[chars[charIndex]];
                    }
                    else
                    {
                        bytes[byteIndex] = 21;
                    }
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
            private readonly Dictionary<char, byte> codePageReverse;
            private readonly char[] codePage;

            public CodePageDecoder(char[] codePage)
            {
                this.codePage = (char[])codePage.Clone();
                codePageReverse = new Dictionary<char, byte>();

                for (int i = 0; i < codePage.Length; i++)
                {
                    if (!codePageReverse.ContainsKey(codePage[i]))
                        codePageReverse.Add(codePage[i], (byte)i);
                }
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

        public static char GetCharacterFromOffset(int codePage, int offest)
        {
            return new CodePageInfo(codePage).GetCharacterFromOffset(offest);
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

        public static int GetOffsetFromCharacter(int codePage, char c)
        {
            return new CodePageInfo(codePage).GetOffsetFromCharacter(c);
        }


        public char[] GetAllValidCharacter()
        {
            int max = 255;
            List<char> list;

            if (encoding.CodePage == 20127)
                max = 127;
            else if (encoding.CodePage == 1200)
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

        public static string GetAllValidCharacter(string codePage)
        {
            return new string(new CodePageInfo(codePage).GetAllValidCharacter());
        }

        public static string GetAllValidCharacter(int codePage)
        {
            return new string(new CodePageInfo(codePage).GetAllValidCharacter());
        }


        public char GetLastValidCharacter()
        {
            return GetAllValidCharacter().Last();
        }

        public static char GetLastValidCharacter(int codePage)
        {
            return GetAllValidCharacter(codePage).Last();
        }


        public char GetFirstValidCharacter()
        {
            return GetAllValidCharacter()[0];
        }

        public static char GetFirstValidCharacter(int codePage)
        {
            return GetAllValidCharacter(codePage)[0];
        }


        public int GetCharacterDifferance(char a, char b)
        {
            int offset_a, offset_b;

            offset_a = GetOffsetFromCharacter(a);

            offset_b = GetOffsetFromCharacter(b);

            return offset_b - offset_a;
        }

        public static int GetCharacterDifferance(int codePage, char a, char b)
        {
            return new CodePageInfo(codePage).GetCharacterDifferance(a, b);
        }


        public static string[] GetEncoderNameList()
        {
            List<string> sl = new List<string>();
            sl.Add(GetCodepageName(20127)); //ASCII
            sl.Add(GetCodepageName(437));   //CP437
            sl.Add(GetCodepageName(850));   //CP850
            sl.Add(GetCodepageName(852));   //CP852
            sl.Add(GetCodepageName(858));   //CP858
            sl.Add(GetCodepageName(1252));  //Windows CP1252
            sl.Add(GetCodepageName(28591)); //ISO-8859-1
            sl.Add(GetCodepageName(1200));  //UTF-16
            return sl.ToArray();
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
