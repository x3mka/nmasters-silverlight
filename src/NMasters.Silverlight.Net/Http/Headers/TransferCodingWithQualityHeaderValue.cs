using System;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a transfer-coding header value with optional quality.</summary>
    public sealed class TransferCodingWithQualityHeaderValue : TransferCodingHeaderValue//, ICloneable
    {
        internal TransferCodingWithQualityHeaderValue()
        {
        }

        //private TransferCodingWithQualityHeaderValue(TransferCodingWithQualityHeaderValue source) : base(source)
        //{
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> class.</summary>
        public TransferCodingWithQualityHeaderValue(string value) : base(value)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> class.</summary>
        public TransferCodingWithQualityHeaderValue(string value, double quality) : base(value)
        {
            this.Quality = new double?(quality);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents transfer-coding value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid transfer-coding with quality header value information.</exception>
        public new static TransferCodingWithQualityHeaderValue Parse(string input)
        {
            int index = 0;
            return (TransferCodingWithQualityHeaderValue) TransferCodingHeaderParser.SingleValueWithQualityParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new TransferCodingWithQualityHeaderValue(this);
        //}

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.TransferCodingWithQualityHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out TransferCodingWithQualityHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (TransferCodingHeaderParser.SingleValueWithQualityParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (TransferCodingWithQualityHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <returns>Returns <see cref="T:System.Double" />.</returns>
        public double? Quality
        {
            get
            {
                return HeaderUtilities.GetQuality(base.Parameters);
            }
            set
            {
                HeaderUtilities.SetQuality(base.Parameters, value);
            }
        }
    }
}

