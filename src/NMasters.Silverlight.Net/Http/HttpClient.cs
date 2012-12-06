using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Configuration;
using NMasters.Silverlight.Net.Http.Content;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Handlers;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http
{
    /// <summary>Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI. </summary>
    public class HttpClient : HttpMessageInvoker
    {
        private Uri _baseAddress;                

        private volatile bool _disposed;                       
        private volatile bool _operationStarted;

        private CancellationTokenSource _pendingRequestsCts;        

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> class.</summary>
        public HttpClient() : this(new HttpClientHandler())
        {
            TransportConfiguration = new HttpTransportConfiguration();
            DefaultRequestHeaders = new HttpRequestHeaders();
            _pendingRequestsCts = new CancellationTokenSource();   
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> class with a specific handler.</summary>
        /// <param name="handler">The HTTP handler stack to use for sending requests. </param>
        public HttpClient(HttpMessageHandler handler) : this(handler, true)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> class with a specific handler.</summary>
        /// <param name="handler">The <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
        /// <param name="disposeHandler">true if the inner handler should be disposed of by Dispose(),false if you intend to reuse the inner handler.</param>
        public HttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {                                                                                
        }

        /// <summary>Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</summary>
        /// <returns>Returns <see cref="T:System.Uri" />.The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</returns>
        public Uri BaseAddress
        {
            get
            {
                return _baseAddress;
            }
            set
            {
                CheckBaseAddress(value, "value");
                CheckDisposedOrStarted();
                _baseAddress = value;
            }
        }

        /// <summary>Gets the headers which should be sent with each request.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpRequestHeaders" />.The headers which should be sent with each request.</returns>
        public HttpRequestHeaders DefaultRequestHeaders { get; protected set; }

        public HttpTransportConfiguration TransportConfiguration { get; protected set; }

        /// <summary>Cancel all pending requests on this instance.</summary>
        public void CancelPendingRequests()
        {
            this.CheckDisposed();
            
            var source = Interlocked.Exchange(ref this._pendingRequestsCts, new CancellationTokenSource());
            source.Cancel();
            source.Dispose();
        }

        #region Private Methods

        private static void CheckBaseAddress(Uri baseAddress, string parameterName)
        {
            if (baseAddress != null)
            {
                if (!baseAddress.IsAbsoluteUri)
                {
                    throw new ArgumentException(SR.net_http_client_absolute_baseaddress_required, parameterName);
                }
                if (!HttpUtilities.IsHttpUri(baseAddress))
                {
                    throw new ArgumentException(SR.net_http_client_http_baseaddress_required, parameterName);
                }
            }
        }     

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (_operationStarted)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        private static void CheckRequestMessage(HttpRequestMessage request)
        {
            if (!request.IsSent())
            {
                throw new InvalidOperationException(SR.net_http_client_request_already_sent);
            }
        }

        private static void DisposeCancellationToken(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                cancellationTokenSource.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private static void DisposeRequestContent(HttpRequestMessage request)
        {
            HttpContent content = request.Content;
            if (content != null)
            {
                content.Dispose();
            }
        }

        #endregion



        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _pendingRequestsCts.Cancel();
                DisposeCancellationToken(_pendingRequestsCts);                
            }
            base.Dispose(disposing);
        }

        private void LogSendError(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource, string method, Exception e)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                if (Logging.On)
                {
                    Logging.PrintError(Logging.Http, this, method, string.Format(CultureInfo.InvariantCulture, SR.net_http_client_send_canceled, new object[] { Logging.GetObjectLogHash(request) }));
                }
            }
            else if (Logging.On)
            {
                Logging.PrintError(Logging.Http, this, method, string.Format(CultureInfo.InvariantCulture, SR.net_http_client_send_error, new object[] { Logging.GetObjectLogHash(request), e }));
            }
        }

        private void PrepareRequestMessage(HttpRequestMessage request)
        {
            Uri baseAddress = null;
            if ((request.RequestUri == null) && (_baseAddress == null))
            {
                throw new InvalidOperationException(SR.net_http_client_invalid_requesturi);
            }
            if (request.RequestUri == null)
            {
                baseAddress = _baseAddress;
            }
            else if (!request.RequestUri.IsAbsoluteUri)
            {
                if (_baseAddress == null)
                {
                    throw new InvalidOperationException(SR.net_http_client_invalid_requesturi);
                }
                baseAddress = new Uri(_baseAddress, request.RequestUri);
            }
            if (baseAddress != null)
            {
                request.RequestUri = baseAddress;
            }
            if (DefaultRequestHeaders != null)
            {
                request.Headers.AddHeaders(DefaultRequestHeaders);
            }
        }        

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> instance.</exception>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            this.CheckDisposed();
            CheckRequestMessage(request);
            this.SetOperationStarted();
            this.PrepareRequestMessage(request);
            CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._pendingRequestsCts.Token);

            // Handle timeout (create timer or something else)
            //TimerThread.Timer timeoutTimer = this.SetTimeout(linkedCts);            

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            try
            {
                Action<Task<HttpResponseMessage>> continuation = delegate (Task<HttpResponseMessage> task) {
                                                                                                               try
                                                                                                               {
                                                                                                                   DisposeRequestContent(request);
                                                                                                                   if (task.IsFaulted)
                                                                                                                   {
                                                                                                                       this.SetTaskFaulted(request, linkedCts, tcs, task.Exception.GetBaseException());
                                                                                                                   }
                                                                                                                   else if (task.IsCanceled)
                                                                                                                   {
                                                                                                                       this.SetTaskCanceled(request, linkedCts, tcs);
                                                                                                                   }
                                                                                                                   else
                                                                                                                   {
                                                                                                                       HttpResponseMessage response = task.Result;
                                                                                                                       if (response == null)
                                                                                                                       {
                                                                                                                           this.SetTaskFaulted(request, linkedCts, tcs, new InvalidOperationException(SR.net_http_handler_noresponse));
                                                                                                                       }
                                                                                                                       else if ((response.Content == null) || (completionOption == HttpCompletionOption.ResponseHeadersRead))
                                                                                                                       {
                                                                                                                           this.SetTaskCompleted(request, linkedCts, tcs, response);
                                                                                                                       }
                                                                                                                       else
                                                                                                                       {
                                                                                                                           this.StartContentBuffering(request, linkedCts, tcs, response);
                                                                                                                       }
                                                                                                                   }
                                                                                                               }
                                                                                                               catch (Exception exception)
                                                                                                               {
                                                                                                                   if (Logging.On)
                                                                                                                   {
                                                                                                                       Logging.Exception(Logging.Http, this, "SendAsync", exception);
                                                                                                                   }
                                                                                                                   tcs.TrySetException(exception);
                                                                                                               }
                };
                base.SendAsync(request, linkedCts.Token).ContinueWithStandard<HttpResponseMessage>(continuation);
            }
            catch
            {                
                throw;
            }
            return tcs.Task;
        }

        private void SetOperationStarted()
        {
            if (!_operationStarted)
            {
                _operationStarted = true;
            }
        }

        private void SetTaskCanceled(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource, TaskCompletionSource<HttpResponseMessage> tcs)
        {
            this.LogSendError(request, cancellationTokenSource, "SendAsync", null);
            tcs.TrySetCanceled();
            DisposeCancellationToken(cancellationTokenSource);
        }

        private void SetTaskCompleted(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource, TaskCompletionSource<HttpResponseMessage> tcs, HttpResponseMessage response)
        {
            //if (Logging.On)
            //{
            //    Logging.PrintInfo(Logging.Http, this, string.Format(CultureInfo.InvariantCulture, SR.net_http_client_send_completed, new object[] { Logging.GetObjectLogHash(request), Logging.GetObjectLogHash(response), response }));
            //}
            tcs.TrySetResult(response);
            DisposeCancellationToken(cancellationTokenSource);
        }

        private void SetTaskFaulted(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource, TaskCompletionSource<HttpResponseMessage> tcs, Exception e)
        {
            LogSendError(request, cancellationTokenSource, "SendAsync", e);
            tcs.TrySetException(e);
            DisposeCancellationToken(cancellationTokenSource);
        }

        private void StartContentBuffering(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource, TaskCompletionSource<HttpResponseMessage> tcs, HttpResponseMessage response)
        {
            response.Content.LoadIntoBufferAsync(TransportConfiguration.MaxResponseContentBufferSize).ContinueWithStandard(delegate (Task contentTask) {
                try
                {
                    bool flag = cancellationTokenSource.Token.IsCancellationRequested;
                    if (contentTask.IsFaulted)
                    {
                        response.Dispose();
                        if (flag && (contentTask.Exception.GetBaseException() is HttpRequestException))
                        {
                            SetTaskCanceled(request, cancellationTokenSource, tcs);
                        }
                        else
                        {
                            SetTaskFaulted(request, cancellationTokenSource, tcs, contentTask.Exception.GetBaseException());
                        }
                    }
                    else if (contentTask.IsCanceled)
                    {
                        response.Dispose();
                        SetTaskCanceled(request, cancellationTokenSource, tcs);
                    }
                    else
                    {
                        SetTaskCompleted(request, cancellationTokenSource, tcs, response);
                    }
                }
                catch (Exception exception)
                {
                    response.Dispose();
                    tcs.TrySetException(exception);
                    if (Logging.On)
                    {
                        Logging.Exception(Logging.Http, this, "SendAsync", exception);
                    }
                }
            });
        }     

                
    }
}

