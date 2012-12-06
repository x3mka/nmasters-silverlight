//using System;

//namespace NMasters.Silverlight.Net
//{
//    internal static class UriHelper
//    {
//        // Fields
//        private const short c_EncodedCharsPerByte = 3;
//        private const short c_MaxAsciiCharsReallocate = 40;
//        private const short c_MaxUnicodeCharsReallocate = 40;
//        private const short c_MaxUTF_8BytesPerUnicodeChar = 4;

//        private static readonly char[] HexUpperChars = new char[]
//                                                           {
//                                                               '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B',
//                                                               'C', 'D', 'E', 'F'
//                                                           };

//        private const string RFC2396ReservedMarks = ";/?:@&=+$,";
//        private const string RFC2396UnreservedMarks = "-_.!~*'()";
//        private const string RFC3986ReservedMarks = ":/?#[]@!$&'()*+,;=";
//        private const string RFC3986UnreservedMarks = "-._~";
      
//        internal static void EscapeAsciiChar(char ch, char[] to, ref int pos)
//        {
//            to[pos++] = '%';
//            to[pos++] = HexUpperChars[(ch & 240) >> 4];
//            to[pos++] = HexUpperChars[ch & '\x000f'];
//        }

//        internal static char EscapedAscii(char digit, char next)
//        {
//            if ((((digit < '0') || (digit > '9')) && ((digit < 'A') || (digit > 'F'))) &&
//                ((digit < 'a') || (digit > 'f')))
//            {
//                return (char) 0xffff;
//            }
//            int num = (digit <= '9') ? (digit - '0') : (((digit <= 'F') ? (digit - 'A') : (digit - 'a')) + 10);
//            if ((((next < '0') || (next > '9')) && ((next < 'A') || (next > 'F'))) && ((next < 'a') || (next > 'f')))
//            {
//                return (char) 0xffff;
//            }
//            return
//                (char)
//                ((num << 4) + ((next <= '9') ? (next - '0') : (((next <= 'F') ? (next - 'A') : (next - 'a')) + 10)));
//        }

//        internal static unsafe char[] EscapeString(string input, int start, int end, char[] dest, ref int destPos,
//                                                   bool isUriString, char force1, char force2, char rsvd)
//        {
//            if ((end - start) >= 0xfff0)
//            {
//                throw new UriFormatException(SR.GetString("net_uri_SizeLimit"));
//            }
//            int index = start;
//            int prevInputPos = start;
//            byte* bytes = stackalloc byte[160];
//            fixed (char* str = ((char*) input))
//            {
//                char* pStr = str;
//                while (index < end)
//                {
//                    char ch = pStr[index];
//                    if (ch > '\x007f')
//                    {
//                        short num3 = (short) Math.Min(end - index, 0x27);
//                        short charCount = 1;
//                        while ((charCount < num3) && (pStr[index + charCount] > '\x007f'))
//                        {
//                            charCount = (short) (charCount + 1);
//                        }
//                        if ((pStr[(index + charCount) - 1] >= 0xd800) && (pStr[(index + charCount) - 1] <= 0xdbff))
//                        {
//                            if ((charCount == 1) || (charCount == (end - index)))
//                            {
//                                throw new UriFormatException(SR.GetString("net_uri_BadString"));
//                            }
//                            charCount = (short) (charCount + 1);
//                        }
//                        dest = EnsureDestinationSize(pStr, dest, index, (short) ((charCount*4)*3), 480, ref destPos,
//                                                     prevInputPos);
//                        short num5 = (short) Encoding.UTF8.GetBytes(pStr + index, charCount, bytes, 160);
//                        if (num5 == 0)
//                        {
//                            throw new UriFormatException(SR.GetString("net_uri_BadString"));
//                        }
//                        index += charCount - 1;
//                        for (charCount = 0; charCount < num5; charCount = (short) (charCount + 1))
//                        {
//                            EscapeAsciiChar(*((char*) (bytes + charCount)), dest, ref destPos);
//                        }
//                        prevInputPos = index + 1;
//                    }
//                    else if ((ch == '%') && (rsvd == '%'))
//                    {
//                        dest = EnsureDestinationSize(pStr, dest, index, 3, 120, ref destPos, prevInputPos);
//                        if (((index + 2) < end) && (EscapedAscii(pStr[index + 1], pStr[index + 2]) != 0xffff))
//                        {
//                            dest[destPos++] = '%';
//                            dest[destPos++] = pStr[index + 1];
//                            dest[destPos++] = pStr[index + 2];
//                            index += 2;
//                        }
//                        else
//                        {
//                            EscapeAsciiChar('%', dest, ref destPos);
//                        }
//                        prevInputPos = index + 1;
//                    }
//                    else if ((ch == force1) || (ch == force2))
//                    {
//                        dest = EnsureDestinationSize(pStr, dest, index, 3, 120, ref destPos, prevInputPos);
//                        EscapeAsciiChar(ch, dest, ref destPos);
//                        prevInputPos = index + 1;
//                    }
//                    else if ((ch != rsvd) && (isUriString ? !IsReservedUnreservedOrHash(ch) : !IsUnreserved(ch)))
//                    {
//                        dest = EnsureDestinationSize(pStr, dest, index, 3, 120, ref destPos, prevInputPos);
//                        EscapeAsciiChar(ch, dest, ref destPos);
//                        prevInputPos = index + 1;
//                    }
//                    index++;
//                }
//                if ((prevInputPos != index) && ((prevInputPos != start) || (dest != null)))
//                {
//                    dest = EnsureDestinationSize(pStr, dest, index, 0, 0, ref destPos, prevInputPos);
//                }
//            }
//            return dest;
//        }

