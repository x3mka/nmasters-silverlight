using System;
using System.Threading;
using System.Threading.Tasks;

namespace NMasters.Silverlight.Net.Http.Handlers
{
    /// <summary>A base type for HTTP message handlers.</summary>
    public abstract class HttpMessageHandler : IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpMessageHandler" /> class.</summary>
        protected HttpMessageHandler()
        {            
        }

        /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpMessageHandler" />.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpMessageHandler" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        protected internal abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}

