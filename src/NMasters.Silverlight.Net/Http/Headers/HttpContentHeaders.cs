using System;
using System.Collections.Generic;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the collection of Content Headers as defined in RFC 2616.</summary>
    public sealed class HttpContentHeaders : HttpHeaders
    {
        private HttpHeaderValueCollection<string> allow;
        private Func<long?> calculateLengthFunc;
        private HttpHeaderValueCollection<string> contentEncoding;
        private HttpHeaderValueCollection<string> contentLanguage;
        private bool contentLengthSet;
        private static readonly HashSet<string> invalidHeaders;
        private static readonly Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>(StringComparer.OrdinalIgnoreCase);

        static HttpContentHeaders()
        {
            parserStore.Add("Allow", GenericHeaderParser.TokenListParser);
            parserStore.Add("Content-Disposition", GenericHeaderParser.ContentDispositionParser);
            parserStore.Add("Content-Encoding", GenericHeaderParser.TokenListParser);
            parserStore.Add("Content-Language", GenericHeaderParser.TokenListParser);
            parserStore.Add("Content-Length", Int64NumberHeaderParser.Parser);
            parserStore.Add("Content-Location", UriHeaderParser.RelativeOrAbsoluteUriParser);
            parserStore.Add("Content-MD5", ByteArrayHeaderParser.Parser);
            parserStore.Add("Content-Range", GenericHeaderParser.ContentRangeParser);
            parserStore.Add("Content-Type", MediaTypeHeaderParser.SingleValueParser);
            parserStore.Add("Expires", DateHeaderParser.Parser);
            parserStore.Add("Last-Modified", DateHeaderParser.Parser);
            invalidHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HttpRequestHeaders.AddKnownHeaders(invalidHeaders);
            HttpResponseHeaders.AddKnownHeaders(invalidHeaders);
            HttpGeneralHeaders.AddKnownHeaders(invalidHeaders);
        }

        internal HttpContentHeaders(Func<long?> calculateLengthFunc)
        {
            this.calculateLengthFunc = calculateLengthFunc;
            base.SetConfiguration(parserStore, invalidHeaders);
        }

        internal static void AddKnownHeaders(HashSet<string> headerSet)
        {
            headerSet.Add("Allow");
            headerSet.Add("Content-Disposition");
            headerSet.Add("Content-Encoding");
            headerSet.Add("Content-Language");
            headerSet.Add("Content-Length");
            headerSet.Add("Content-Location");
            headerSet.Add("Content-MD5");
            headerSet.Add("Content-Range");
            headerSet.Add("Content-Type");
            headerSet.Add("Expires");
            headerSet.Add("Last-Modified");
        }

        /// <summary>Gets the value of the Allow content header on an HTTP response. </summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.The value of the Allow header on an HTTP response.</returns>
        public ICollection<string> Allow
        {
            get
            {
                if (this.allow == null)
                {
                    this.allow = new HttpHeaderValueCollection<string>("Allow", this, HeaderUtilities.TokenValidator);
                }
                return this.allow;
            }
        }

        /// <summary>Gets the value of the Content-Disposition content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentDispositionHeaderValue" />.The value of the Content-Disposition content header on an HTTP response.</returns>
        public ContentDispositionHeaderValue ContentDisposition
        {
            get
            {
                return (ContentDispositionHeaderValue) base.GetParsedValues("Content-Disposition");
            }
            set
            {
                base.SetOrRemoveParsedValue("Content-Disposition", value);
            }
        }

        /// <summary>Gets the value of the Content-Encoding content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.The value of the Content-Encoding content header on an HTTP response.</returns>
        public ICollection<string> ContentEncoding
        {
            get
            {
                if (this.contentEncoding == null)
                {
                    this.contentEncoding = new HttpHeaderValueCollection<string>("Content-Encoding", this, HeaderUtilities.TokenValidator);
                }
                return this.contentEncoding;
            }
        }

        /// <summary>Gets the value of the Content-Language content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.The value of the Content-Language content header on an HTTP response.</returns>
        public ICollection<string> ContentLanguage
        {
            get
            {
                if (this.contentLanguage == null)
                {
                    this.contentLanguage = new HttpHeaderValueCollection<string>("Content-Language", this, HeaderUtilities.TokenValidator);
                }
                return this.contentLanguage;
            }
        }

        /// <summary>Gets or sets the value of the Content-Length content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Int64" />.The value of the Content-Length content header on an HTTP response.</returns>
        public long? ContentLength
        {
            get
            {
                object parsedValues = base.GetParsedValues("Content-Length");
                if (!this.contentLengthSet && (parsedValues == null))
                {
                    long? nullable = this.calculateLengthFunc.Invoke();
                    if (nullable.HasValue)
                    {
                        base.SetParsedValue("Content-Length", nullable.Value);
                    }
                    return nullable;
                }
                if (parsedValues == null)
                {
                    return null;
                }
                return new long?((long) parsedValues);
            }
            set
            {
                base.SetOrRemoveParsedValue("Content-Length", value);
                this.contentLengthSet = true;
            }
        }

        /// <summary>Gets or sets the value of the Content-Location content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Uri" />.The value of the Content-Location content header on an HTTP response.</returns>
        public Uri ContentLocation
        {
            get
            {
                return (Uri) base.GetParsedValues("Content-Location");
            }
            set
            {
                base.SetOrRemoveParsedValue("Content-Location", value);
            }
        }

        /// <summary>Gets or sets the value of the Content-MD5 content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Byte" />.The value of the Content-MD5 content header on an HTTP response.</returns>
        public byte[] ContentMD5
        {
            get
            {
                return (byte[]) base.GetParsedValues("Content-MD5");
            }
            set
            {
                base.SetOrRemoveParsedValue("Content-MD5", value);
            }
        }

        /// <summary>Gets or sets the value of the Content-Range content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.ContentRangeHeaderValue" />.The value of the Content-Range content header on an HTTP response.</returns>
        public ContentRangeHeaderValue ContentRange
        {
            get
            {
                return (ContentRangeHeaderValue) base.GetParsedValues("Content-Range");
            }
            set
            {
                base.SetOrRemoveParsedValue("Content-Range", value);
            }
        }

        /// <summary>Gets or sets the value of the Content-Type content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.MediaTypeHeaderValue" />.The value of the Content-Type content header on an HTTP response.</returns>
        public MediaTypeHeaderValue ContentType
        {
            get
            {
                return (MediaTypeHeaderValue) base.GetParsedValues("Content-Type");
            }
            set
            {
                base.SetOrRemoveParsedValue("Content-Type", value);
            }
        }

        /// <summary>Gets or sets the value of the Expires content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The value of the Expires content header on an HTTP response.</returns>
        public DateTimeOffset? Expires
        {
            get
            {
                return HeaderUtilities.GetDateTimeOffsetValue("Expires", this);
            }
            set
            {
                base.SetOrRemoveParsedValue("Expires", value);
            }
        }

        /// <summary>Gets or sets the value of the Last-Modified content header on an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The value of the Last-Modified content header on an HTTP response.</returns>
        public DateTimeOffset? LastModified
        {
            get
            {
                return HeaderUtilities.GetDateTimeOffsetValue("Last-Modified", this);
            }
            set
            {
                base.SetOrRemoveParsedValue("Last-Modified", value);
            }
        }
    }
}