//        internal static bool Is3986Unreserved(char c)
//        {
//            return (Uri.IsAsciiLetterOrDigit(c) || ("-._~".IndexOf(c) >= 0));
//        }

//        internal static bool IsNotSafeForUnescape(char ch)
//        {
//            if (((ch > '\x001f') && ((ch < '\x007f') || (ch > '\x009f'))) &&
//                (((((ch < ';') || (ch > '@')) || ((ch | '\x0002') == 0x3e)) && ((ch < '#') || (ch > '&'))) &&
//                 (((ch != '+') && (ch != ',')) && ((ch != '/') && (ch != '\\')))))
//            {
//                return false;
//            }
//            return true;
//        }

//        private static bool IsReservedUnreservedOrHash(char c)
//        {
//            if (!IsUnreserved(c))
//            {
//                if (!UriParser.ShouldUseLegacyV2Quirks)
//                {
//                    return (":/?#[]@!$&'()*+,;=".IndexOf(c) >= 0);
//                }
//                if (";/?:@&=+$,".IndexOf(c) < 0)
//                {
//                    return (c == '#');
//                }
//            }
//            return true;
//        }

//        internal static bool IsUnreserved(char c)
//        {
//            if (Uri.IsAsciiLetterOrDigit(c))
//            {
//                return true;
//            }
//            if (UriParser.ShouldUseLegacyV2Quirks)
//            {
//                return ("-_.!~*'()".IndexOf(c) >= 0);
//            }
//            return ("-._~".IndexOf(c) >= 0);
//        }

//        internal static unsafe void MatchUTF8Sequence(char* pDest, char[] dest, ref int destOffset,
//                                                      char[] unescapedChars, int charCount, byte[] bytes, int byteCount,
//                                                      bool isQuery, bool iriParsing)
//        {
//            char[] chArray;
//            int index = 0;
//            if (((chArray = unescapedChars) == null) || (chArray.Length == 0))
//            {
//                chRef = null;
//                goto Label_001C;
//            }
//            fixed (char* chRef = chArray)
//            {
//                int num2;
//                Label_001C:
//                num2 = 0;
//                while (num2 < charCount)
//                {
//                    bool flag = char.IsHighSurrogate(chRef[num2]);
//                    byte[] buffer = Encoding.UTF8.GetBytes(unescapedChars, num2, flag ? 2 : 1);
//                    int length = buffer.Length;
//                    bool flag2 = false;
//                    if (iriParsing)
//                    {
//                        if (!flag)
//                        {
//                            flag2 = Uri.CheckIriUnicodeRange(unescapedChars[num2], isQuery);
//                        }
//                        else
//                        {
//                            bool surrogatePair = false;
//                            flag2 = Uri.CheckIriUnicodeRange(unescapedChars[num2], unescapedChars[num2 + 1],
//                                                             ref surrogatePair, isQuery);
//                        }
//                    }
//                    Label_008B:
//                    while (bytes[index] != buffer[0])
//                    {
//                        EscapeAsciiChar((char) bytes[index++], dest, ref destOffset);
//                    }
//                    bool flag4 = true;
//                    int num4 = 0;
//                    while (num4 < length)
//                    {
//                        if (bytes[index + num4] != buffer[num4])
//                        {
//                            flag4 = false;
//                            break;
//                        }
//                        num4++;
//                    }
//                    if (flag4)
//                    {
//                        index += length;
//                        if (iriParsing)
//                        {
//                            if (!flag2)
//                            {
//                                for (int i = 0; i < buffer.Length; i++)
//                                {
//                                    EscapeAsciiChar((char) buffer[i], dest, ref destOffset);
//                                }
//                            }
//                            else if (!Uri.IsBidiControlCharacter(chRef[num2]))
//                            {
//                                pDest[destOffset++] = chRef[num2];
//                            }
//                            if (flag)
//                            {
//                                pDest[destOffset++] = chRef[num2 + 1];
//                            }
//                        }
//                        else
//                        {
//                            pDest[destOffset++] = chRef[num2];
//                            if (flag)
//                            {
//                                pDest[destOffset++] = chRef[num2 + 1];
//                            }
//                        }
//                    }
//                    else
//                    {
//                        for (int j = 0; j < num4; j++)
//                        {
//                            EscapeAsciiChar((char) bytes[index++], dest, ref destOffset);
//                        }
//                        goto Label_008B;
//                    }
//                    if (flag)
//                    {
//                        num2++;
//                    }
//                    num2++;
//                }
//            }
//            while (index < byteCount)
//            {
//                EscapeAsciiChar((char) bytes[index++], dest, ref destOffset);
//            }
//        }

