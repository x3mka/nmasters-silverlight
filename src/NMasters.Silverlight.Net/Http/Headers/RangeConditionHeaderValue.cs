using System;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a header value which can either be a date/time or an entity-tag value.</summary>
    public class RangeConditionHeaderValue //: ICloneable
    {
        private DateTimeOffset? date;
        private EntityTagHeaderValue entityTag;

        private RangeConditionHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
        public RangeConditionHeaderValue(DateTimeOffset date)
        {
            this.date = new DateTimeOffset?(date);
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
        public RangeConditionHeaderValue(EntityTagHeaderValue entityTag)
        {
            if (entityTag == null)
            {
                throw new ArgumentNullException("entityTag");
            }
            this.entityTag = entityTag;
        }

        private RangeConditionHeaderValue(RangeConditionHeaderValue source)
        {
            this.entityTag = source.entityTag;
            this.date = source.date;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> class.</summary>
        public RangeConditionHeaderValue(string entityTag) : this(new EntityTagHeaderValue(entityTag))
        {
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            RangeConditionHeaderValue value2 = obj as RangeConditionHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (this.entityTag != null)
            {
                return this.entityTag.Equals(value2.entityTag);
            }
            return (value2.date.HasValue && (this.date.Value == value2.date.Value));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (this.entityTag == null)
            {
                return this.date.Value.GetHashCode();
            }
            return this.entityTag.GetHashCode();
        }

        internal static int GetRangeConditionLength(string input, int startIndex, out object parsedValue)
        {
            RangeConditionHeaderValue value3;
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || ((startIndex + 1) >= input.Length))
            {
                return 0;
            }
            int length = startIndex;
            DateTimeOffset minValue = DateTimeOffset.MinValue;
            EntityTagHeaderValue value2 = null;
            char ch = input[length];
            char ch2 = input[length + 1];
            if ((ch == '"') || (((ch == 'w') || (ch == 'W')) && (ch2 == '/')))
            {
                int num2 = EntityTagHeaderValue.GetEntityTagLength(input, length, out value2);
                if (num2 != 0)
                {
                    length += num2;
                    if (length == input.Length)
                    {
                        goto Label_0084;
                    }
                }
                return 0;
            }
            if (!HttpRuleParser.TryStringToDate(input.Substring(length), out minValue))
            {
                return 0;
            }
            length = input.Length;
        Label_0084:
            value3 = new RangeConditionHeaderValue();
            if (value2 == null)
            {
                value3.date = new DateTimeOffset?(minValue);
            }
            else
            {
                value3.entityTag = value2;
            }
            parsedValue = value3;
            return (length - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents range condition header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid range Condition header value information.</exception>
        public static RangeConditionHeaderValue Parse(string input)
        {
            int index = 0;
            return (RangeConditionHeaderValue) GenericHeaderParser.RangeConditionParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new RangeConditionHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.entityTag == null)
            {
                return HttpRuleParser.DateToString(this.date.Value);
            }
            return this.entityTag.ToString();
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out RangeConditionHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.RangeConditionParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (RangeConditionHeaderValue) obj2;
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

        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
        public EntityTagHeaderValue EntityTag
        {
            get
            {
                return this.entityTag;
            }
        }
    }
}

