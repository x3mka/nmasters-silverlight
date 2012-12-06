namespace NMasters.Silverlight.Net.Http.Headers
{
    internal abstract class BaseHeaderParser : HttpHeaderParser
    {
        protected BaseHeaderParser(bool supportsMultipleValues) : base(supportsMultipleValues)
        {
        }

        protected abstract int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue);
        public sealed override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(value) || (index == value.Length))
            {
                return base.SupportsMultipleValues;
            }
            bool separatorFound = false;
            int startIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, index, base.SupportsMultipleValues, out separatorFound);
            if (separatorFound && !base.SupportsMultipleValues)
            {
                return false;
            }
            if (startIndex == value.Length)
            {
                if (base.SupportsMultipleValues)
                {
                    index = startIndex;
                }
                return base.SupportsMultipleValues;
            }
            object obj2 = null;
            int num2 = this.GetParsedValueLength(value, startIndex, storeValue, out obj2);
            if (num2 == 0)
            {
                return false;
            }
            startIndex += num2;
            startIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(value, startIndex, base.SupportsMultipleValues, out separatorFound);
            if ((separatorFound && !base.SupportsMultipleValues) || (!separatorFound && (startIndex < value.Length)))
            {
                return false;
            }
            index = startIndex;
            parsedValue = obj2;
            return true;
        }
    }
}