//        internal static unsafe bool TestForSubPath(char* pMe, ushort meLength, char* pShe, ushort sheLength,
//                                                   bool ignoreCase)
//        {
//            char ch;
//            ushort index = 0;
//            bool flag = true;
//            while ((index < meLength) && (index < sheLength))
//            {
//                ch = pMe[index];
//                char c = pShe[index];
//                if ((ch == '?') || (ch == '#'))
//                {
//                    return true;
//                }
//                if (ch == '/')
//                {
//                    if (c != '/')
//                    {
//                        return false;
//                    }
//                    if (!flag)
//                    {
//                        return false;
//                    }
//                    flag = true;
//                }
//                else
//                {
//                    switch (c)
//                    {
//                        case '?':
//                        case '#':
//                            goto Label_0096;
//                    }
//                    if (!ignoreCase)
//                    {
//                        if (ch != c)
//                        {
//                            flag = false;
//                        }
//                    }
//                    else if (char.ToLower(ch, CultureInfo.InvariantCulture) !=
//                             char.ToLower(c, CultureInfo.InvariantCulture))
//                    {
//                        flag = false;
//                    }
//                }
//                index = (ushort) (index + 1);
//            }
//            Label_0096:
//            while (index < meLength)
//            {
//                if (((ch = pMe[index]) == '?') || (ch == '#'))
//                {
//                    return true;
//                }
//                if (ch == '/')
//                {
//                    return false;
//                }
//                index = (ushort) (index + 1);
//            }
//            return true;
//        }

