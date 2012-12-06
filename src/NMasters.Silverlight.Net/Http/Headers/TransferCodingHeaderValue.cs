using System;
using System.Collections.Generic;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a transfer-coding header value.</summary>
    public class TransferCodingHeaderValue //: ICloneable
    {
        private ICollection<NameValueHeaderValue> parameters;
        private string value;

        internal TransferCodingHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> class.</summary>
        //protected TransferCodingHeaderValue(TransferCodingHeaderValue source)
        //{
        //    this.value = source.value;
        //    if (source.parameters != null)
        //    {
        //        foreach (NameValueHeaderValue value2 in source.parameters)
        //        {
        //            this.Parameters.Add((NameValueHeaderValue) ((ICloneable) value2).Clone());
        //        }
        //    }
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> class.</summary>
        public TransferCodingHeaderValue(string value)
        {
            HeaderUtilities.CheckValidToken(value, "value");
            this.value = value;
        }

        /// <summary>Determines whether the specified Object is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            TransferCodingHeaderValue value2 = obj as TransferCodingHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((string.Compare(this.value, value2.value, StringComparison.OrdinalIgnoreCase) == 0) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, value2.parameters));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (this.value.ToLowerInvariant().GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters));
        }

        internal static int GetTransferCodingLength(string input, int startIndex, Func<TransferCodingHeaderValue> transferCodingCreator, out TransferCodingHeaderValue parsedValue)
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
            TransferCodingHeaderValue value2 = null;
            if ((num2 < input.Length) && (input[num2] == ';'))
            {
                value2 = transferCodingCreator.Invoke();
                value2.value = str;
                num2++;
                int num3 = NameValueHeaderValue.GetNameValueListLength(input, num2, ';', value2.Parameters);
                if (num3 == 0)
                {
                    return 0;
                }
                parsedValue = value2;
                return ((num2 + num3) - startIndex);
            }
            value2 = transferCodingCreator.Invoke();
            value2.value = str;
            parsedValue = value2;
            return (num2 - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents transfer-coding header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid transfer-coding header value information.</exception>
        public static TransferCodingHeaderValue Parse(string input)
        {
            int index = 0;
            return (TransferCodingHeaderValue) TransferCodingHeaderParser.SingleValueParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new TransferCodingHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            return (this.value + NameValueHeaderValue.ToString(this.parameters, ';', true));
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out TransferCodingHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (TransferCodingHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (TransferCodingHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets the transfer-coding parameters.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.The transfer-coding parameters.</returns>
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

        /// <summary>Gets the transfer-coding value.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The transfer-coding value.</returns>
        public string Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

