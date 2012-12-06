using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a name/value pair.</summary>
    public class NameValueHeaderValue //: ICloneable
    {
        private static readonly Func<NameValueHeaderValue> defaultNameValueCreator = CreateNameValue;
        private string name;
        private string value;

        internal NameValueHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
        protected NameValueHeaderValue(NameValueHeaderValue source)
        {
            this.name = source.name;
            this.value = source.value;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
        /// <param name="name">The header name.</param>
        public NameValueHeaderValue(string name) : this(name, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> class.</summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        public NameValueHeaderValue(string name, string value)
        {
            CheckNameValueFormat(name, value);
            this.name = name;
            this.value = value;
        }

        private static void CheckNameValueFormat(string name, string value)
        {
            HeaderUtilities.CheckValidToken(name, "name");
            CheckValueFormat(value);
        }

        private static void CheckValueFormat(string value)
        {
            if (!string.IsNullOrEmpty(value) && (GetValueLength(value, 0) != value.Length))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { value }));
            }
        }

        private static NameValueHeaderValue CreateNameValue()
        {
            return new NameValueHeaderValue();
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            NameValueHeaderValue value2 = obj as NameValueHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (string.Compare(this.name, value2.name, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.value))
            {
                return string.IsNullOrEmpty(value2.value);
            }
            if (this.value[0] == '"')
            {
                return (string.CompareOrdinal(this.value, value2.value) == 0);
            }
            return (string.Compare(this.value, value2.value, StringComparison.OrdinalIgnoreCase) == 0);
        }

        internal static NameValueHeaderValue Find(ICollection<NameValueHeaderValue> values, string name)
        {
            if ((values != null) && (values.Count != 0))
            {
                foreach (NameValueHeaderValue value2 in values)
                {
                    if (string.Compare(value2.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return value2;
                    }
                }
            }
            return null;
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = this.name.ToLowerInvariant().GetHashCode();
            if (string.IsNullOrEmpty(this.value))
            {
                return hashCode;
            }
            if (this.value[0] == '"')
            {
                return (hashCode ^ this.value.GetHashCode());
            }
            return (hashCode ^ this.value.ToLowerInvariant().GetHashCode());
        }

        internal static int GetHashCode(ICollection<NameValueHeaderValue> values)
        {
            if ((values == null) || (values.Count == 0))
            {
                return 0;
            }
            int num = 0;
            foreach (NameValueHeaderValue value2 in values)
            {
                num ^= value2.GetHashCode();
            }
            return num;
        }

        internal static int GetNameValueLength(string input, int startIndex, out NameValueHeaderValue parsedValue)
        {
            return GetNameValueLength(input, startIndex, defaultNameValueCreator, out parsedValue);
        }

        internal static int GetNameValueLength(string input, int startIndex, Func<NameValueHeaderValue> nameValueCreator, out NameValueHeaderValue parsedValue)
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
            string str = input.Substring(startIndex, tokenLength);
            int num2 = startIndex + tokenLength;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if ((num2 == input.Length) || (input[num2] != '='))
            {
                parsedValue = nameValueCreator.Invoke();
                parsedValue.name = str;
                num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
                return (num2 - startIndex);
            }
            num2++;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            int valueLength = GetValueLength(input, num2);
            if (valueLength == 0)
            {
                return 0;
            }
            parsedValue = nameValueCreator.Invoke();
            parsedValue.name = str;
            parsedValue.value = input.Substring(num2, valueLength);
            num2 += valueLength;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            return (num2 - startIndex);
        }

        internal static int GetNameValueListLength(string input, int startIndex, char delimiter, ICollection<NameValueHeaderValue> nameValueCollection)
        {
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int num = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
            while (true)
            {
                NameValueHeaderValue parsedValue = null;
                int num2 = GetNameValueLength(input, num, defaultNameValueCreator, out parsedValue);
                if (num2 == 0)
                {
                    return 0;
                }
                nameValueCollection.Add(parsedValue);
                num += num2;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
                if ((num == input.Length) || (input[num] != delimiter))
                {
                    return (num - startIndex);
                }
                num++;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
            }
        }

        internal static int GetValueLength(string input, int startIndex)
        {
            if (startIndex >= input.Length)
            {
                return 0;
            }
            int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
            if ((tokenLength == 0) && (HttpRuleParser.GetQuotedStringLength(input, startIndex, out tokenLength) != HttpParseResult.Parsed))
            {
                return 0;
            }
            return tokenLength;
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents name value header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid name value header value information.</exception>
        public static NameValueHeaderValue Parse(string input)
        {
            int index = 0;
            return (NameValueHeaderValue) GenericHeaderParser.SingleValueNameValueParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new NameValueHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.value))
            {
                return (this.name + "=" + this.value);
            }
            return this.name;
        }

        internal static string ToString(ICollection<NameValueHeaderValue> values, char separator, bool leadingSeparator)
        {
            if ((values == null) || (values.Count == 0))
            {
                return null;
            }
            StringBuilder destination = new StringBuilder();
            ToString(values, separator, leadingSeparator, destination);
            return destination.ToString();
        }

        internal static void ToString(ICollection<NameValueHeaderValue> values, char separator, bool leadingSeparator, StringBuilder destination)
        {
            if ((values != null) && (values.Count != 0))
            {
                foreach (NameValueHeaderValue value2 in values)
                {
                    if (leadingSeparator || (destination.Length > 0))
                    {
                        destination.Append(separator);
                        destination.Append(' ');
                    }
                    destination.Append(value2.ToString());
                }
            }
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out NameValueHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueNameValueParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (NameValueHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets the header name.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The header name.</returns>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>Gets the header value.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The header value.</returns>
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                CheckValueFormat(value);
                this.value = value;
            }
        }
    }
}

