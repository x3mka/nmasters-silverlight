using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NMasters.Silverlight.Net.Http.Content;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;

namespace NMasters.Silverlight.Net.Http
{
    /// <summary>Represents a HTTP request message.</summary>
    public class HttpRequestMessage : IDisposable
    {
        private HttpContent content;
        private bool disposed;
        private HttpRequestHeaders headers;
        private const int messageAlreadySent = 1;
        private const int messageNotYetSent = 0;
        private HttpMethod method;
        private IDictionary<string, object> properties;
        private Uri requestUri;
        private int sendStatus;
        private System.Version version;

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" /> class.</summary>
        public HttpRequestMessage() : this(HttpMethod.Get, (Uri) null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">A string that represents the request  <see cref="T:System.Uri" />.</param>
        public HttpRequestMessage(HttpMethod method, string requestUri)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Http, this, ".ctor", string.Concat(new object[] { "Method: ", method, ", Uri: '", requestUri, "'" }));
            }
            if (string.IsNullOrEmpty(requestUri))
            {
                this.InitializeValues(method, null);
            }
            else
            {
                this.InitializeValues(method, new Uri(requestUri, UriKind.RelativeOrAbsolute));
            }
            if (Logging.On)
            {
                Logging.Exit(Logging.Http, this, ".ctor", (string) null);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The <see cref="T:System.Uri" /> to request.</param>
        public HttpRequestMessage(HttpMethod method, Uri requestUri)
        {            
            if (Logging.On)
            {
                Logging.Enter(Logging.Http, this, ".ctor", string.Concat(new object[] { "Method: ", method, ", Uri: '", requestUri, "'" }));
            }
            this.InitializeValues(method, requestUri);
            if (Logging.On)
            {
                Logging.Exit(Logging.Http, this, ".ctor", (string) null);
            }
        }

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" />.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                if (this.content != null)
                {
                    this.content.Dispose();
                }
            }
        }

        private void InitializeValues(HttpMethod method, Uri requestUri)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (((requestUri != null) && requestUri.IsAbsoluteUri) && !HttpUtilities.IsHttpUri(requestUri))
            {
                throw new ArgumentException(SR.net_http_client_http_baseaddress_required, "requestUri");
            }
            this.method = method;
            this.requestUri = requestUri;
            this.version = HttpUtilities.DefaultVersion;
        }

        internal bool IsSent()
        {
            return (Interlocked.Exchange(ref this.sendStatus, 1) == 0);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string representation of the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Method: ");
            builder.Append(this.method);
            builder.Append(", RequestUri: '");
            builder.Append((this.requestUri == null) ? "<null>" : this.requestUri.ToString());
            builder.Append("', Version: ");
            builder.Append(this.version);
            builder.Append(", Content: ");
            builder.Append((this.content == null) ? "<null>" : this.content.GetType().FullName);
            builder.Append(", Headers:\r\n");
            builder.Append(HeaderUtilities.DumpHeaders(new HttpHeaders[] { this.headers, (this.content == null) ? null : this.content.Headers }));
            return builder.ToString();
        }

        /// <summary>Gets or sets the contents of the HTTP message. </summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" />.The content of a message</returns>
        public HttpContent Content
        {
            get
            {
                return this.content;
            }
            set
            {
                this.CheckDisposed();
                if (Logging.On)
                {
                    if (value == null)
                    {
                        Logging.PrintInfo(Logging.Http, this, SR.net_http_log_content_null);
                    }
                    else
                    {
                        Logging.Associate(Logging.Http, this, value);
                    }
                }
                this.content = value;
            }
        }

        /// <summary>Gets the collection of HTTP request headers.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpRequestHeaders" />.The collection of HTTP request headers.</returns>
        public HttpRequestHeaders Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HttpRequestHeaders();
                }
                return this.headers;
            }
        }

        /// <summary>Gets or sets the HTTP method used by the HTTP request message.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpMethod" />.The HTTP method used by the request message. The default is the GET method.</returns>
        public HttpMethod Method
        {
            get
            {
                return this.method;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.CheckDisposed();
                this.method = value;
            }
        }

        /// <summary>Gets a set of properties for the HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        public IDictionary<string, object> Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new Dictionary<string, object>();
                }
                return this.properties;
            }
        }

        /// <summary>Gets or sets the <see cref="T:System.Uri" /> used for the HTTP request.</summary>
        /// <returns>Returns <see cref="T:System.Uri" />.The <see cref="T:System.Uri" /> used for the HTTP request.</returns>
        public Uri RequestUri
        {
            get
            {
                return this.requestUri;
            }
            set
            {
                if (((value != null) && value.IsAbsoluteUri) && !HttpUtilities.IsHttpUri(value))
                {
                    throw new ArgumentException(SR.net_http_client_http_baseaddress_required, "value");
                }
                this.CheckDisposed();
                this.requestUri = value;
            }
        }

        /// <summary>Gets or sets the HTTP message version.</summary>
        /// <returns>Returns <see cref="T:System.Version" />.The HTTP message version. The default is 1.1.</returns>
        public System.Version Version
        {
            get
            {
                return this.version;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.CheckDisposed();
                this.version = value;
            }
        }
    }
}

