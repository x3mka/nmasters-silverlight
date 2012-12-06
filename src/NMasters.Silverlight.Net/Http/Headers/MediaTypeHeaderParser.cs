using System;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class MediaTypeHeaderParser : BaseHeaderParser
    {
        private Func<MediaTypeHeaderValue> mediaTypeCreator;
        internal static readonly MediaTypeHeaderParser MultipleValuesParser = new MediaTypeHeaderParser(true, CreateMediaTypeWithQuality);
        internal static readonly MediaTypeHeaderParser SingleValueParser = new MediaTypeHeaderParser(false, CreateMediaType);
        internal static readonly MediaTypeHeaderParser SingleValueWithQualityParser = new MediaTypeHeaderParser(false, CreateMediaTypeWithQuality);
        private bool supportsMultipleValues;

        private MediaTypeHeaderParser(bool supportsMultipleValues, Func<MediaTypeHeaderValue> mediaTypeCreator) : base(supportsMultipleValues)
        {
            this.supportsMultipleValues = supportsMultipleValues;
            this.mediaTypeCreator = mediaTypeCreator;
        }

        private static MediaTypeHeaderValue CreateMediaType()
        {
            return new MediaTypeHeaderValue();
        }

        private static MediaTypeHeaderValue CreateMediaTypeWithQuality()
        {
            return new MediaTypeWithQualityHeaderValue();
        }

        protected override int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue)
        {
            MediaTypeHeaderValue value2 = null;
            int num = MediaTypeHeaderValue.GetMediaTypeLength(value, startIndex, this.mediaTypeCreator, out value2);
            parsedValue = value2;
            return num;
        }
    }
}

