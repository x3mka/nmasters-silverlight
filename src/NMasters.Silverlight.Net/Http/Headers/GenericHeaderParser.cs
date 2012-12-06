using System;
using System.Collections;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal sealed class GenericHeaderParser : BaseHeaderParser
    {
        private IEqualityComparer comparer;
        internal static readonly HttpHeaderParser ContentDispositionParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(ContentDispositionHeaderValue.GetDispositionTypeLength));
        internal static readonly HttpHeaderParser ContentRangeParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(ContentRangeHeaderValue.GetContentRangeLength));
        private GetParsedValueLengthDelegate getParsedValueLength;
        internal static readonly HttpHeaderParser HostParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseHost), StringComparer.OrdinalIgnoreCase);
        internal static readonly HttpHeaderParser MailAddressParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseMailAddress));
        internal static readonly HttpHeaderParser MultipleValueAuthenticationParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(AuthenticationHeaderValue.GetAuthenticationLength));
        internal static readonly HttpHeaderParser MultipleValueEntityTagParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseMultipleEntityTags));
        internal static readonly HttpHeaderParser MultipleValueNameValueParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseNameValue));
        internal static readonly HttpHeaderParser MultipleValueNameValueWithParametersParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(NameValueWithParametersHeaderValue.GetNameValueWithParametersLength));
        internal static readonly HttpHeaderParser MultipleValueProductParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseProduct));
        internal static readonly HttpHeaderParser MultipleValueStringWithQualityParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(StringWithQualityHeaderValue.GetStringWithQualityLength));
        internal static readonly HttpHeaderParser MultipleValueViaParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(ViaHeaderValue.GetViaLength));
        internal static readonly HttpHeaderParser MultipleValueWarningParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(WarningHeaderValue.GetWarningLength));
        internal static readonly HttpHeaderParser RangeConditionParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(RangeConditionHeaderValue.GetRangeConditionLength));
        internal static readonly HttpHeaderParser RangeParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(RangeHeaderValue.GetRangeLength));
        internal static readonly HttpHeaderParser RetryConditionParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(RetryConditionHeaderValue.GetRetryConditionLength));
        internal static readonly HttpHeaderParser SingleValueAuthenticationParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(AuthenticationHeaderValue.GetAuthenticationLength));
        internal static readonly HttpHeaderParser SingleValueEntityTagParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseSingleEntityTag));
        internal static readonly HttpHeaderParser SingleValueNameValueParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseNameValue));
        internal static readonly HttpHeaderParser SingleValueNameValueWithParametersParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(NameValueWithParametersHeaderValue.GetNameValueWithParametersLength));
        internal static readonly HttpHeaderParser SingleValueProductParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseProduct));
        internal static readonly HttpHeaderParser SingleValueStringWithQualityParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(StringWithQualityHeaderValue.GetStringWithQualityLength));
        internal static readonly HttpHeaderParser SingleValueViaParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(ViaHeaderValue.GetViaLength));
        internal static readonly HttpHeaderParser SingleValueWarningParser = new GenericHeaderParser(false, new GetParsedValueLengthDelegate(WarningHeaderValue.GetWarningLength));
        internal static readonly HttpHeaderParser TokenListParser = new GenericHeaderParser(true, new GetParsedValueLengthDelegate(GenericHeaderParser.ParseTokenList), StringComparer.OrdinalIgnoreCase);

        private GenericHeaderParser(bool supportsMultipleValues, GetParsedValueLengthDelegate getParsedValueLength) : this(supportsMultipleValues, getParsedValueLength, null)
        {
        }

        private GenericHeaderParser(bool supportsMultipleValues, GetParsedValueLengthDelegate getParsedValueLength, IEqualityComparer comparer) : base(supportsMultipleValues)
        {
            this.getParsedValueLength = getParsedValueLength;
            this.comparer = comparer;
        }

        protected override int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue)
        {
            return this.getParsedValueLength(value, startIndex, out parsedValue);
        }

        private static int ParseHost(string value, int startIndex, out object parsedValue)
        {
            string host = null;
            int num = HttpRuleParser.GetHostLength(value, startIndex, false, out host);
            parsedValue = host;
            return num;
        }

        private static int ParseMailAddress(string value, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (HttpRuleParser.ContainsInvalidNewLine(value, startIndex))
            {
                return 0;
            }
            string str = value.Substring(startIndex);
            //SL fixes
            return 0;
            //if (!HeaderUtilities.IsValidEmailAddress(str))
            //{
            //    return 0;
            //}
            //parsedValue = str;
            //return str.Length;
        }

        private static int ParseMultipleEntityTags(string value, int startIndex, out object parsedValue)
        {
            EntityTagHeaderValue value2 = null;
            int num = EntityTagHeaderValue.GetEntityTagLength(value, startIndex, out value2);
            parsedValue = value2;
            return num;
        }

        private static int ParseNameValue(string value, int startIndex, out object parsedValue)
        {
            NameValueHeaderValue value2 = null;
            int num = NameValueHeaderValue.GetNameValueLength(value, startIndex, out value2);
            parsedValue = value2;
            return num;
        }

        private static int ParseProduct(string value, int startIndex, out object parsedValue)
        {
            ProductHeaderValue value2 = null;
            int num = ProductHeaderValue.GetProductLength(value, startIndex, out value2);
            parsedValue = value2;
            return num;
        }

        private static int ParseSingleEntityTag(string value, int startIndex, out object parsedValue)
        {
            EntityTagHeaderValue value2 = null;
            parsedValue = null;
            int num = EntityTagHeaderValue.GetEntityTagLength(value, startIndex, out value2);
            if (value2 == EntityTagHeaderValue.Any)
            {
                return 0;
            }
            parsedValue = value2;
            return num;
        }

        private static int ParseTokenList(string value, int startIndex, out object parsedValue)
        {
            int tokenLength = HttpRuleParser.GetTokenLength(value, startIndex);
            parsedValue = value.Substring(startIndex, tokenLength);
            return tokenLength;
        }

        public override IEqualityComparer Comparer
        {
            get
            {
                return this.comparer;
            }
        }

        private delegate int GetParsedValueLengthDelegate(string value, int startIndex, out object parsedValue);
    }
}

