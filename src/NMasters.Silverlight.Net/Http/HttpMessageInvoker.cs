using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Handlers;

namespace NMasters.Silverlight.Net.Http
{
    /// <summary>The base type for <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> and other message originators.</summary>
    public class HttpMessageInvoker : IDisposable
    {
        private volatile bool _disposed;
        private readonly bool _disposeHandler;
        private readonly HttpMessageHandler _handler;

        /// <summary>Initializes an instance of a <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageInvoker" /> class with a specific <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageHandler" />.</summary>
        /// <param name="handler">The <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
        public HttpMessageInvoker(HttpMessageHandler handler) : this(handler, true)
        {
        }

        /// <summary>Initializes an instance of a <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageInvoker" /> class with a specific <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageHandler" />.</summary>
        /// <param name="handler">The <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
        /// <param name="disposeHandler">true if the inner handler should be disposed of by Dispose(),false if you intend to reuse the inner handler.</param>
        public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
        {
            _handler = handler;
            _disposeHandler = disposeHandler;
        }

        protected void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageInvoker" />.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageInvoker" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                if (_disposeHandler)
                {
                    _handler.Dispose();
                }
            }
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {            
            if (request == null)            
                throw new ArgumentNullException("request");
            
            CheckDisposed();            
            Task<HttpResponseMessage> retObject = _handler.SendAsync(request, cancellationToken);            
            return retObject;
        }
    }
}

