using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class ProductInfoHeaderParser : HttpHeaderParser
    {
        internal static readonly ProductInfoHeaderParser MultipleValueParser = new ProductInfoHeaderParser(true);
        private const string separator = " ";
        internal static readonly ProductInfoHeaderParser SingleValueParser = new ProductInfoHeaderParser(false);

        private ProductInfoHeaderParser(bool supportsMultipleValues) : base(supportsMultipleValues, " ")
        {
        }

        public override bool TryParseValue(string value, object storeValue, ref int index, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(value) || (index == value.Length))
            {
                return false;
            }
            int startIndex = index + HttpRuleParser.GetWhitespaceLength(value, index);
            if (startIndex == value.Length)
            {
                return false;
            }
            ProductInfoHeaderValue value2 = null;
            int num2 = ProductInfoHeaderValue.GetProductInfoLength(value, startIndex, out value2);
            if (num2 == 0)
            {
                return false;
            }
            startIndex += num2;
            if (startIndex < value.Length)
            {
                char ch = value[startIndex - 1];
                if ((ch != ' ') && (ch != '\t'))
                {
                    return false;
                }
            }
            index = startIndex;
            parsedValue = value2;
            return true;
        }
    }
}

