using System;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a warning value used by the Warning header.</summary>
    public class WarningHeaderValue //: ICloneable
    {
        private string agent;
        private int code;
        private DateTimeOffset? date;
        private string text;

        private WarningHeaderValue()
        {
        }

        private WarningHeaderValue(WarningHeaderValue source)
        {
            this.code = source.code;
            this.agent = source.agent;
            this.text = source.text;
            this.date = source.date;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> class.</summary>
        /// <param name="code">The specific warning code.</param>
        /// <param name="agent">The host that attached the warning.</param>
        /// <param name="text">A quoted-string containing the warning text.</param>
        public WarningHeaderValue(int code, string agent, string text)
        {
            CheckCode(code);
            CheckAgent(agent);
            HeaderUtilities.CheckValidQuotedString(text, "text");
            this.code = code;
            this.agent = agent;
            this.text = text;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> class.</summary>
        /// <param name="code">The specific warning code.</param>
        /// <param name="agent">The host that attached the warning.</param>
        /// <param name="text">A quoted-string containing the warning text.</param>
        /// <param name="date">The date/time stamp of the warning.</param>
        public WarningHeaderValue(int code, string agent, string text, DateTimeOffset date)
        {
            CheckCode(code);
            CheckAgent(agent);
            HeaderUtilities.CheckValidQuotedString(text, "text");
            this.code = code;
            this.agent = agent;
            this.text = text;
            this.date = new DateTimeOffset?(date);
        }

        private static void CheckAgent(string agent)
        {
            if (string.IsNullOrEmpty(agent))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "agent");
            }
            string host = null;
            if (HttpRuleParser.GetHostLength(agent, 0, true, out host) != agent.Length)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { agent }));
            }
        }

        private static void CheckCode(int code)
        {
            if ((code < 0) || (code > 0x3e7))
            {
                throw new ArgumentOutOfRangeException("code");
            }
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            WarningHeaderValue value2 = obj as WarningHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (((this.code != value2.code) || (string.Compare(this.agent, value2.agent, StringComparison.OrdinalIgnoreCase) != 0)) || (string.CompareOrdinal(this.text, value2.text) != 0))
            {
                return false;
            }
            if (!this.date.HasValue)
            {
                return !value2.date.HasValue;
            }
            return (value2.date.HasValue && (this.date.Value == value2.date.Value));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int num = (this.code.GetHashCode() ^ this.agent.ToLowerInvariant().GetHashCode()) ^ this.text.GetHashCode();
            if (this.date.HasValue)
            {
                num ^= this.date.Value.GetHashCode();
            }
            return num;
        }

        internal static int GetWarningLength(string input, int startIndex, out object parsedValue)
        {
            int num;
            string str;
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int current = startIndex;
            if (!TryReadCode(input, ref current, out num))
            {
                return 0;
            }
            if (!TryReadAgent(input, current, ref current, out str))
            {
                return 0;
            }
            int length = 0;
            int num4 = current;
            if (HttpRuleParser.GetQuotedStringLength(input, current, out length) != HttpParseResult.Parsed)
            {
                return 0;
            }
            current += length;
            DateTimeOffset? date = null;
            if (!TryReadDate(input, ref current, out date))
            {
                return 0;
            }
            WarningHeaderValue value2 = new WarningHeaderValue {
                code = num,
                agent = str,
                text = input.Substring(num4, length),
                date = date
            };
            parsedValue = value2;
            return (current - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> instance.</summary>
        /// <returns>Returns an <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents authentication header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid authentication header value information.</exception>
        public static WarningHeaderValue Parse(string input)
        {
            int index = 0;
            return (WarningHeaderValue) GenericHeaderParser.SingleValueWarningParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.Returns a copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new WarningHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.code.ToString("000", NumberFormatInfo.InvariantInfo));
            builder.Append(' ');
            builder.Append(this.agent);
            builder.Append(' ');
            builder.Append(this.text);
            if (this.date.HasValue)
            {
                builder.Append(" \"");
                builder.Append(HttpRuleParser.DateToString(this.date.Value));
                builder.Append('"');
            }
            return builder.ToString();
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.WarningHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out WarningHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueWarningParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (WarningHeaderValue) obj2;
                return true;
            }
            return false;
        }

        private static bool TryReadAgent(string input, int startIndex, ref int current, out string agent)
        {
            agent = null;
            int num = HttpRuleParser.GetHostLength(input, startIndex, true, out agent);
            if (num == 0)
            {
                return false;
            }
            current += num;
            int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
            current += whitespaceLength;
            return ((whitespaceLength != 0) && (current != input.Length));
        }

        private static bool TryReadCode(string input, ref int current, out int code)
        {
            code = 0;
            int length = HttpRuleParser.GetNumberLength(input, current, false);
            if ((length == 0) || (length > 3))
            {
                return false;
            }
            if (!HeaderUtilities.TryParseInt32(input.Substring(current, length), out code))
            {
                return false;
            }
            current += length;
            int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
            current += whitespaceLength;
            return ((whitespaceLength != 0) && (current != input.Length));
        }

        private static bool TryReadDate(string input, ref int current, out DateTimeOffset? date)
        {
            // SL
            date = DateTimeOffset.MinValue;

            int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
            current += whitespaceLength;
            if ((current < input.Length) && (input[current] == '"'))
            {
                DateTimeOffset offset;
                if (whitespaceLength == 0)
                {
                    return false;
                }
                current++;
                int startIndex = current;
                while (current < input.Length)
                {
                    if (input[current] == '"')
                    {
                        break;
                    }
                    current++;
                }
                if ((current == input.Length) || (current == startIndex))
                {
                    return false;
                }
                if (!HttpRuleParser.TryStringToDate(input.Substring(startIndex, current - startIndex), out offset))
                {
                    return false;
                }
                date = new DateTimeOffset?(offset);
                current++;
                current += HttpRuleParser.GetWhitespaceLength(input, current);
            }
            return true;
        }

        /// <summary>Gets the host that attached the warning.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The host that attached the warning.</returns>
        public string Agent
        {
            get
            {
                return this.agent;
            }
        }

        /// <summary>Gets the specific warning code.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.The specific warning code.</returns>
        public int Code
        {
            get
            {
                return this.code;
            }
        }

        /// <summary>Gets the date/time stamp of the warning.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The date/time stamp of the warning.</returns>
        public DateTimeOffset? Date
        {
            get
            {
                return this.date;
            }
        }

        /// <summary>Gets a quoted-string containing the warning text.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A quoted-string containing the warning text.</returns>
        public string Text
        {
            get
            {
                return this.text;
            }
        }
    }
}

