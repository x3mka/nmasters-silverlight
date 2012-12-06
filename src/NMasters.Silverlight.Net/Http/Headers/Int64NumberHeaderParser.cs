using System.Globalization;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class Int64NumberHeaderParser : BaseHeaderParser
    {
        internal static readonly Int64NumberHeaderParser Parser = new Int64NumberHeaderParser();

        private Int64NumberHeaderParser() : base(false)
        {
        }

        protected override int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue)
        {
            parsedValue = null;
            int length = HttpRuleParser.GetNumberLength(value, startIndex, false);
            if ((length == 0) || (length > 0x13))
            {
                return 0;
            }
            long result = 0;
            if (!HeaderUtilities.TryParseInt64(value.Substring(startIndex, length), out result))
            {
                return 0;
            }
            parsedValue = result;
            return length;
        }

        public override string ToString(object value)
        {
            long num = (long) value;
            return num.ToString(NumberFormatInfo.InvariantInfo);
        }
    }
}

