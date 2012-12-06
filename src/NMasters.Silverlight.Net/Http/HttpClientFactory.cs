using System.Collections.Generic;
using System.Linq;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Handlers;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http
{
    /// <summary>Represents the factory for creating new instance of <see cref="T:System.Net.Http.HttpClient" />.</summary>
    public static class HttpClientFactory
    {
        /// <summary>Creates a new instance of the <see cref="T:System.Net.Http.HttpClient" />.</summary>
        /// <returns>A new instance of the <see cref="T:System.Net.Http.HttpClient" />.</returns>
        /// <param name="handlers">The list of HTTP handler that delegates the processing of HTTP response messages to another handler.</param>
        public static HttpClient Create(params DelegatingHandler[] handlers)
        {
            return Create(new HttpClientHandler(), handlers);
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Net.Http.HttpClient" />.</summary>
        /// <returns>A new instance of the <see cref="T:System.Net.Http.HttpClient" />.</returns>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        /// <param name="handlers">The list of HTTP handler that delegates the processing of HTTP response messages to another handler.</param>
        public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
        {
            return new HttpClient(CreatePipeline(innerHandler, handlers));
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Net.Http.HttpClient" /> which should be pipelined.</summary>
        /// <returns>A new instance of the <see cref="T:System.Net.Http.HttpClient" /> which should be pipelined.</returns>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        /// <param name="handlers">The list of HTTP handler that delegates the processing of HTTP response messages to another handler.</param>
        public static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<DelegatingHandler> handlers)
        {
            if (innerHandler == null)
            {
                throw Error.ArgumentNull("innerHandler");
            }
            if (handlers == null)
            {
                return innerHandler;
            }
            HttpMessageHandler handler = innerHandler;
            foreach (DelegatingHandler handler2 in handlers.Reverse<DelegatingHandler>())
            {
                if (handler2 == null)
                {
                    // todo: reenable error throw
                    throw Error.Argument("handlers", FSR.DelegatingHandlerArrayContainsNullItem, new object[] { typeof(DelegatingHandler).Name });
                }
                if (handler2.InnerHandler != null)
                {
                    // todo: reenable error throw
                    throw Error.Argument("handlers", FSR.DelegatingHandlerArrayHasNonNullInnerHandler, new object[] { typeof(DelegatingHandler).Name, "InnerHandler", handler2.GetType().Name });
                }
                handler2.InnerHandler = handler;
                handler = handler2;
            }
            return handler;
        }
    }
}

