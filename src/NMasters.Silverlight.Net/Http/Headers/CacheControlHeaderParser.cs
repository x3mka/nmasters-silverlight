namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class CacheControlHeaderParser : BaseHeaderParser
    {
        internal static readonly CacheControlHeaderParser Parser = new CacheControlHeaderParser();

        private CacheControlHeaderParser() : base(true)
        {
        }

        protected override int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue)
        {
            CacheControlHeaderValue value2 = storeValue as CacheControlHeaderValue;
            int num = CacheControlHeaderValue.GetCacheControlLength(value, startIndex, value2, out value2);
            parsedValue = value2;
            return num;
        }
    }
}

