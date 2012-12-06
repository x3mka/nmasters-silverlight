using System;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a header value which can either be a date/time or a timespan value.</summary>
    public class RetryConditionHeaderValue //: ICloneable
    {
        private DateTimeOffset? date;
        private TimeSpan? delta;

        private RetryConditionHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> class.</summary>
        public RetryConditionHeaderValue(DateTimeOffset date)
        {
            this.date = new DateTimeOffset?(date);
        }

        private RetryConditionHeaderValue(RetryConditionHeaderValue source)
        {
            this.delta = source.delta;
            this.date = source.date;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> class.</summary>
        public RetryConditionHeaderValue(TimeSpan delta)
        {
            if (delta.TotalSeconds > 2147483647.0)
            {
                throw new ArgumentOutOfRangeException("delta");
            }
            this.delta = new TimeSpan?(delta);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            RetryConditionHeaderValue value2 = obj as RetryConditionHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (this.delta.HasValue)
            {
                return (value2.delta.HasValue && (this.delta.Value == value2.delta.Value));
            }
            return (value2.date.HasValue && (this.date.Value == value2.date.Value));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (!this.delta.HasValue)
            {
                return this.date.Value.GetHashCode();
            }
            return this.delta.Value.GetHashCode();
        }

        internal static int GetRetryConditionLength(string input, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int num = startIndex;
            DateTimeOffset minValue = DateTimeOffset.MinValue;
            int result = -1;
            char ch = input[num];
            if ((ch >= '0') && (ch <= '9'))
            {
                int num3 = num;
                int length = HttpRuleParser.GetNumberLength(input, num, false);
                if ((length == 0) || (length > 10))
                {
                    return 0;
                }
                num += length;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
                if ((num != input.Length) || !HeaderUtilities.TryParseInt32(input.Substring(num3, length), out result))
                {
                    return 0;
                }
            }
            else
            {
                if (!HttpRuleParser.TryStringToDate(input.Substring(num), out minValue))
                {
                    return 0;
                }
                num = input.Length;
            }
            RetryConditionHeaderValue value2 = new RetryConditionHeaderValue();
            if (result == -1)
            {
                value2.date = new DateTimeOffset?(minValue);
            }
            else
            {
                value2.delta = new TimeSpan(0, 0, result);
            }
            parsedValue = value2;
            return (num - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents retry condition header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid retry condition header value information.</exception>
        public static RetryConditionHeaderValue Parse(string input)
        {
            int index = 0;
            return (RetryConditionHeaderValue) GenericHeaderParser.RetryConditionParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new RetryConditionHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.delta.HasValue)
            {
                int totalSeconds = (int) this.delta.Value.TotalSeconds;
                return totalSeconds.ToString(NumberFormatInfo.InvariantInfo);
            }
            return HttpRuleParser.DateToString(this.date.Value);
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out RetryConditionHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.RetryConditionParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (RetryConditionHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
        public DateTimeOffset? Date
        {
            get
            {
                return this.date;
            }
        }

        /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
        public TimeSpan? Delta
        {
            get
            {
                return this.delta;
            }
        }
    }
}

