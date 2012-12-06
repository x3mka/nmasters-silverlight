using System;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class TransferCodingHeaderParser : BaseHeaderParser
    {
        internal static readonly TransferCodingHeaderParser MultipleValueParser = new TransferCodingHeaderParser(true, CreateTransferCoding);
        internal static readonly TransferCodingHeaderParser MultipleValueWithQualityParser = new TransferCodingHeaderParser(true, CreateTransferCodingWithQuality);
        internal static readonly TransferCodingHeaderParser SingleValueParser = new TransferCodingHeaderParser(false, CreateTransferCoding);
        internal static readonly TransferCodingHeaderParser SingleValueWithQualityParser = new TransferCodingHeaderParser(false, CreateTransferCodingWithQuality);
        private Func<TransferCodingHeaderValue> transferCodingCreator;

        private TransferCodingHeaderParser(bool supportsMultipleValues, Func<TransferCodingHeaderValue> transferCodingCreator) : base(supportsMultipleValues)
        {
            this.transferCodingCreator = transferCodingCreator;
        }

        private static TransferCodingHeaderValue CreateTransferCoding()
        {
            return new TransferCodingHeaderValue();
        }

        private static TransferCodingHeaderValue CreateTransferCodingWithQuality()
        {
            return new TransferCodingWithQualityHeaderValue();
        }

        protected override int GetParsedValueLength(string value, int startIndex, object storeValue, out object parsedValue)
        {
            TransferCodingHeaderValue value2 = null;
            int num = TransferCodingHeaderValue.GetTransferCodingLength(value, startIndex, this.transferCodingCreator, out value2);
            parsedValue = value2;
            return num;
        }
    }
}

