using System;
using System.Collections.Generic;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a byte-range header value.</summary>
    public class RangeItemHeaderValue //: ICloneable
    {
        private long? from;
        private long? to;

        private RangeItemHeaderValue(RangeItemHeaderValue source)
        {
            this.from = source.from;
            this.to = source.to;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeItemHeaderValue" /> class.</summary>
        public RangeItemHeaderValue(long? from, long? to)
        {
            if (!from.HasValue && !to.HasValue)
            {
                throw new ArgumentException(SR.net_http_headers_invalid_range);
            }
            if (from.HasValue && (from.Value < 0))
            {
                throw new ArgumentOutOfRangeException("from");
            }
            if (to.HasValue && (to.Value < 0))
            {
                throw new ArgumentOutOfRangeException("to");
            }
            if ((from.HasValue && to.HasValue) && (from.Value > to.Value))
            {
                throw new ArgumentOutOfRangeException("from");
            }
            this.from = from;
            this.to = to;
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            RangeItemHeaderValue value2 = obj as RangeItemHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (this.from != value2.from)
            {
                return false;
            }
            return (this.to == value2.to);
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (!this.from.HasValue)
            {
                return this.to.GetHashCode();
            }
            if (!this.to.HasValue)
            {
                return this.from.GetHashCode();
            }
            return (this.from.GetHashCode() ^ this.to.GetHashCode());
        }

        internal static int GetRangeItemLength(string input, int startIndex, out RangeItemHeaderValue parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int num = startIndex;
            int num2 = num;
            int length = HttpRuleParser.GetNumberLength(input, num, false);
            if (length > 0x13)
            {
                return 0;
            }
            num += length;
            num += HttpRuleParser.GetWhitespaceLength(input, num);
            if ((num == input.Length) || (input[num] != '-'))
            {
                return 0;
            }
            num++;
            num += HttpRuleParser.GetWhitespaceLength(input, num);
            int num4 = num;
            int num5 = 0;
            if (num < input.Length)
            {
                num5 = HttpRuleParser.GetNumberLength(input, num, false);
                if (num5 > 0x13)
                {
                    return 0;
                }
                num += num5;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
            }
            if ((length == 0) && (num5 == 0))
            {
                return 0;
            }
            long result = 0;
            if ((length > 0) && !HeaderUtilities.TryParseInt64(input.Substring(num2, length), out result))
            {
                return 0;
            }
            long num7 = 0;
            if ((num5 > 0) && !HeaderUtilities.TryParseInt64(input.Substring(num4, num5), out num7))
            {
                return 0;
            }
            if (((length > 0) && (num5 > 0)) && (result > num7))
            {
                return 0;
            }
            parsedValue = new RangeItemHeaderValue((length == 0) ? null : new long?(result), (num5 == 0) ? null : new long?(num7));
            return (num - startIndex);
        }

        internal static int GetRangeItemListLength(string input, int startIndex, ICollection<RangeItemHeaderValue> rangeCollection)
        {
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            bool separatorFound = false;
            int num = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
            if (num == input.Length)
            {
                return 0;
            }
            RangeItemHeaderValue parsedValue = null;
            do
            {
                int num2 = GetRangeItemLength(input, num, out parsedValue);
                if (num2 == 0)
                {
                    return 0;
                }
                rangeCollection.Add(parsedValue);
                num += num2;
                num = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, num, true, out separatorFound);
                if ((num < input.Length) && !separatorFound)
                {
                    return 0;
                }
            }
            while (num != input.Length);
            return (num - startIndex);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeItemHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new RangeItemHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeItemHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (!this.from.HasValue)
            {
                return ("-" + this.to.Value.ToString(NumberFormatInfo.InvariantInfo));
            }
            if (!this.to.HasValue)
            {
                return (this.from.Value.ToString(NumberFormatInfo.InvariantInfo) + "-");
            }
            return (this.from.Value.ToString(NumberFormatInfo.InvariantInfo) + "-" + this.to.Value.ToString(NumberFormatInfo.InvariantInfo));
        }

        /// <returns>Returns <see cref="T:System.Int64" />.</returns>
        public long? From
        {
            get
            {
                return this.from;
            }
        }

        /// <returns>Returns <see cref="T:System.Int64" />.</returns>
        public long? To
        {
            get
            {
                return this.to;
            }
        }
    }
}

