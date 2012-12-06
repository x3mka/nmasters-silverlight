using System;
using System.Threading;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Helpers;

namespace NMasters.Silverlight.Net.Http.Handlers
{
    /// <summary>A base type for handlers which only do some small processing of request and/or response messages.</summary>
    public abstract class MessageProcessingHandler : DelegatingHandler
    {
        /// <summary>Creates an instance of a <see cref="T:NMasters.Silverlight.Net.Http.Handlers.MessageProcessingHandler" /> class.</summary>
        protected MessageProcessingHandler()
        {
        }

        /// <summary>Creates an instance of a <see cref="T:NMasters.Silverlight.Net.Http.Handlers.MessageProcessingHandler" /> class with a specific inner handler.</summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        protected MessageProcessingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        private static void HandleCanceledOperations(CancellationToken cancellationToken, TaskCompletionSource<HttpResponseMessage> tcs, OperationCanceledException e)
        {
            // Check if the exception was due to a cancellation. If so, check if the OperationCanceledException is 
            // related to our CancellationToken. If it was indeed caused due to our cancellation token being
            // canceled, set the Task as canceled. Set it to faulted otherwise, since the OperationCanceledException
            // is not related to our cancellation token.
            if (cancellationToken.IsCancellationRequested && (e.CancellationToken == cancellationToken))
            {
                tcs.TrySetCanceled();
            }
            else
            {
                tcs.TrySetException(e);
            }
        }

        /// <summary>Processes an HTTP request message.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" />.The HTTP request message that was processed.</returns>
        /// <param name="request">The HTTP request message to process.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        protected abstract HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken);
        /// <summary>Processes an HTTP response message.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" />.The HTTP response message that was processed.</returns>
        /// <param name="response">The HTTP response message to process.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        protected abstract HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken);
        /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        protected internal sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Action<Task<HttpResponseMessage>> continuation = null;
            if (request == null)
            {
                throw new ArgumentNullException("request", SR.net_http_handler_norequest);
            }
            TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
            try
            {
                HttpRequestMessage newRequestMessage = this.ProcessRequest(request, cancellationToken);
                if (continuation == null)
                {
                    continuation = delegate (Task<HttpResponseMessage> task) {
                        if (task.IsFaulted)
                        {
                            tcs.TrySetException(task.Exception.GetBaseException());
                        }
                        else if (task.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else if (task.Result == null)
                        {
                            tcs.TrySetException(new InvalidOperationException(SR.net_http_handler_noresponse));
                        }
                        else
                        {
                            try
                            {
                                HttpResponseMessage message = this.ProcessResponse(task.Result, cancellationToken);
                                tcs.TrySetResult(message);
                            }
                            catch (OperationCanceledException exception)
                            {
                                HandleCanceledOperations(cancellationToken, tcs, exception);
                            }
                            catch (Exception exception2)
                            {
                                tcs.TrySetException(exception2);
                            }
                        }
                    };
                }
                base.SendAsync(newRequestMessage, cancellationToken).ContinueWithStandard<HttpResponseMessage>(continuation);
            }
            catch (OperationCanceledException exception)
            {
                HandleCanceledOperations(cancellationToken, tcs, exception);
            }
            catch (Exception exception2)
            {
                tcs.TrySetException(exception2);
            }
            return tcs.Task;
        }
    }
}

