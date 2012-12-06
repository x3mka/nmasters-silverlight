using System;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the value of the Content-Range header.</summary>
    public class ContentRangeHeaderValue //: ICloneable
    {
        private long? from;
        private long? length;
        private long? to;
        private string unit;

        private ContentRangeHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
        /// <param name="length">The starting or ending point of the range, in bytes.</param>
        public ContentRangeHeaderValue(long length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            this.length = new long?(length);
            this.unit = "bytes";
        }

        private ContentRangeHeaderValue(ContentRangeHeaderValue source)
        {
            this.from = source.from;
            this.to = source.to;
            this.length = source.length;
            this.unit = source.unit;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
        /// <param name="from">The position, in bytes, at which to start sending data.</param>
        /// <param name="to">The position, in bytes, at which to stop sending data.</param>
        public ContentRangeHeaderValue(long from, long to)
        {
            if (to < 0)
            {
                throw new ArgumentOutOfRangeException("to");
            }
            if ((from < 0) || (from > to))
            {
                throw new ArgumentOutOfRangeException("from");
            }
            this.from = new long?(from);
            this.to = new long?(to);
            this.unit = "bytes";
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> class.</summary>
        /// <param name="from">The position, in bytes, at which to start sending data.</param>
        /// <param name="to">The position, in bytes, at which to stop sending data.</param>
        /// <param name="length">The starting or ending point of the range, in bytes.</param>
        public ContentRangeHeaderValue(long from, long to, long length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            if ((to < 0) || (to > length))
            {
                throw new ArgumentOutOfRangeException("to");
            }
            if ((from < 0) || (from > to))
            {
                throw new ArgumentOutOfRangeException("from");
            }
            this.from = new long?(from);
            this.to = new long?(to);
            this.length = new long?(length);
            this.unit = "bytes";
        }

        /// <summary>Determines whether the specified Object is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            ContentRangeHeaderValue value2 = obj as ContentRangeHeaderValue;
            return (((value2 != null) && (((this.from == value2.from) && (this.to == value2.to)) && (this.length == value2.length))) && (string.Compare(this.unit, value2.unit, StringComparison.OrdinalIgnoreCase) == 0));
        }

        internal static int GetContentRangeLength(string input, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
            if (tokenLength == 0)
            {
                return 0;
            }
            string unit = input.Substring(startIndex, tokenLength);
            int num2 = startIndex + tokenLength;
            int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, num2);
            if (whitespaceLength == 0)
            {
                return 0;
            }
            num2 += whitespaceLength;
            if (num2 == input.Length)
            {
                return 0;
            }
            int fromStartIndex = num2;
            int fromLength = 0;
            int toStartIndex = 0;
            int toLength = 0;
            if (!TryGetRangeLength(input, ref num2, out fromLength, out toStartIndex, out toLength))
            {
                return 0;
            }
            if ((num2 == input.Length) || (input[num2] != '/'))
            {
                return 0;
            }
            num2++;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if (num2 == input.Length)
            {
                return 0;
            }
            int lengthStartIndex = num2;
            int lengthLength = 0;
            if (!TryGetLengthLength(input, ref num2, out lengthLength))
            {
                return 0;
            }
            if (!TryCreateContentRange(input, unit, fromStartIndex, fromLength, toStartIndex, toLength, lengthStartIndex, lengthLength, out parsedValue))
            {
                return 0;
            }
            return (num2 - startIndex);
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = this.unit.ToLowerInvariant().GetHashCode();
            if (this.HasRange)
            {
                hashCode = (hashCode ^ this.from.GetHashCode()) ^ this.to.GetHashCode();
            }
            if (this.HasLength)
            {
                hashCode ^= this.length.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents content range header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid content range header value information.</exception>
        public static ContentRangeHeaderValue Parse(string input)
        {
            int index = 0;
            return (ContentRangeHeaderValue) GenericHeaderParser.ContentRangeParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new ContentRangeHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(this.unit);
            builder.Append(' ');
            if (this.HasRange)
            {
                builder.Append(this.from.Value.ToString(NumberFormatInfo.InvariantInfo));
                builder.Append('-');
                builder.Append(this.to.Value.ToString(NumberFormatInfo.InvariantInfo));
            }
            else
            {
                builder.Append('*');
            }
            builder.Append('/');
            if (this.HasLength)
            {
                builder.Append(this.length.Value.ToString(NumberFormatInfo.InvariantInfo));
            }
            else
            {
                builder.Append('*');
            }
            return builder.ToString();
        }

        private static bool TryCreateContentRange(string input, string unit, int fromStartIndex, int fromLength, int toStartIndex, int toLength, int lengthStartIndex, int lengthLength, out object parsedValue)
        {
            parsedValue = null;
            long result = 0;
            if ((fromLength > 0) && !HeaderUtilities.TryParseInt64(input.Substring(fromStartIndex, fromLength), out result))
            {
                return false;
            }
            long num2 = 0;
            if ((toLength > 0) && !HeaderUtilities.TryParseInt64(input.Substring(toStartIndex, toLength), out num2))
            {
                return false;
            }
            if (((fromLength > 0) && (toLength > 0)) && (result > num2))
            {
                return false;
            }
            long num3 = 0;
            if ((lengthLength > 0) && !HeaderUtilities.TryParseInt64(input.Substring(lengthStartIndex, lengthLength), out num3))
            {
                return false;
            }
            if (((toLength > 0) && (lengthLength > 0)) && (num2 >= num3))
            {
                return false;
            }
            ContentRangeHeaderValue value2 = new ContentRangeHeaderValue {
                unit = unit
            };
            if (fromLength > 0)
            {
                value2.from = new long?(result);
                value2.to = new long?(num2);
            }
            if (lengthLength > 0)
            {
                value2.length = new long?(num3);
            }
            parsedValue = value2;
            return true;
        }

        private static bool TryGetLengthLength(string input, ref int current, out int lengthLength)
        {
            lengthLength = 0;
            if (input[current] == '*')
            {
                current++;
            }
            else
            {
                lengthLength = HttpRuleParser.GetNumberLength(input, current, false);
                if ((lengthLength == 0) || (lengthLength > 0x13))
                {
                    return false;
                }
                current += lengthLength;
            }
            current += HttpRuleParser.GetWhitespaceLength(input, current);
            return true;
        }

        private static bool TryGetRangeLength(string input, ref int current, out int fromLength, out int toStartIndex, out int toLength)
        {
            fromLength = 0;
            toStartIndex = 0;
            toLength = 0;
            if (input[current] == '*')
            {
                current++;
            }
            else
            {
                fromLength = HttpRuleParser.GetNumberLength(input, current, false);
                if ((fromLength == 0) || (fromLength > 0x13))
                {
                    return false;
                }
                current += fromLength;
                current += HttpRuleParser.GetWhitespaceLength(input, current);
                if ((current == input.Length) || (input[current] != '-'))
                {
                    return false;
                }
                current++;
                current += HttpRuleParser.GetWhitespaceLength(input, current);
                if (current == input.Length)
                {
                    return false;
                }
                toStartIndex = current;
                toLength = HttpRuleParser.GetNumberLength(input, current, false);
                if ((toLength == 0) || (toLength > 0x13))
                {
                    return false;
                }
                current += toLength;
            }
            current += HttpRuleParser.GetWhitespaceLength(input, current);
            return true;
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out ContentRangeHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.ContentRangeParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (ContentRangeHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets the position at which to start sending data.</summary>
        /// <returns>Returns <see cref="T:System.Int64" />.The position, in bytes, at which to start sending data.</returns>
        public long? From
        {
            get
            {
                return this.from;
            }
        }

        /// <summary>Gets whether the Content-Range header has a length specified.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the Content-Range has a length specified; otherwise, false.</returns>
        public bool HasLength
        {
            get
            {
                return this.length.HasValue;
            }
        }

        /// <summary>Gets whether the Content-Range has a range specified. </summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the Content-Range has a range specified; otherwise, false.</returns>
        public bool HasRange
        {
            get
            {
                return this.from.HasValue;
            }
        }

        /// <summary>Gets the length of the full entity-body.</summary>
        /// <returns>Returns <see cref="T:System.Int64" />.The length of the full entity-body.</returns>
        public long? Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>Gets the position at which to stop sending data.</summary>
        /// <returns>Returns <see cref="T:System.Int64" />.The position at which to stop sending data.</returns>
        public long? To
        {
            get
            {
                return this.to;
            }
        }

        /// <summary>The range units used.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A <see cref="T:System.String" /> that contains range units. </returns>
        public string Unit
        {
            get
            {
                return this.unit;
            }
            set
            {
                HeaderUtilities.CheckValidToken(value, "value");
                this.unit = value;
            }
        }
    }
}

