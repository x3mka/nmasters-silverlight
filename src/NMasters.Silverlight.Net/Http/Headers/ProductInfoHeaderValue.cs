using System;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a value which can either be a product or a comment.</summary>
    public class ProductInfoHeaderValue //: ICloneable
    {
        private string comment;
        private ProductHeaderValue product;

        private ProductInfoHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> class.</summary>
        public ProductInfoHeaderValue(ProductHeaderValue product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }
            this.product = product;
        }

        private ProductInfoHeaderValue(ProductInfoHeaderValue source)
        {
            this.product = source.product;
            this.comment = source.comment;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> class.</summary>
        public ProductInfoHeaderValue(string comment)
        {
            HeaderUtilities.CheckValidComment(comment, "comment");
            this.comment = comment;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> class.</summary>
        public ProductInfoHeaderValue(string productName, string productVersion) : this(new ProductHeaderValue(productName, productVersion))
        {
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            ProductInfoHeaderValue value2 = obj as ProductInfoHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (this.product == null)
            {
                return (string.CompareOrdinal(this.comment, value2.comment) == 0);
            }
            return this.product.Equals(value2.product);
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (this.product == null)
            {
                return this.comment.GetHashCode();
            }
            return this.product.GetHashCode();
        }

        internal static int GetProductInfoLength(string input, int startIndex, out ProductInfoHeaderValue parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int num = startIndex;
            string str = null;
            ProductHeaderValue value2 = null;
            if (input[num] == '(')
            {
                int length = 0;
                if (HttpRuleParser.GetCommentLength(input, num, out length) != HttpParseResult.Parsed)
                {
                    return 0;
                }
                str = input.Substring(num, length);
                num += length;
                num += HttpRuleParser.GetWhitespaceLength(input, num);
            }
            else
            {
                int num3 = ProductHeaderValue.GetProductLength(input, num, out value2);
                if (num3 == 0)
                {
                    return 0;
                }
                num += num3;
            }
            parsedValue = new ProductInfoHeaderValue();
            parsedValue.product = value2;
            parsedValue.comment = str;
            return (num - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents product info header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid product info header value information.</exception>
        public static ProductInfoHeaderValue Parse(string input)
        {
            int index = 0;
            object obj2 = ProductInfoHeaderParser.SingleValueParser.ParseValue(input, null, ref index);
            if (index < input.Length)
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { input.Substring(index) }));
            }
            return (ProductInfoHeaderValue) obj2;
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new ProductInfoHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (this.product == null)
            {
                return this.comment;
            }
            return this.product.ToString();
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductInfoHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out ProductInfoHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (!ProductInfoHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out obj2))
            {
                return false;
            }
            if (index < input.Length)
            {
                return false;
            }
            parsedValue = (ProductInfoHeaderValue) obj2;
            return true;
        }

        /// <returns>Returns <see cref="T:System.String" />.</returns>
        public string Comment
        {
            get
            {
                return this.comment;
            }
        }

        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ProductHeaderValue" />.</returns>
        public ProductHeaderValue Product
        {
            get
            {
                return this.product;
            }
        }
    }
}

