using System;
using System.Collections.Generic;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a name/value pair with parameters.</summary>
    public class NameValueWithParametersHeaderValue : NameValueHeaderValue//, ICloneable
    {
        private static readonly Func<NameValueHeaderValue> nameValueCreator = CreateNameValue;
        private ICollection<NameValueHeaderValue> parameters;

        internal NameValueWithParametersHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
        //protected NameValueWithParametersHeaderValue(NameValueWithParametersHeaderValue source) : base(source)
        //{
        //    if (source.parameters != null)
        //    {
        //        foreach (NameValueHeaderValue value2 in source.parameters)
        //        {
        //            this.Parameters.Add((NameValueHeaderValue) ((ICloneable) value2).Clone());
        //        }
        //    }
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
        public NameValueWithParametersHeaderValue(string name) : base(name)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> class.</summary>
        public NameValueWithParametersHeaderValue(string name, string value) : base(name, value)
        {
        }

        private static NameValueHeaderValue CreateNameValue()
        {
            return new NameValueWithParametersHeaderValue();
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }
            NameValueWithParametersHeaderValue value2 = obj as NameValueWithParametersHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, value2.parameters);
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters));
        }

        internal static int GetNameValueWithParametersLength(string input, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            NameValueHeaderValue value2 = null;
            int num = NameValueHeaderValue.GetNameValueLength(input, startIndex, nameValueCreator, out value2);
            if (num == 0)
            {
                return 0;
            }
            int num2 = startIndex + num;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            NameValueWithParametersHeaderValue value3 = value2 as NameValueWithParametersHeaderValue;
            if ((num2 < input.Length) && (input[num2] == ';'))
            {
                num2++;
                int num3 = NameValueHeaderValue.GetNameValueListLength(input, num2, ';', value3.Parameters);
                if (num3 == 0)
                {
                    return 0;
                }
                parsedValue = value3;
                return ((num2 + num3) - startIndex);
            }
            parsedValue = value3;
            return (num2 - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents name value with parameter header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid name value with parameter header value information.</exception>
        public new static NameValueWithParametersHeaderValue Parse(string input)
        {
            int index = 0;
            return (NameValueWithParametersHeaderValue) GenericHeaderParser.SingleValueNameValueWithParametersParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new NameValueWithParametersHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            return (base.ToString() + NameValueHeaderValue.ToString(this.parameters, ';', true));
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.NameValueWithParametersHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out NameValueWithParametersHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueNameValueWithParametersParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (NameValueWithParametersHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public ICollection<NameValueHeaderValue> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new ObjectCollection<NameValueHeaderValue>();
                }
                return this.parameters;
            }
        }
    }
}

