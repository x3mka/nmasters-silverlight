using System;
using System.Collections.Generic;
using System.Globalization;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents a media-type as defined in the RFC 2616.</summary>
    public class MediaTypeHeaderValue //: ICloneable
    {
        private const string charSet = "charset";
        private string mediaType;
        private ICollection<NameValueHeaderValue> parameters;

        internal MediaTypeHeaderValue()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> class.</summary>
        //protected MediaTypeHeaderValue(MediaTypeHeaderValue source)
        //{
        //    this.mediaType = source.mediaType;
        //    if (source.parameters != null)
        //    {
        //        foreach (NameValueHeaderValue value2 in source.parameters)
        //        {
        //            this.Parameters.Add((NameValueHeaderValue) ((ICloneable) value2).Clone());
        //        }
        //    }
        //}

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> class.</summary>
        public MediaTypeHeaderValue(string mediaType)
        {
            CheckMediaTypeFormat(mediaType, "mediaType");
            this.mediaType = mediaType;
        }

        private static void CheckMediaTypeFormat(string mediaType, string parameterName)
        {
            string str;
            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
            }
            if ((GetMediaTypeExpressionLength(mediaType, 0, out str) == 0) || (str.Length != mediaType.Length))
            {
                throw new FormatException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { mediaType }));
            }
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            MediaTypeHeaderValue value2 = obj as MediaTypeHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            return ((string.Compare(this.mediaType, value2.mediaType, StringComparison.OrdinalIgnoreCase) == 0) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.parameters, value2.parameters));
        }

        /// <summary>Serves as a hash function for an <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (this.mediaType.ToLowerInvariant().GetHashCode() ^ NameValueHeaderValue.GetHashCode(this.parameters));
        }

        private static int GetMediaTypeExpressionLength(string input, int startIndex, out string mediaType)
        {
            mediaType = null;
            int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
            if (tokenLength == 0)
            {
                return 0;
            }
            int num2 = startIndex + tokenLength;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            if ((num2 >= input.Length) || (input[num2] != '/'))
            {
                return 0;
            }
            num2++;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            int length = HttpRuleParser.GetTokenLength(input, num2);
            if (length == 0)
            {
                return 0;
            }
            int num4 = (num2 + length) - startIndex;
            if (((tokenLength + length) + 1) == num4)
            {
                mediaType = input.Substring(startIndex, num4);
                return num4;
            }
            mediaType = input.Substring(startIndex, tokenLength) + "/" + input.Substring(num2, length);
            return num4;
        }

        internal static int GetMediaTypeLength(string input, int startIndex, Func<MediaTypeHeaderValue> mediaTypeCreator, out MediaTypeHeaderValue parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            string mediaType = null;
            int num = GetMediaTypeExpressionLength(input, startIndex, out mediaType);
            if (num == 0)
            {
                return 0;
            }
            int num2 = startIndex + num;
            num2 += HttpRuleParser.GetWhitespaceLength(input, num2);
            MediaTypeHeaderValue value2 = null;
            if ((num2 < input.Length) && (input[num2] == ';'))
            {
                value2 = mediaTypeCreator.Invoke();
                value2.mediaType = mediaType;
                num2++;
                int num3 = NameValueHeaderValue.GetNameValueListLength(input, num2, ';', value2.Parameters);
                if (num3 == 0)
                {
                    return 0;
                }
                parsedValue = value2;
                return ((num2 + num3) - startIndex);
            }
            value2 = mediaTypeCreator.Invoke();
            value2.mediaType = mediaType;
            parsedValue = value2;
            return (num2 - startIndex);
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents media type header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid media type header value information.</exception>
        public static MediaTypeHeaderValue Parse(string input)
        {
            int index = 0;
            return (MediaTypeHeaderValue) MediaTypeHeaderParser.SingleValueParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new MediaTypeHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            return (this.mediaType + NameValueHeaderValue.ToString(this.parameters, ';', true));
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out MediaTypeHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (MediaTypeHeaderParser.SingleValueParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (MediaTypeHeaderValue) obj2;
                return true;
            }
            return false;
        }

        /// <summary>Gets or sets the character set.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The character set.</returns>
        public string CharSet
        {
            get
            {
                NameValueHeaderValue value2 = NameValueHeaderValue.Find(this.parameters, "charset");
                if (value2 != null)
                {
                    return value2.Value;
                }
                return null;
            }
            set
            {
                NameValueHeaderValue item = NameValueHeaderValue.Find(this.parameters, "charset");
                if (string.IsNullOrEmpty(value))
                {
                    if (item != null)
                    {
                        this.parameters.Remove(item);
                    }
                }
                else if (item != null)
                {
                    item.Value = value;
                }
                else
                {
                    this.Parameters.Add(new NameValueHeaderValue("charset", value));
                }
            }
        }

        /// <summary>Gets or sets the media-type header value.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The media-type header value.</returns>
        public string MediaType
        {
            get
            {
                return this.mediaType;
            }
            set
            {
                CheckMediaTypeFormat(value, "value");
                this.mediaType = value;
            }
        }

        /// <summary>Gets or sets the media-type header value parameters.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.The media-type header value parameters.</returns>
        public ICollection<NameValueHeaderValue> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new ObjectCollection<NameValueHeaderValue>();
                }
                return this.parameters;
            }
        }
    }
}

