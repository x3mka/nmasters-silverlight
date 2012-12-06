using System;
using System.Collections;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Helpers;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal abstract class HttpHeaderParser
    {
        internal const string DefaultSeparator = ", ";
        private string separator;
        private bool supportsMultipleValues;

        protected HttpHeaderParser(bool supportsMultipleValues)
        {
            this.supportsMultipleValues = supportsMultipleValues;
            if (supportsMultipleValues)
            {
                this.separator = ", ";
            }
        }

        protected HttpHeaderParser(bool supportsMultipleValues, string separator)
        {
            this.supportsMultipleValues = supportsMultipleValues;
            this.separator = separator;
        }

        public object ParseValue(string value, object storeValue, ref int index)
        {
            object parsedValue = null;
            if (!this.TryParseValue(value, storeValue, ref index, out parsedValue))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { (value == null) ? "<null>" : value.Substring(index) }));
            }
            return parsedValue;
        }

        public virtual string ToString(object value)
        {
            return value.ToString();
        }

        public abstract bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue);

        public virtual IEqualityComparer Comparer
        {
            get
            {
                return null;
            }
        }

        public string Separator
        {
            get
            {
                return this.separator;
            }
        }

        public bool SupportsMultipleValues
        {
            get
            {
                return this.supportsMultipleValues;
            }
        }
    }
}

