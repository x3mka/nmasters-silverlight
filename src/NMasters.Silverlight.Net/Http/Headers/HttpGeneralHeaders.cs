using System;
using System.Collections.Generic;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal sealed class HttpGeneralHeaders
    {
        private HttpHeaderValueCollection<string> connection;
        private bool connectionCloseSet;
        private HttpHeaders parent;
        private HttpHeaderValueCollection<NameValueHeaderValue> pragma;
        private HttpHeaderValueCollection<string> trailer;
        private HttpHeaderValueCollection<TransferCodingHeaderValue> transferEncoding;
        private bool transferEncodingChunkedSet;
        private HttpHeaderValueCollection<ProductHeaderValue> upgrade;
        private HttpHeaderValueCollection<ViaHeaderValue> via;
        private HttpHeaderValueCollection<WarningHeaderValue> warning;

        internal HttpGeneralHeaders(HttpHeaders parent)
        {
            this.parent = parent;
        }

        internal static void AddKnownHeaders(HashSet<string> headerSet)
        {
            headerSet.Add("Cache-Control");
            headerSet.Add("Connection");
            headerSet.Add("Date");
            headerSet.Add("Pragma");
            headerSet.Add("Trailer");
            headerSet.Add("Transfer-Encoding");
            headerSet.Add("Upgrade");
            headerSet.Add("Via");
            headerSet.Add("Warning");
        }

        internal static void AddParsers(Dictionary<string, HttpHeaderParser> parserStore)
        {
            parserStore.Add("Cache-Control", CacheControlHeaderParser.Parser);
            parserStore.Add("Connection", GenericHeaderParser.TokenListParser);
            parserStore.Add("Date", DateHeaderParser.Parser);
            parserStore.Add("Pragma", GenericHeaderParser.MultipleValueNameValueParser);
            parserStore.Add("Trailer", GenericHeaderParser.TokenListParser);
            parserStore.Add("Transfer-Encoding", TransferCodingHeaderParser.MultipleValueParser);
            parserStore.Add("Upgrade", GenericHeaderParser.MultipleValueProductParser);
            parserStore.Add("Via", GenericHeaderParser.MultipleValueViaParser);
            parserStore.Add("Warning", GenericHeaderParser.MultipleValueWarningParser);
        }

        internal void AddSpecialsFrom(HttpGeneralHeaders sourceHeaders)
        {
            if (!this.TransferEncodingChunked.HasValue)
            {
                this.TransferEncodingChunked = sourceHeaders.TransferEncodingChunked;
            }
            if (!this.ConnectionClose.HasValue)
            {
                this.ConnectionClose = sourceHeaders.ConnectionClose;
            }
        }

        public CacheControlHeaderValue CacheControl
        {
            get
            {
                return (CacheControlHeaderValue) this.parent.GetParsedValues("Cache-Control");
            }
            set
            {
                this.parent.SetOrRemoveParsedValue("Cache-Control", value);
            }
        }

        public HttpHeaderValueCollection<string> Connection
        {
            get
            {
                return this.ConnectionCore;
            }
        }

        public bool? ConnectionClose
        {
            get
            {
                if (this.ConnectionCore.IsSpecialValueSet)
                {
                    return true;
                }
                if (this.connectionCloseSet)
                {
                    return false;
                }
                return null;
            }
            set
            {
                if (value == true)
                {
                    this.connectionCloseSet = true;
                    this.ConnectionCore.SetSpecialValue();
                }
                else
                {
                    this.connectionCloseSet = value.HasValue;
                    this.ConnectionCore.RemoveSpecialValue();
                }
            }
        }

        private HttpHeaderValueCollection<string> ConnectionCore
        {
            get
            {
                if (this.connection == null)
                {
                    this.connection = new HttpHeaderValueCollection<string>("Connection", this.parent, "close", HeaderUtilities.TokenValidator);
                }
                return this.connection;
            }
        }

        public DateTimeOffset? Date
        {
            get
            {
                return HeaderUtilities.GetDateTimeOffsetValue("Date", this.parent);
            }
            set
            {
                this.parent.SetOrRemoveParsedValue("Date", value);
            }
        }

        public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
        {
            get
            {
                if (this.pragma == null)
                {
                    this.pragma = new HttpHeaderValueCollection<NameValueHeaderValue>("Pragma", this.parent);
                }
                return this.pragma;
            }
        }

        public HttpHeaderValueCollection<string> Trailer
        {
            get
            {
                if (this.trailer == null)
                {
                    this.trailer = new HttpHeaderValueCollection<string>("Trailer", this.parent, HeaderUtilities.TokenValidator);
                }
                return this.trailer;
            }
        }

        public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
        {
            get
            {
                return this.TransferEncodingCore;
            }
        }

        public bool? TransferEncodingChunked
        {
            get
            {
                if (this.TransferEncodingCore.IsSpecialValueSet)
                {
                    return true;
                }
                if (this.transferEncodingChunkedSet)
                {
                    return false;
                }
                return null;
            }
            set
            {
                if (value == true)
                {
                    this.transferEncodingChunkedSet = true;
                    this.TransferEncodingCore.SetSpecialValue();
                }
                else
                {
                    this.transferEncodingChunkedSet = value.HasValue;
                    this.TransferEncodingCore.RemoveSpecialValue();
                }
            }
        }

        private HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncodingCore
        {
            get
            {
                if (this.transferEncoding == null)
                {
                    this.transferEncoding = new HttpHeaderValueCollection<TransferCodingHeaderValue>("Transfer-Encoding", this.parent, HeaderUtilities.TransferEncodingChunked);
                }
                return this.transferEncoding;
            }
        }

        public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
        {
            get
            {
                if (this.upgrade == null)
                {
                    this.upgrade = new HttpHeaderValueCollection<ProductHeaderValue>("Upgrade", this.parent);
                }
                return this.upgrade;
            }
        }

        public HttpHeaderValueCollection<ViaHeaderValue> Via
        {
            get
            {
                if (this.via == null)
                {
                    this.via = new HttpHeaderValueCollection<ViaHeaderValue>("Via", this.parent);
                }
                return this.via;
            }
        }

        public HttpHeaderValueCollection<WarningHeaderValue> Warning
        {
            get
            {
                if (this.warning == null)
                {
                    this.warning = new HttpHeaderValueCollection<WarningHeaderValue>("Warning", this.parent);
                }
                return this.warning;
            }
        }
    }
}

