using System;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class TimeSpanHeaderParser : BaseHeaderParser
    {
        internal static readonly TimeSpanHeaderParser Parser = new TimeSpanHeaderParser();

        private TimeSpanHeaderParser() : base(false)
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
            parsedValue = new TimeSpan(0, 0, result);
            return length;
        }

        public override string ToString(object value)
        {
            TimeSpan span = (TimeSpan) value;
            int totalSeconds = (int) span.TotalSeconds;
            return totalSeconds.ToString(NumberFormatInfo.InvariantInfo);
        }
    }
}

