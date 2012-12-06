using System;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a content-type header value with an additional quality.</summary>
    public sealed class MediaTypeWithQualityHeaderValue : MediaTypeHeaderValue//, ICloneable
    {
        internal MediaTypeWithQualityHeaderValue()
        {
        }

        //private MediaTypeWithQualityHeaderValue(MediaTypeWithQualityHeaderValue source) : base(source)
        //{
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> class.</summary>
        public MediaTypeWithQualityHeaderValue(string mediaType) : base(mediaType)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> class.</summary>
        public MediaTypeWithQualityHeaderValue(string mediaType, double quality) : base(mediaType)
        {
            this.Quality = new double?(quality);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents media type with quality header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid media type with quality header value information.</exception>
        public new static MediaTypeWithQualityHeaderValue Parse(string input)
        {
            int index = 0;
            return (MediaTypeWithQualityHeaderValue) MediaTypeHeaderParser.SingleValueWithQualityParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new MediaTypeWithQualityHeaderValue(this);
        //}

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out MediaTypeWithQualityHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (MediaTypeHeaderParser.SingleValueWithQualityParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (MediaTypeWithQualityHeaderValue) obj2;
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

