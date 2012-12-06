using System.Globalization;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class Int32NumberHeaderParser : BaseHeaderParser
    {
        internal static readonly Int32NumberHeaderParser Parser = new Int32NumberHeaderParser();

        private Int32NumberHeaderParser() : base(false)
        {
        }

        protected override int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue)
        {
            parsedValue = null;
            int length = HttpRuleParser.GetNumberLength(value, startIndex, false);
            if ((length == 0) || (length > 10))
            {
                return 0;
            }
            int result = 0;
            if (!HeaderUtilities.TryParseInt32(value.Substring(startIndex, length), out result))
            {
                return 0;
            }
            parsedValue = result;
            return length;
        }

        public override string ToString(object value)
        {
            int num = (int) value;
            return num.ToString(NumberFormatInfo.InvariantInfo);
        }
    }
}

