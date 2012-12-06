using System;
using System.Collections.Generic;
using System.Text;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the value of the Range header.</summary>
    public class RangeHeaderValue //: ICloneable
    {
        private ICollection<RangeItemHeaderValue> ranges;
        private string unit;

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> class.</summary>
        public RangeHeaderValue()
        {
            this.unit = "bytes";
        }

        //private RangeHeaderValue(RangeHeaderValue source)
        //{
        //    this.unit = source.unit;
        //    if (source.ranges != null)
        //    {
        //        foreach (RangeItemHeaderValue value2 in source.ranges)
        //        {
        //            this.Ranges.Add((RangeItemHeaderValue) ((ICloneable) value2).Clone());
        //        }
        //    }
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> class.</summary>
        public RangeHeaderValue(long? from, long? to)
        {
            this.unit = "bytes";
            this.Ranges.Add(new RangeItemHeaderValue(from, to));
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            RangeHeaderValue value2 = obj as RangeHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((string.Compare(this.unit, value2.unit, StringComparison.OrdinalIgnoreCase) == 0) && HeaderUtilities.AreEqualCollections<RangeItemHeaderValue>(this.Ranges, value2.Ranges));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = this.unit.ToLowerInvariant().GetHashCode();
            foreach (RangeItemHeaderValue value2 in this.Ranges)
            {
                hashCode ^= value2.GetHashCode();
            }
            return hashCode;
        }

        internal static int GetRangeLength(string input, int startIndex, out object parsedValue)
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
            RangeHeaderValue value2 = new RangeHeaderValue {
                unit = input.Substring(startIndex, tokenLength)
            };
            int num2 = startIndex + tokenLength;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if ((num2 == input.Length) || (input[num2] != '='))
            {
                return 0;
            }
            num2++;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            int num3 = RangeItemHeaderValue.GetRangeItemListLength(input, num2, value2.Ranges);
            if (num3 == 0)
            {
                return 0;
            }
            num2 += num3;
            parsedValue = value2;
            return (num2 - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents range header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid range header value information.</exception>
        public static RangeHeaderValue Parse(string input)
        {
            int index = 0;
            return (RangeHeaderValue) GenericHeaderParser.RangeParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new RangeHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(this.unit);
            builder.Append('=');
            bool flag = true;
            foreach (RangeItemHeaderValue value2 in this.Ranges)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(", ");
                }
                builder.Append(value2.From);
                builder.Append('-');
                builder.Append(value2.To);
            }
            return builder.ToString();
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">he string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out RangeHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.RangeParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (RangeHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public ICollection<RangeItemHeaderValue> Ranges
        {
            get
            {
                if (this.ranges == null)
                {
                    this.ranges = new ObjectCollection<RangeItemHeaderValue>();
                }
                return this.ranges;
            }
        }

        /// <returns>Returns <see cref="T:System.String" />.</returns>
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