//        internal static unsafe char[] UnescapeString(char* pStr, int start, int end, char[] dest, ref int destPosition,
//                                                     char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode,
//                                                     UriParser syntax, bool isQuery)
//        {
//            byte[] bytes = null;
//            byte num = 0;
//            bool flag = false;
//            int index = start;
//            bool iriParsing = Uri.IriParsingStatic(syntax) &&
//                              ((unescapeMode & UnescapeMode.EscapeUnescape) == UnescapeMode.EscapeUnescape);
//            Label_001D:
//            ;
//            try
//            {
//                fixed (char* chRef = dest)
//                {
//                    char ch;
//                    if ((unescapeMode & UnescapeMode.EscapeUnescape) == UnescapeMode.CopyOnly)
//                    {
//                        while (start < end)
//                        {
//                            chRef[destPosition++] = pStr[start++];
//                        }
//                        return dest;
//                    }
//                    Label_006E:
//                    ch = '\0';
//                    while (index < end)
//                    {
//                        ch = pStr[index];
//                        if (ch == '%')
//                        {
//                            if ((unescapeMode & UnescapeMode.Unescape) == UnescapeMode.CopyOnly)
//                            {
//                                flag = true;
//                            }
//                            else
//                            {
//                                if ((index + 2) < end)
//                                {
//                                    ch = EscapedAscii(pStr[index + 1], pStr[index + 2]);
//                                    if (unescapeMode >= UnescapeMode.UnescapeAll)
//                                    {
//                                        if (ch == 0xffff)
//                                        {
//                                            if (unescapeMode >= UnescapeMode.UnescapeAllOrThrow)
//                                            {
//                                                throw new UriFormatException(SR.GetString("net_uri_BadString"));
//                                            }
//                                            goto Label_01D7;
//                                        }
//                                        break;
//                                    }
//                                    if (ch == 0xffff)
//                                    {
//                                        if ((unescapeMode & UnescapeMode.Escape) == UnescapeMode.CopyOnly)
//                                        {
//                                            goto Label_01D7;
//                                        }
//                                        flag = true;
//                                        break;
//                                    }
//                                    if (ch == '%')
//                                    {
//                                        index += 2;
//                                    }
//                                    else if (((ch == rsvd1) || (ch == rsvd2)) || (ch == rsvd3))
//                                    {
//                                        index += 2;
//                                    }
//                                    else if (((unescapeMode & UnescapeMode.V1ToStringFlag) == UnescapeMode.CopyOnly) &&
//                                             IsNotSafeForUnescape(ch))
//                                    {
//                                        index += 2;
//                                    }
//                                    else
//                                    {
//                                        if (!iriParsing ||
//                                            (((ch > '\x009f') || !IsNotSafeForUnescape(ch)) &&
//                                             ((ch <= '\x009f') || Uri.CheckIriUnicodeRange(ch, isQuery))))
//                                        {
//                                            break;
//                                        }
//                                        index += 2;
//                                    }
//                                    goto Label_01D7;
//                                }
//                                if (unescapeMode >= UnescapeMode.UnescapeAll)
//                                {
//                                    if (unescapeMode >= UnescapeMode.UnescapeAllOrThrow)
//                                    {
//                                        throw new UriFormatException(SR.GetString("net_uri_BadString"));
//                                    }
//                                    goto Label_01D7;
//                                }
//                                flag = true;
//                            }
//                            break;
//                        }
//                        if (((unescapeMode & (UnescapeMode.UnescapeAll | UnescapeMode.Unescape)) !=
//                             (UnescapeMode.UnescapeAll | UnescapeMode.Unescape)) &&
//                            ((unescapeMode & UnescapeMode.Escape) != UnescapeMode.CopyOnly))
//                        {
//                            if (((ch == rsvd1) || (ch == rsvd2)) || (ch == rsvd3))
//                            {
//                                flag = true;
//                                break;
//                            }
//                            if (((unescapeMode & UnescapeMode.V1ToStringFlag) == UnescapeMode.CopyOnly) &&
//                                ((ch <= '\x001f') || ((ch >= '\x007f') && (ch <= '\x009f'))))
//                            {
//                                flag = true;
//                                break;
//                            }
//                        }
//                        Label_01D7:
//                        index++;
//                    }
//                    while (start < index)
//                    {
//                        chRef[destPosition++] = pStr[start++];
//                    }
//                    if (index != end)
//                    {
//                        if (flag)
//                        {
//                            if (num == 0)
//                            {
//                                num = 30;
//                                char[] chArray = new char[dest.Length + (num*3)];
//                                fixed (char* chRef2 = chArray)
//                                {
//                                    for (int i = 0; i < destPosition; i++)
//                                    {
//                                        chRef2[i] = chRef[i];
//                                    }
//                                }
//                                dest = chArray;
//                                goto Label_001D;
//                            }
//                            num = (byte) (num - 1);
//                            EscapeAsciiChar(pStr[index], dest, ref destPosition);
//                            flag = false;
//                            start = ++index;
//                            goto Label_006E;
//                        }
//                        if (ch <= '\x007f')
//                        {
//                            dest[destPosition++] = ch;
//                            index += 3;
//                            start = index;
//                            goto Label_006E;
//                        }
//                        int byteCount = 1;
//                        if (bytes == null)
//                        {
//                            bytes = new byte[end - index];
//                        }
//                        bytes[0] = (byte) ch;
//                        index += 3;
//                        while (index < end)
//                        {
//                            if (((ch = pStr[index]) != '%') || ((index + 2) >= end))
//                            {
//                                break;
//                            }
//                            ch = EscapedAscii(pStr[index + 1], pStr[index + 2]);
//                            if ((ch == 0xffff) || (ch < '\x0080'))
//                            {
//                                break;
//                            }
//                            bytes[byteCount++] = (byte) ch;
//                            index += 3;
//                        }
//                        Encoding encoding = (Encoding) Encoding.UTF8.Clone();
//                        encoding.EncoderFallback = new EncoderReplacementFallback("");
//                        encoding.DecoderFallback = new DecoderReplacementFallback("");
//                        char[] chars = new char[bytes.Length];
//                        int charCount = encoding.GetChars(bytes, 0, byteCount, chars, 0);
//                        start = index;
//                        MatchUTF8Sequence(chRef, dest, ref destPosition, chars, charCount, bytes, byteCount, isQuery,
//                                          iriParsing);
//                    }
//                    if (index != end)
//                    {
//                        goto Label_006E;
//                    }
//                    return dest;
//                }
//            }
//            finally
//            {
//                chRef = null;
//            }
//            return dest;
//        }

//        internal static unsafe char[] UnescapeString(string input, int start, int end, char[] dest, ref int destPosition,
//                                                     char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode,
//                                                     UriParser syntax, bool isQuery)
//        {
//            fixed (char* str = ((char*) input))
//            {
//                char* pStr = str;
//                return UnescapeString(pStr, start, end, dest, ref destPosition, rsvd1, rsvd2, rsvd3, unescapeMode,
//                                      syntax, isQuery);
//            }
//        }
//    }
//}