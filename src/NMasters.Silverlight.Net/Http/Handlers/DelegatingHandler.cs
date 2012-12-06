using System;
using System.Threading;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Handlers
{
    /// <summary>A base type for HTTP handlers that delegate the processing of HTTP response messages to another handler, called the inner handler.</summary>
    public abstract class DelegatingHandler : HttpMessageHandler
    {
        private volatile bool disposed;
        private HttpMessageHandler innerHandler;
        private volatile bool operationStarted;

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.DelegatingHandler" /> class.</summary>
        protected DelegatingHandler()
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.DelegatingHandler" /> class with a specific inner handler.</summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        protected DelegatingHandler(HttpMessageHandler innerHandler)
        {
            this.InnerHandler = innerHandler;
        }

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        private void CheckDisposedOrStarted()
        {
            this.CheckDisposed();
            if (this.operationStarted)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.DelegatingHandler" />, and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                if (this.innerHandler != null)
                {
                    this.innerHandler.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", SR.net_http_handler_norequest);
            }
            this.SetOperationStarted();
            return this.innerHandler.SendAsync(request, cancellationToken);
        }

        private void SetOperationStarted()
        {
            this.CheckDisposed();
            if (this.innerHandler == null)
            {
                throw new InvalidOperationException(SR.net_http_handler_not_assigned);
            }
            if (!this.operationStarted)
            {
                this.operationStarted = true;
            }
        }

        /// <summary>Gets or sets the inner handler which processes the HTTP response messages.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpMessageHandler" />.The inner handler for HTTP response messages.</returns>
        public HttpMessageHandler InnerHandler
        {
            get
            {
                return this.innerHandler;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.CheckDisposedOrStarted();
                if (Logging.On)
                {
                    Logging.Associate(Logging.Http, this, value);
                }
                this.innerHandler = value;
            }
        }
    }
}

