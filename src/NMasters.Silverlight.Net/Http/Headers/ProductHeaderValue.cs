using System;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a product token in header value.</summary>
    public class ProductHeaderValue //: ICloneable
    {
        private string name;
        private string version;

        private ProductHeaderValue()
        {
        }

        private ProductHeaderValue(ProductHeaderValue source)
        {
            this.name = source.name;
            this.version = source.version;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> class.</summary>
        public ProductHeaderValue(string name) : this(name, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> class.</summary>
        public ProductHeaderValue(string name, string version)
        {
            HeaderUtilities.CheckValidToken(name, "name");
            if (!string.IsNullOrEmpty(version))
            {
                HeaderUtilities.CheckValidToken(version, "version");
                this.version = version;
            }
            this.name = name;
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            ProductHeaderValue value2 = obj as ProductHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((string.Compare(this.name, value2.name, StringComparison.OrdinalIgnoreCase) == 0) && (string.Compare(this.version, value2.version, StringComparison.OrdinalIgnoreCase) == 0));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = this.name.ToLowerInvariant().GetHashCode();
            if (!string.IsNullOrEmpty(this.version))
            {
                hashCode ^= this.version.ToLowerInvariant().GetHashCode();
            }
            return hashCode;
        }

        internal static int GetProductLength(string input, int startIndex, out ProductHeaderValue parsedValue)
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
            ProductHeaderValue value2 = new ProductHeaderValue {
                name = input.Substring(startIndex, tokenLength)
            };
            int num2 = startIndex + tokenLength;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if ((num2 == input.Length) || (input[num2] != '/'))
            {
                parsedValue = value2;
                return (num2 - startIndex);
            }
            num2++;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            int length = HttpRuleParser.GetTokenLength(input, num2);
            if (length == 0)
            {
                return 0;
            }
            value2.version = input.Substring(num2, length);
            num2 += length;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            parsedValue = value2;
            return (num2 - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents product header value information.</param>
        public static ProductHeaderValue Parse(string input)
        {
            int index = 0;
            return (ProductHeaderValue) GenericHeaderParser.SingleValueProductParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new ProductHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.version))
            {
                return this.name;
            }
            return (this.name + "/" + this.version);
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out ProductHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueProductParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (ProductHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets the name of the product token.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The name of the product token.</returns>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>Gets the version of the product token.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The version of the product token. </returns>
        public string Version
        {
            get
            {
                return this.version;
            }
        }
    }
}

