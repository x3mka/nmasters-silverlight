using System;
using System.Collections.Generic;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents the collection of Request Headers as defined in RFC 2616.</summary>
    public sealed class HttpRequestHeaders : HttpHeaders
    {
        private HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept;
        private HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptCharset;
        private HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptEncoding;
        private HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptLanguage;
        private HttpHeaderValueCollection<NameValueWithParametersHeaderValue> expect;
        private bool expectContinueSet;
        private HttpGeneralHeaders generalHeaders;
        private HttpHeaderValueCollection<EntityTagHeaderValue> ifMatch;
        private HttpHeaderValueCollection<EntityTagHeaderValue> ifNoneMatch;
        private static readonly HashSet<string> invalidHeaders;
        private static readonly Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>(StringComparer.OrdinalIgnoreCase);
        private HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> te;
        private HttpHeaderValueCollection<ProductInfoHeaderValue> userAgent;

        static HttpRequestHeaders()
        {
            parserStore.Add("Accept", MediaTypeHeaderParser.MultipleValuesParser);
            parserStore.Add("Accept-Charset", GenericHeaderParser.MultipleValueStringWithQualityParser);
            parserStore.Add("Accept-Encoding", GenericHeaderParser.MultipleValueStringWithQualityParser);
            parserStore.Add("Accept-Language", GenericHeaderParser.MultipleValueStringWithQualityParser);
            parserStore.Add("Authorization", GenericHeaderParser.SingleValueAuthenticationParser);
            parserStore.Add("Expect", GenericHeaderParser.MultipleValueNameValueWithParametersParser);
            parserStore.Add("From", GenericHeaderParser.MailAddressParser);
            parserStore.Add("Host", GenericHeaderParser.HostParser);
            parserStore.Add("If-Match", GenericHeaderParser.MultipleValueEntityTagParser);
            parserStore.Add("If-Modified-Since", DateHeaderParser.Parser);
            parserStore.Add("If-None-Match", GenericHeaderParser.MultipleValueEntityTagParser);
            parserStore.Add("If-Range", GenericHeaderParser.RangeConditionParser);
            parserStore.Add("If-Unmodified-Since", DateHeaderParser.Parser);
            parserStore.Add("Max-Forwards", Int32NumberHeaderParser.Parser);
            parserStore.Add("Proxy-Authorization", GenericHeaderParser.SingleValueAuthenticationParser);
            parserStore.Add("Range", GenericHeaderParser.RangeParser);
            parserStore.Add("Referer", UriHeaderParser.RelativeOrAbsoluteUriParser);
            parserStore.Add("TE", TransferCodingHeaderParser.MultipleValueWithQualityParser);
            parserStore.Add("User-Agent", ProductInfoHeaderParser.MultipleValueParser);
            HttpGeneralHeaders.AddParsers(parserStore);
            invalidHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HttpContentHeaders.AddKnownHeaders(invalidHeaders);
        }

        internal HttpRequestHeaders()
        {
            this.generalHeaders = new HttpGeneralHeaders(this);
            base.SetConfiguration(parserStore, invalidHeaders);
        }

        internal override void AddHeaders(HttpHeaders sourceHeaders)
        {
            base.AddHeaders(sourceHeaders);
            HttpRequestHeaders headers = sourceHeaders as HttpRequestHeaders;
            this.generalHeaders.AddSpecialsFrom(headers.generalHeaders);
            if (!this.ExpectContinue.HasValue)
            {
                this.ExpectContinue = headers.ExpectContinue;
            }
        }

        internal static void AddKnownHeaders(HashSet<string> headerSet)
        {
            headerSet.Add("Accept");
            headerSet.Add("Accept-Charset");
            headerSet.Add("Accept-Encoding");
            headerSet.Add("Accept-Language");
            headerSet.Add("Authorization");
            headerSet.Add("Expect");
            headerSet.Add("From");
            headerSet.Add("Host");
            headerSet.Add("If-Match");
            headerSet.Add("If-Modified-Since");
            headerSet.Add("If-None-Match");
            headerSet.Add("If-Range");
            headerSet.Add("If-Unmodified-Since");
            headerSet.Add("Max-Forwards");
            headerSet.Add("Proxy-Authorization");
            headerSet.Add("Range");
            headerSet.Add("Referer");
            headerSet.Add("TE");
            headerSet.Add("User-Agent");
        }

        /// <summary>Gets the value of the Accept header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Accept header for an HTTP request.</returns>
        public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept
        {
            get
            {
                if (this.accept == null)
                {
                    this.accept = new HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>("Accept", this);
                }
                return this.accept;
            }
        }

        /// <summary>Gets the value of the Accept-Charset header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Accept-Charset header for an HTTP request.</returns>
        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset
        {
            get
            {
                if (this.acceptCharset == null)
                {
                    this.acceptCharset = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Charset", this);
                }
                return this.acceptCharset;
            }
        }

        /// <summary>Gets the value of the Accept-Encoding header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Accept-Encoding header for an HTTP request.</returns>
        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding
        {
            get
            {
                if (this.acceptEncoding == null)
                {
                    this.acceptEncoding = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Encoding", this);
                }
                return this.acceptEncoding;
            }
        }

        /// <summary>Gets the value of the Accept-Language header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Accept-Language header for an HTTP request.</returns>
        public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage
        {
            get
            {
                if (this.acceptLanguage == null)
                {
                    this.acceptLanguage = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Language", this);
                }
                return this.acceptLanguage;
            }
        }

        /// <summary>Gets or sets the value of the Authorization header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" />.The value of the Authorization header for an HTTP request.</returns>
        public AuthenticationHeaderValue Authorization
        {
            get
            {
                return (AuthenticationHeaderValue) base.GetParsedValues("Authorization");
            }
            set
            {
                base.SetOrRemoveParsedValue("Authorization", value);
            }
        }

        /// <summary>Gets or sets the value of the Cache-Control header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.CacheControlHeaderValue" />.The value of the Cache-Control header for an HTTP request.</returns>
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

        /// <summary>Gets the value of the Connection header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Connection header for an HTTP request.</returns>
        public HttpHeaderValueCollection<string> Connection
        {
            get
            {
                return this.generalHeaders.Connection;
            }
        }

        /// <summary>Gets or sets a value that indicates if the Connection header for an HTTP request contains Close.</summary>
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

        /// <summary>Gets or sets the value of the Date header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The value of the Date header for an HTTP request.</returns>
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

        /// <summary>Gets the value of the Expect header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Expect header for an HTTP request.</returns>
        public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect
        {
            get
            {
                return this.ExpectCore;
            }
        }

        /// <summary>Gets or sets a value that indicates if the Expect header for an HTTP request contains Continue.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the Expect header contains Continue, otherwise false.</returns>
        public bool? ExpectContinue
        {
            get
            {
                if (this.ExpectCore.IsSpecialValueSet)
                {
                    return true;
                }
                if (this.expectContinueSet)
                {
                    return false;
                }
                return null;
            }
            set
            {
                if (value == true)
                {
                    this.expectContinueSet = true;
                    this.ExpectCore.SetSpecialValue();
                }
                else
                {
                    this.expectContinueSet = value.HasValue;
                    this.ExpectCore.RemoveSpecialValue();
                }
            }
        }

        private HttpHeaderValueCollection<NameValueWithParametersHeaderValue> ExpectCore
        {
            get
            {
                if (this.expect == null)
                {
                    this.expect = new HttpHeaderValueCollection<NameValueWithParametersHeaderValue>("Expect", this, HeaderUtilities.ExpectContinue);
                }
                return this.expect;
            }
        }

        /// <summary>Gets or sets the value of the From header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The value of the From header for an HTTP request.</returns>
        public string From
        {
            get
            {
                return (string) base.GetParsedValues("From");
            }
            set
            {
                if (value == string.Empty)
                {
                    value = null;
                }
                // SL fixes
                //if ((value != null) && !HeaderUtilities.IsValidEmailAddress(value))
                //{
                //    throw new FormatException(SR.net_http_headers_invalid_from_header);
                //}
                base.SetOrRemoveParsedValue("From", value);
            }
        }

        /// <summary>Gets or sets the value of the Host header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The value of the Host header for an HTTP request.</returns>
        public string Host
        {
            get
            {
                return (string) base.GetParsedValues("Host");
            }
            set
            {
                if (value == string.Empty)
                {
                    value = null;
                }
                string host = null;
                if ((value != null) && (HttpRuleParser.GetHostLength(value, 0, false, out host) != value.Length))
                {
                    throw new FormatException(SR.net_http_headers_invalid_host_header);
                }
                base.SetOrRemoveParsedValue("Host", value);
            }
        }

        /// <summary>Gets the value of the If-Match header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the If-Match header for an HTTP request.</returns>
        public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch
        {
            get
            {
                if (this.ifMatch == null)
                {
                    this.ifMatch = new HttpHeaderValueCollection<EntityTagHeaderValue>("If-Match", this);
                }
                return this.ifMatch;
            }
        }

        /// <summary>Gets or sets the value of the If-Modified-Since header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The value of the If-Modified-Since header for an HTTP request.</returns>
        public DateTimeOffset? IfModifiedSince
        {
            get
            {
                return HeaderUtilities.GetDateTimeOffsetValue("If-Modified-Since", this);
            }
            set
            {
                base.SetOrRemoveParsedValue("If-Modified-Since", value);
            }
        }

        /// <summary>Gets the value of the If-None-Match header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.Gets the value of the If-None-Match header for an HTTP request.</returns>
        public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch
        {
            get
            {
                if (this.ifNoneMatch == null)
                {
                    this.ifNoneMatch = new HttpHeaderValueCollection<EntityTagHeaderValue>("If-None-Match", this);
                }
                return this.ifNoneMatch;
            }
        }

        /// <summary>Gets or sets the value of the If-Range header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeConditionHeaderValue" />.The value of the If-Range header for an HTTP request.</returns>
        public RangeConditionHeaderValue IfRange
        {
            get
            {
                return (RangeConditionHeaderValue) base.GetParsedValues("If-Range");
            }
            set
            {
                base.SetOrRemoveParsedValue("If-Range", value);
            }
        }

        /// <summary>Gets or sets the value of the If-Unmodified-Since header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.DateTimeOffset" />.The value of the If-Unmodified-Since header for an HTTP request.</returns>
        public DateTimeOffset? IfUnmodifiedSince
        {
            get
            {
                return HeaderUtilities.GetDateTimeOffsetValue("If-Unmodified-Since", this);
            }
            set
            {
                base.SetOrRemoveParsedValue("If-Unmodified-Since", value);
            }
        }

        /// <summary>Gets or sets the value of the Max-Forwards header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.The value of the Max-Forwards header for an HTTP request.</returns>
        public int? MaxForwards
        {
            get
            {
                object parsedValues = base.GetParsedValues("Max-Forwards");
                if (parsedValues != null)
                {
                    return new int?((int) parsedValues);
                }
                return null;
            }
            set
            {
                base.SetOrRemoveParsedValue("Max-Forwards", value);
            }
        }

        /// <summary>Gets the value of the Pragma header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Pragma header for an HTTP request.</returns>
        public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
        {
            get
            {
                return this.generalHeaders.Pragma;
            }
        }

        /// <summary>Gets or sets the value of the Proxy-Authorization header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" />.The value of the Proxy-Authorization header for an HTTP request.</returns>
        public AuthenticationHeaderValue ProxyAuthorization
        {
            get
            {
                return (AuthenticationHeaderValue) base.GetParsedValues("Proxy-Authorization");
            }
            set
            {
                base.SetOrRemoveParsedValue("Proxy-Authorization", value);
            }
        }

        /// <summary>Gets or sets the value of the Range header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.RangeHeaderValue" />.The value of the Range header for an HTTP request.</returns>
        public RangeHeaderValue Range
        {
            get
            {
                return (RangeHeaderValue) base.GetParsedValues("Range");
            }
            set
            {
                base.SetOrRemoveParsedValue("Range", value);
            }
        }

        /// <summary>Gets or sets the value of the Referer header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.Uri" />.The value of the Referer header for an HTTP request.</returns>
        public Uri Referrer
        {
            get
            {
                return (Uri) base.GetParsedValues("Referer");
            }
            set
            {
                base.SetOrRemoveParsedValue("Referer", value);
            }
        }

        /// <summary>Gets the value of the TE header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the TE header for an HTTP request.</returns>
        public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE
        {
            get
            {
                if (this.te == null)
                {
                    this.te = new HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue>("TE", this);
                }
                return this.te;
            }
        }

        /// <summary>Gets the value of the Trailer header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Trailer header for an HTTP request.</returns>
        public HttpHeaderValueCollection<string> Trailer
        {
            get
            {
                return this.generalHeaders.Trailer;
            }
        }

        /// <summary>Gets the value of the Transfer-Encoding header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Transfer-Encoding header for an HTTP request.</returns>
        public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
        {
            get
            {
                return this.generalHeaders.TransferEncoding;
            }
        }

        /// <summary>Gets or sets a value that indicates if the Transfer-Encoding header for an HTTP request contains chunked.</summary>
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

        /// <summary>Gets the value of the Upgrade header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Upgrade header for an HTTP request.</returns>
        public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
        {
            get
            {
                return this.generalHeaders.Upgrade;
            }
        }

        /// <summary>Gets the value of the User-Agent header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the User-Agent header for an HTTP request.</returns>
        public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent
        {
            get
            {
                if (this.userAgent == null)
                {
                    this.userAgent = new HttpHeaderValueCollection<ProductInfoHeaderValue>("User-Agent", this);
                }
                return this.userAgent;
            }
        }

        /// <summary>Gets the value of the Via header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Via header for an HTTP request.</returns>
        public HttpHeaderValueCollection<ViaHeaderValue> Via
        {
            get
            {
                return this.generalHeaders.Via;
            }
        }

        /// <summary>Gets the value of the Warning header for an HTTP request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpHeaderValueCollection`1" />.The value of the Warning header for an HTTP request.</returns>
        public HttpHeaderValueCollection<WarningHeaderValue> Warning
        {
            get
            {
                return this.generalHeaders.Warning;
            }
        }
    }
}

