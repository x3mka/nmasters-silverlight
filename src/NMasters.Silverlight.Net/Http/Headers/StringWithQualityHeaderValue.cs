using System;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a string header value with an optional quality.</summary>
    public class StringWithQualityHeaderValue //: ICloneable
    {
        private double? quality;
        private string value;

        private StringWithQualityHeaderValue()
        {
        }

        private StringWithQualityHeaderValue(StringWithQualityHeaderValue source)
        {
            this.value = source.value;
            this.quality = source.quality;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> class.</summary>
        public StringWithQualityHeaderValue(string value)
        {
            HeaderUtilities.CheckValidToken(value, "value");
            this.value = value;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> class.</summary>
        public StringWithQualityHeaderValue(string value, double quality)
        {
            HeaderUtilities.CheckValidToken(value, "value");
            if ((quality < 0.0) || (quality > 1.0))
            {
                throw new ArgumentOutOfRangeException("quality");
            }
            this.value = value;
            this.quality = new double?(quality);
        }

        /// <summary>Determines whether the specified Object is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            StringWithQualityHeaderValue value2 = obj as StringWithQualityHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (string.Compare(this.value, value2.value, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }
            if (!this.quality.HasValue)
            {
                return !value2.quality.HasValue;
            }
            return (value2.quality.HasValue && (this.quality.Value == value2.quality.Value));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = this.value.ToLowerInvariant().GetHashCode();
            if (this.quality.HasValue)
            {
                hashCode ^= this.quality.Value.GetHashCode();
            }
            return hashCode;
        }

        internal static int GetStringWithQualityLength(string input, int startIndex, out object parsedValue)
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
            StringWithQualityHeaderValue result = new StringWithQualityHeaderValue {
                value = input.Substring(startIndex, tokenLength)
            };
            int num2 = startIndex + tokenLength;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if ((num2 == input.Length) || (input[num2] != ';'))
            {
                parsedValue = result;
                return (num2 - startIndex);
            }
            num2++;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if (!TryReadQuality(input, result, ref num2))
            {
                return 0;
            }
            parsedValue = result;
            return (num2 - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents quality header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid string with quality header value information.</exception>
        public static StringWithQualityHeaderValue Parse(string input)
        {
            int index = 0;
            return (StringWithQualityHeaderValue) GenericHeaderParser.SingleValueStringWithQualityParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new StringWithQualityHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.quality.HasValue)
            {
                return (this.value + "; q=" + this.quality.Value.ToString("0.0##", NumberFormatInfo.InvariantInfo));
            }
            return this.value;
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.StringWithQualityHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out StringWithQualityHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueStringWithQualityParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (StringWithQualityHeaderValue) obj2;
                return true;
            }
            return false;
        }

        private static bool TryReadQuality(string input, StringWithQualityHeaderValue result, ref int index)
        {
            int startIndex = index;
            if ((startIndex == input.Length) || ((input[startIndex] != 'q') && (input[startIndex] != 'Q')))
            {
                return false;
            }
            startIndex++;
            startIndex += HttpRuleParser.GetWhitespaceLength(input, startIndex);
            if ((startIndex == input.Length) || (input[startIndex] != '='))
            {
                return false;
            }
            startIndex++;
            startIndex += HttpRuleParser.GetWhitespaceLength(input, startIndex);
            if (startIndex == input.Length)
            {
                return false;
            }
            int length = HttpRuleParser.GetNumberLength(input, startIndex, true);
            if (length == 0)
            {
                return false;
            }
            double num3 = 0.0;
            if (!double.TryParse(input.Substring(startIndex, length), NumberStyles.AllowDecimalPoint, (IFormatProvider) NumberFormatInfo.InvariantInfo, out num3))
            {
                return false;
            }
            if ((num3 < 0.0) || (num3 > 1.0))
            {
                return false;
            }
            result.quality = new double?(num3);
            startIndex += length;
            startIndex += HttpRuleParser.GetWhitespaceLength(input, startIndex);
            index = startIndex;
            return true;
        }

        /// <returns>Returns <see cref="T:System.Double" />.</returns>
        public double? Quality
        {
            get
            {
                return this.quality;
            }
        }

        /// <returns>Returns <see cref="T:System.String" />.</returns>
        public string Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

