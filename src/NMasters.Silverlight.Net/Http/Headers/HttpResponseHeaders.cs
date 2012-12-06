using System;
using System.Collections.Generic;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the collection of Response Headers as defined in RFC 2616.</summary>
    public sealed class HttpResponseHeaders : HttpHeaders
    {
        private HttpHeaderValueCollection<string> acceptRanges;
        private HttpGeneralHeaders generalHeaders;
        private static readonly HashSet<string> invalidHeaders;
        private static readonly Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>(StringComparer.OrdinalIgnoreCase);
        private HttpHeaderValueCollection<AuthenticationHeaderValue> proxyAuthenticate;
        private HttpHeaderValueCollection<ProductInfoHeaderValue> server;
        private HttpHeaderValueCollection<string> vary;
        private HttpHeaderValueCollection<AuthenticationHeaderValue> wwwAuthenticate;

        static HttpResponseHeaders()
        {
            parserStore.Add("Accept-Ranges", GenericHeaderParser.TokenListParser);
            parserStore.Add("Age", TimeSpanHeaderParser.Parser);
            parserStore.Add("ETag", GenericHeaderParser.SingleValueEntityTagParser);
            parserStore.Add("Location", UriHeaderParser.RelativeOrAbsoluteUriParser);
            parserStore.Add("Proxy-Authenticate", GenericHeaderParser.MultipleValueAuthenticationParser);
            parserStore.Add("Retry-After", GenericHeaderParser.RetryConditionParser);
            parserStore.Add("Server", ProductInfoHeaderParser.MultipleValueParser);
            parserStore.Add("Vary", GenericHeaderParser.TokenListParser);
            parserStore.Add("WWW-Authenticate", GenericHeaderParser.MultipleValueAuthenticationParser);
            HttpGeneralHeaders.AddParsers(parserStore);
            invalidHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HttpContentHeaders.AddKnownHeaders(invalidHeaders);
        }

        internal HttpResponseHeaders()
        {
            this.generalHeaders = new HttpGeneralHeaders(this);
            base.SetConfiguration(parserStore, invalidHeaders);
        }

        internal override void AddHeaders(HttpHeaders sourceHeaders)
        {
            base.AddHeaders(sourceHeaders);
            HttpResponseHeaders headers = sourceHeaders as HttpResponseHeaders;
            this.generalHeaders.AddSpecialsFrom(headers.generalHeaders);
        }

        internal static void AddKnownHeaders(HashSet<string> headerSet)
        {
            headerSet.Add("Accept-Ranges");
            headerSet.Add("Age");
            headerSet.Add("ETag");
            headerSet.Add("Location");
            headerSet.Add("Proxy-Authenticate");
            headerSet.Add("Retry-After");
            headerSet.Add("Server");
            headerSet.Add("Vary");
            headerSet.Add("WWW-Authenticate");
        }

        /// <summary>Gets the value of the Accept-Ranges header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Accept-Ranges header for an HTTP response.</returns>
        public HttpHeaderValueCollection<string> AcceptRanges
        {
            get
            {
                if (this.acceptRanges == null)
                {
                    this.acceptRanges = new HttpHeaderValueCollection<string>("Accept-Ranges", this, HeaderUtilities.TokenValidator);
                }
                return this.acceptRanges;
            }
        }

        /// <summary>Gets or sets the value of the Age header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.TimeSpan" />.The value of the Age header for an HTTP response.</returns>
        public TimeSpan? Age
        {
            get
            {
                return HeaderUtilities.GetTimeSpanValue("Age", this);
            }
            set
            {
                base.SetOrRemoveParsedValue("Age", value);
            }
        }

        /// <summary>Gets or sets the value of the Cache-Control header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" />.The value of the Cache-Control header for an HTTP response.</returns>
        public CacheControlHeaderValue CacheControl
        {
            get
            {
                return this.generalHeaders.CacheControl;
            }
            set
            {
                this.generalHeaders.CacheControl = value;
            }
        }

        /// <summary>Gets the value of the Connection header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Connection header for an HTTP response.</returns>
        public HttpHeaderValueCollection<string> Connection
        {
            get
            {
                return this.generalHeaders.Connection;
            }
        }

        /// <summary>Gets or sets a value that indicates if the Connection header for an HTTP response contains Close.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the Connection header contains Close, otherwise false.</returns>
        public bool? ConnectionClose
        {
            get
            {
                return this.generalHeaders.ConnectionClose;
            }
            set
            {
                this.generalHeaders.ConnectionClose = value;
            }
        }

        /// <summary>Gets or sets the value of the Date header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The value of the Date header for an HTTP response.</returns>
        public DateTimeOffset? Date
        {
            get
            {
                return this.generalHeaders.Date;
            }
            set
            {
                this.generalHeaders.Date = value;
            }
        }

        /// <summary>Gets or sets the value of the ETag header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.EntityTagHeaderValue" />.The value of the ETag header for an HTTP response.</returns>
        public EntityTagHeaderValue ETag
        {
            get
            {
                return (EntityTagHeaderValue) base.GetParsedValues("ETag");
            }
            set
            {
                base.SetOrRemoveParsedValue("ETag", value);
            }
        }

        /// <summary>Gets or sets the value of the Location header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Uri" />.The value of the Location header for an HTTP response.</returns>
        public Uri Location
        {
            get
            {
                return (Uri) base.GetParsedValues("Location");
            }
            set
            {
                base.SetOrRemoveParsedValue("Location", value);
            }
        }

        /// <summary>Gets the value of the Pragma header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Pragma header for an HTTP response.</returns>
        public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
        {
            get
            {
                return this.generalHeaders.Pragma;
            }
        }

        /// <summary>Gets the value of the Proxy-Authenticate header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Proxy-Authenticate header for an HTTP response.</returns>
        public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate
        {
            get
            {
                if (this.proxyAuthenticate == null)
                {
                    this.proxyAuthenticate = new HttpHeaderValueCollection<AuthenticationHeaderValue>("Proxy-Authenticate", this);
                }
                return this.proxyAuthenticate;
            }
        }

        /// <summary>Gets or sets the value of the Retry-After header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.RetryConditionHeaderValue" />.The value of the Retry-After header for an HTTP response.</returns>
        public RetryConditionHeaderValue RetryAfter
        {
            get
            {
                return (RetryConditionHeaderValue) base.GetParsedValues("Retry-After");
            }
            set
            {
                base.SetOrRemoveParsedValue("Retry-After", value);
            }
        }

        /// <summary>Gets the value of the Server header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Server header for an HTTP response.</returns>
        public HttpHeaderValueCollection<ProductInfoHeaderValue> Server
        {
            get
            {
                if (this.server == null)
                {
                    this.server = new HttpHeaderValueCollection<ProductInfoHeaderValue>("Server", this);
                }
                return this.server;
            }
        }

        /// <summary>Gets the value of the Trailer header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Trailer header for an HTTP response.</returns>
        public HttpHeaderValueCollection<string> Trailer
        {
            get
            {
                return this.generalHeaders.Trailer;
            }
        }

        /// <summary>Gets the value of the Transfer-Encoding header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Transfer-Encoding header for an HTTP response.</returns>
        public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
        {
            get
            {
                return this.generalHeaders.TransferEncoding;
            }
        }

        /// <summary>Gets or sets a value that indicates if the Transfer-Encoding header for an HTTP response contains chunked.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the Transfer-Encoding header contains chunked, otherwise false.</returns>
        public bool? TransferEncodingChunked
        {
            get
            {
                return this.generalHeaders.TransferEncodingChunked;
            }
            set
            {
                this.generalHeaders.TransferEncodingChunked = value;
            }
        }

        /// <summary>Gets the value of the Upgrade header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Upgrade header for an HTTP response.</returns>
        public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
        {
            get
            {
                return this.generalHeaders.Upgrade;
            }
        }

        /// <summary>Gets the value of the Vary header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Vary header for an HTTP response.</returns>
        public HttpHeaderValueCollection<string> Vary
        {
            get
            {
                if (this.vary == null)
                {
                    this.vary = new HttpHeaderValueCollection<string>("Vary", this, HeaderUtilities.TokenValidator);
                }
                return this.vary;
            }
        }

        /// <summary>Gets the value of the Via header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Via header for an HTTP response.</returns>
        public HttpHeaderValueCollection<ViaHeaderValue> Via
        {
            get
            {
                return this.generalHeaders.Via;
            }
        }

        /// <summary>Gets the value of the Warning header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Warning header for an HTTP response.</returns>
        public HttpHeaderValueCollection<WarningHeaderValue> Warning
        {
            get
            {
                return this.generalHeaders.Warning;
            }
        }

        /// <summary>Gets the value of the WWW-Authenticate header for an HTTP response.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the WWW-Authenticate header for an HTTP response.</returns>
        public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate
        {
            get
            {
                if (this.wwwAuthenticate == null)
                {
                    this.wwwAuthenticate = new HttpHeaderValueCollection<AuthenticationHeaderValue>("WWW-Authenticate", this);
                }
                return this.wwwAuthenticate;
            }
        }
    }
}

