using System;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents an entity-tag header value.</summary>
    public class EntityTagHeaderValue //: ICloneable
    {
        private static EntityTagHeaderValue any;
        private bool isWeak;
        private string tag;

        private EntityTagHeaderValue()
        {
        }

        private EntityTagHeaderValue(EntityTagHeaderValue source)
        {
            this.tag = source.tag;
            this.isWeak = source.isWeak;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> class.</summary>
        /// <param name="tag">A string that contains an <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" />.</param>
        public EntityTagHeaderValue(string tag) : this(tag, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> class.</summary>
        /// <param name="tag">A string that contains an  <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" />.</param>
        /// <param name="isWeak">A value that indicates if this entity-tag header is a weak validator. If the entity-tag header is weak validator, then <paramref name="isWeak" /> should be set to true. If the entity-tag header is a strong validator, then <paramref name="isWeak" /> should be set to false.</param>
        public EntityTagHeaderValue(string tag, bool isWeak)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "tag");
            }
            int length = 0;
            if ((HttpRuleParser.GetQuotedStringLength(tag, 0, out length) != HttpParseResult.Parsed) || (length != tag.Length))
            {
                throw new FormatException(SR.net_http_headers_invalid_etag_name);
            }
            this.tag = tag;
            this.isWeak = isWeak;
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            EntityTagHeaderValue value2 = obj as EntityTagHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((this.isWeak == value2.isWeak) && (string.CompareOrdinal(this.tag, value2.tag) == 0));
        }

        internal static int GetEntityTagLength(string input, int startIndex, out EntityTagHeaderValue parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            bool flag = false;
            int num = startIndex;
            switch (input[startIndex])
            {
                case '*':
                    parsedValue = Any;
                    num++;
                    goto Label_00BF;

                case 'W':
                case 'w':
                    num++;
                    if (((num + 2) >= input.Length) || (input[num] != '/'))
                    {
                        return 0;
                    }
                    flag = true;
                    num++;
                    num += HttpRuleParser.GetWhitespaceLength(input, num);
                    break;
            }
            int num2 = num;
            int length = 0;
            if (HttpRuleParser.GetQuotedStringLength(input, num, out length) != HttpParseResult.Parsed)
            {
                return 0;
            }
            parsedValue = new EntityTagHeaderValue();
            if (length == input.Length)
            {
                parsedValue.tag = input;
                parsedValue.isWeak = false;
            }
            else
            {
                parsedValue.tag = input.Substring(num2, length);
                parsedValue.isWeak = flag;
            }
            num += length;
        Label_00BF:
            num += HttpRuleParser.GetWhitespaceLength(input, num);
            return (num - startIndex);
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (this.tag.GetHashCode() ^ this.isWeak.GetHashCode());
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents entity tag header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid entity tag header value information.</exception>
        public static EntityTagHeaderValue Parse(string input)
        {
            int index = 0;
            return (EntityTagHeaderValue) GenericHeaderParser.SingleValueEntityTagParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    if (this == any)
        //    {
        //        return any;
        //    }
        //    return new EntityTagHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.isWeak)
            {
                return ("W/" + this.tag);
            }
            return this.tag;
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out EntityTagHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueEntityTagParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (EntityTagHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets the entity-tag header value.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
        public static EntityTagHeaderValue Any
        {
            get
            {
                if (any == null)
                {
                    any = new EntityTagHeaderValue();
                    any.tag = "*";
                    any.isWeak = false;
                }
                return any;
            }
        }

        /// <summary>Gets whether the entity-tag is prefaced by a weakness indicator.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the entity-tag is prefaced by a weakness indicator; otherwise, false.</returns>
        public bool IsWeak
        {
            get
            {
                return this.isWeak;
            }
        }

        /// <summary>Gets the opaque quoted string. </summary>
        /// <returns>Returns <see cref="T:System.String" />.An opaque quoted string.</returns>
        public string Tag
        {
            get
            {
                return this.tag;
            }
        }
    }
}

