using System;
using System.Globalization;
using System.Net;
using System.Text;
using NMasters.Silverlight.Net.Http.Content;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;

namespace NMasters.Silverlight.Net.Http
{
    /// <summary>Represents a HTTP response message.</summary>
    public class HttpResponseMessage : IDisposable
    {
        private HttpContent content;
        private const HttpStatusCode defaultStatusCode = HttpStatusCode.OK;
        private bool disposed;
        private HttpResponseHeaders headers;
        private string reasonPhrase;
        private HttpRequestMessage requestMessage;
        private HttpStatusCode statusCode;
        private System.Version version;

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" /> class.</summary>
        public HttpResponseMessage() : this(HttpStatusCode.OK)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" /> class with a specific <see cref="P:NMasters.Silverlight.Net.Http.HttpResponseMessage.StatusCode" />.</summary>
        /// <param name="statusCode">The status code of the HTTP response.</param>
        public HttpResponseMessage(HttpStatusCode statusCode)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Http, this, ".ctor", string.Concat(new object[] { "StatusCode: ", (int) statusCode, ", ReasonPhrase: '", this.reasonPhrase, "'" }));
            }
            if ((statusCode < ((HttpStatusCode) 0)) || (statusCode > ((HttpStatusCode) 0x3e7)))
            {
                throw new ArgumentOutOfRangeException("statusCode");
            }
            this.statusCode = statusCode;
            this.version = HttpUtilities.DefaultVersion;
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

        private bool ContainsNewLineCharacter(string value)
        {
            foreach (char ch in value)
            {
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        return true;
                }
            }
            return false;
        }

        /// <summary>Releases the unmanaged resources and disposes of unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" />.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" /> and optionally disposes of the managed resources.</summary>
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

        /// <summary>Throws an exception if the <see cref="P:NMasters.Silverlight.Net.Http.HttpResponseMessage.IsSuccessStatusCode" /> property for the HTTP response is false.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" />.The HTTP response message if the call is successful.</returns>
        public HttpResponseMessage EnsureSuccessStatusCode()
        {
            if (this.IsSuccessStatusCode)
            {
                return this;
            }
            if (this.content != null)
            {
                this.content.Dispose();
            }
            throw new HttpRequestException(string.Format(CultureInfo.InvariantCulture, SR.net_http_message_not_success_statuscode, new object[] { (int) this.statusCode, this.ReasonPhrase }));
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string representation of the current object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("StatusCode: ");
            builder.Append((int) this.statusCode);
            builder.Append(", ReasonPhrase: '");
            builder.Append(this.ReasonPhrase ?? "<null>");
            builder.Append("', Version: ");
            builder.Append(this.version);
            builder.Append(", Content: ");
            builder.Append((this.content == null) ? "<null>" : this.content.GetType().FullName);
            builder.Append(", Headers:\r\n");
            builder.Append(HeaderUtilities.DumpHeaders(new HttpHeaders[] { this.headers, (this.content == null) ? null : this.content.Headers }));
            return builder.ToString();
        }

        /// <summary>Gets or sets the content of a HTTP response message. </summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" />.The content of the HTTP response message.</returns>
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

        /// <summary>Gets the collection of HTTP response headers. </summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpResponseHeaders" />.The collection of HTTP response headers.</returns>
        public HttpResponseHeaders Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HttpResponseHeaders();
                }
                return this.headers;
            }
        }

        /// <summary>Gets a value that indicates if the HTTP response was successful.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.A value that indicates if the HTTP response was successful. true if <see cref="P:NMasters.Silverlight.Net.Http.HttpResponseMessage.StatusCode" /> was in the range 200-299; otherwise false.</returns>
        public bool IsSuccessStatusCode
        {
            get
            {
                return ((this.statusCode >= HttpStatusCode.OK) && (this.statusCode <= ((HttpStatusCode) 0x12b)));
            }
        }

        /// <summary>Gets or sets the reason phrase which typically is sent by servers together with the status code. </summary>
        /// <returns>Returns <see cref="T:System.String" />.The reason phrase sent by the server.</returns>
        public string ReasonPhrase
        {
            get
            {
                if (this.reasonPhrase != null)
                {
                    return this.reasonPhrase;
                }
                return HttpStatusDescription.Get(this.StatusCode);
            }
            set
            {
                if ((value != null) && this.ContainsNewLineCharacter(value))
                {
                    throw new FormatException(SR.net_http_reasonphrase_format_error);
                }
                this.CheckDisposed();
                this.reasonPhrase = value;
            }
        }

        /// <summary>Gets or sets the request message which led to this response message.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" />.The request message which led to this response message.</returns>
        public HttpRequestMessage RequestMessage
        {
            get
            {
                return this.requestMessage;
            }
            set
            {
                this.CheckDisposed();
                if (Logging.On && (value != null))
                {
                    Logging.Associate(Logging.Http, this, value);
                }
                this.requestMessage = value;
            }
        }

        /// <summary>Gets or sets the status code of the HTTP response.</summary>
        /// <returns>Returns <see cref="T:System.Net.HttpStatusCode" />.The status code of the HTTP response.</returns>
        public HttpStatusCode StatusCode
        {
            get
            {
                return this.statusCode;
            }
            set
            {
                if ((value < ((HttpStatusCode) 0)) || (value > ((HttpStatusCode) 0x3e7)))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.CheckDisposed();
                this.statusCode = value;
            }
        }

        /// <summary>Gets or sets the HTTP message version. </summary>
        /// <returns>Returns <see cref="T:System.Version" />.The HTTP message version. The default is 1.1. </returns>
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

