using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Configuration;
using NMasters.Silverlight.Net.Http.Content;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Handlers
{
    /// <summary>The default message handler used by <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" />.  </summary>
    public class HttpClientHandler : HttpMessageHandler
    {
        private bool allowAutoRedirect;        
        private ClientCertificateOption clientCertOptions;        
        private CookieContainer cookieContainer;
        private ICredentials credentials;
        private volatile bool disposed;
                
        private int maxAutomaticRedirections;
        private long maxRequestContentBufferSize;        
        private volatile bool operationStarted;
        private bool preAuthenticate;        
        
        private bool useCookies;
        private bool useDefaultCredentials;        

        /// <summary>Creates an instance of a <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler" /> class.</summary>
        public HttpClientHandler()
        {
            this.allowAutoRedirect = true;
            this.maxRequestContentBufferSize = 0x7fffffff;            
            this.cookieContainer = new CookieContainer();
            this.credentials = null;
            this.maxAutomaticRedirections = 50;
            this.preAuthenticate = false;            
            this.useCookies = true;
            this.useDefaultCredentials = false;
            this.clientCertOptions = ClientCertificateOption.Manual;
        }

        private static bool AreEqual(string x, string y)
        {
            return (string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        internal void CheckDisposedOrStarted()
        {
            this.CheckDisposed();
            if (this.operationStarted)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request)
        {
            HttpWebRequest objB = WebRequest.CreateHttp(request.RequestUri);            
            objB.Method = request.Method.Method;            
            this.SetDefaultOptions(objB);            
            this.SetServicePointOptions(objB, request);
            SetRequestHeaders(objB, request);
            SetContentHeaders(objB, request);
            this.InitializeWebRequest(request, objB);
            return objB;
        }

        private HttpResponseMessage CreateResponseMessage(HttpWebResponse webResponse, HttpRequestMessage request)
        {
            HttpResponseMessage message = new HttpResponseMessage(webResponse.StatusCode) {
                ReasonPhrase = webResponse.StatusDescription,                
                RequestMessage = request,
                Content = new StreamContent(new WebExceptionWrapperStream(webResponse.GetResponseStream()))
            };
            request.RequestUri = webResponse.ResponseUri;
            WebHeaderCollection headers = webResponse.Headers;
            HttpContentHeaders headers2 = message.Content.Headers;
            HttpResponseHeaders headers3 = message.Headers;
            if (webResponse.ContentLength >= 0)
            {
                headers2.ContentLength = new long?(webResponse.ContentLength);
            }

            // SL fixes
            //for (int i = 0; i < headers.Count; i++)
            //{
            //    string key = headers.GetKey(i);
            //    if (string.Compare(key, "Content-Length", StringComparison.OrdinalIgnoreCase) != 0)
            //    {
            //        string[] values = headers.GetValues(i);
            //        if (!headers3.TryAddWithoutValidation(key, values))
            //        {
            //            headers2.TryAddWithoutValidation(key, values);
            //        }
            //    }
            //}
            return message;
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;                
            }
            base.Dispose(disposing);
        }

        private void GetRequestStreamCallback(IAsyncResult ar)
        {
            Action<Task> continuation = null;
            RequestState state = ar.AsyncState as RequestState;
            try
            {                
                Stream stream = state.webRequest.EndGetRequestStream(ar);
                state.requestStream = stream;
                if (continuation == null)
                {
                    continuation = delegate (Task task) {
                        try
                        {
                            if (task.IsFaulted)
                            {
                                this.HandleAsyncException(state, task.Exception.GetBaseException());
                            }
                            else if (task.IsCanceled)
                            {
                                state.tcs.TrySetCanceled();
                            }
                            else
                            {
                                state.requestStream.Close();
                                this.StartGettingResponse(state);
                            }
                        }
                        catch (Exception exception)
                        {
                            this.HandleAsyncException(state, exception);
                        }
                    };
                }
                state.requestMessage.Content.CopyToAsync(stream).ContinueWithStandard(continuation);
            }
            catch (Exception exception)
            {
                this.HandleAsyncException(state, exception);
            }
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            RequestState asyncState = ar.AsyncState as RequestState;
            try
            {
                HttpWebResponse webResponse = asyncState.webRequest.EndGetResponse(ar) as HttpWebResponse;
                asyncState.tcs.TrySetResult(this.CreateResponseMessage(webResponse, asyncState.requestMessage));
            }
            catch (Exception exception)
            {
                this.HandleAsyncException(asyncState, exception);
            }
        }

        private void HandleAsyncException(RequestState state, Exception e)
        {
            HttpResponseMessage message;
            if (Logging.On)
            {
                Logging.Exception(Logging.Http, this, "SendAsync", e);
            }
            if (this.TryGetExceptionResponse(e as WebException, state.requestMessage, out message))
            {
                state.tcs.TrySetResult(message);
            }
            else if (state.cancellationToken.IsCancellationRequested)
            {
                state.tcs.TrySetCanceled();
            }
            else if ((e is WebException) || (e is IOException))
            {
                state.tcs.TrySetException(new HttpRequestException(SR.net_http_client_execution_error, e));
            }
            else
            {
                state.tcs.TrySetException(e);
            }
        }

        internal virtual void InitializeWebRequest(HttpRequestMessage request, HttpWebRequest webRequest)
        {
        }

        private static void OnCancel(object state)
        {
            (state as HttpWebRequest).Abort();
        }

        private void PrepareAndStartContentUpload(RequestState state)
        {
            HttpContent requestContent = state.requestMessage.Content;
            try
            {
                if (state.requestMessage.Headers.TransferEncodingChunked == true)
                {
                    // SL changes
                    //state.webRequest.SendChunked = true;
                    this.StartGettingRequestStream(state);
                }
                else
                {
                    Action<Task> continuation = null;
                    long? contentLength = requestContent.Headers.ContentLength;
                    if (contentLength.HasValue)
                    {
                        state.webRequest.ContentLength = contentLength.Value;
                        this.StartGettingRequestStream(state);
                    }
                    else
                    {
                        if (this.maxRequestContentBufferSize == 0)
                        {
                            throw new HttpRequestException(SR.net_http_handler_nocontentlength);
                        }
                        if (continuation == null)
                        {
                            continuation = delegate (Task task) {
                                if (task.IsFaulted)
                                {
                                    this.HandleAsyncException(state, task.Exception.GetBaseException());
                                }
                                else
                                {
                                    contentLength = requestContent.Headers.ContentLength;
                                    state.webRequest.ContentLength = contentLength.Value;
                                    this.StartGettingRequestStream(state);
                                }
                            };
                        }
                        requestContent.LoadIntoBufferAsync(this.maxRequestContentBufferSize).ContinueWithStandard(continuation);
                    }
                }
            }
            catch (Exception exception)
            {
                this.HandleAsyncException(state, exception);
            }
        }  

        /// <summary>Creates an instance of  <see cref="T:NMasters.Silverlight.Net.Http.HttpResponseMessage" /> based on the information provided in the <see cref="T:NMasters.Silverlight.Net.Http.HttpRequestMessage" /> as an operation that will not block.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request", SR.net_http_handler_norequest);
            }
            this.CheckDisposed();
            if (Logging.On)
            {
                Logging.Enter(Logging.Http, this, "SendAsync", request);
            }
            this.SetOperationStarted();
            TaskCompletionSource<HttpResponseMessage> source = new TaskCompletionSource<HttpResponseMessage>();
            RequestState state = new RequestState {
                tcs = source,
                cancellationToken = cancellationToken,
                requestMessage = request
            };
            
            try
            {
                HttpWebRequest request2 = this.CreateAndPrepareWebRequest(request);                

                state.webRequest = request2;
                cancellationToken.Register(OnCancel, request2);                
                Task.Factory.StartNew(StartRequest, state);
            }
            catch (Exception exception)
            {
                this.HandleAsyncException(state, exception);
            }
            if (Logging.On)
            {
                Logging.Exit(Logging.Http, this, "SendAsync", source.Task);
            }
            return source.Task;
        }    

        private static void SetContentHeader(HttpWebRequest webRequest, KeyValuePair<string, IEnumerable<string>> header)
        {
            if (string.Compare("Content-Type", header.Key, StringComparison.OrdinalIgnoreCase) == 0)
            {
                webRequest.ContentType = string.Join(", ", header.Value);
            }
            else
            {    
                // SL fixes
                //webRequest.Headers.Add(header.Key, string.Join(", ", header.Value));
            }
        }

        private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            if (request.Content != null)
            {
                if (request.Content.Headers.Contains("Content-Length"))
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> pair in request.Content.Headers)
                    {
                        if (string.Compare("Content-Length", pair.Key, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            SetContentHeader(webRequest, pair);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> pair2 in request.Content.Headers)
                    {
                        SetContentHeader(webRequest, pair2);
                    }
                }
            }
        }

        private void SetDefaultOptions(HttpWebRequest webRequest)
        {            
            //webRequest..Timeout = -1;
            //webRequest.AllowAutoRedirect = this.allowAutoRedirect;
            //webRequest.AutomaticDecompression = this.automaticDecompression;
            //webRequest.PreAuthenticate = this.preAuthenticate;
            if (this.useDefaultCredentials)
            {
                webRequest.UseDefaultCredentials = true;
            }
            else
            {
                webRequest.Credentials = this.credentials;
            }
            if (this.allowAutoRedirect)
            {
                //webRequest.MaximumAutomaticRedirections = this.maxAutomaticRedirections;
            }                       
            if (this.useCookies)
            {
                webRequest.CookieContainer = this.cookieContainer;
            }
        }

        private void SetOperationStarted()
        {
            if (!this.operationStarted)
            {
                this.operationStarted = true;
            }
        }

        private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            WebHeaderCollection webRequestHeaders = webRequest.Headers;
            HttpRequestHeaders headers = request.Headers;

            // Most headers are just added directly to HWR's internal headers collection. But there are some exceptions
            // requiring different handling.
            // The following bool vars are used to skip string comparison when not required: E.g. if the 'Host' header
            // was not set, we don't need to compare every header in the collection with 'Host' to make sure we don't
            // add it to HWR's header collection.
            bool isHostSet = headers.Contains(HttpKnownHeaderNames.Host);
            bool isExpectSet = headers.Contains(HttpKnownHeaderNames.Expect);
            bool isTransferEncodingSet = headers.Contains(HttpKnownHeaderNames.TransferEncoding);
            bool isConnectionSet = headers.Contains(HttpKnownHeaderNames.Connection);
#if NET_4
            bool isAcceptSet = headers.Contains(HttpKnownHeaderNames.Accept);
            bool isRangeSet = headers.Contains(HttpKnownHeaderNames.Range);
            bool isRefererSet = headers.Contains(HttpKnownHeaderNames.Referer);
            bool isUserAgentSet = headers.Contains(HttpKnownHeaderNames.UserAgent);
 
            if (isRangeSet)
            {
                RangeHeaderValue range = headers.Range;
                if (range != null)
                {
                    foreach (var rangeItem in range.Ranges)
                    {
                        webRequest.AddRange((long)rangeItem.From, (long)rangeItem.To);
                    }
                }
            }
 
            if (isRefererSet)
            {
                Uri referer = headers.Referrer;
                if (referer != null)
                {
                    webRequest.Referer = referer.OriginalString;
                }
            }
 
            if (isAcceptSet && (headers.Accept.Count > 0))
            {
                webRequest.Accept = headers.Accept.ToString();
            }
 
            if (isUserAgentSet && headers.UserAgent.Count > 0)
            {
                webRequest.UserAgent = headers.UserAgent.ToString();
            }
#endif
            if (isHostSet)
            {
                string host = headers.Host;
                if (host != null)
                {
                    //webRequest.Host = host;
                }
            }

            // The following headers (Expect, Transfer-Encoding, Connection) have both a collection property and a 
            // bool property indicating a special value. Internally (in HttpHeaders) we don't distinguish between 
            // "special" values and other values. So we must make sure that we add all but the special value to HWR.
            // E.g. the 'Transfer-Encoding: chunked' value must be set using HWR.SendChunked, whereas all other values
            // can be added to the 'Transfer-Encoding'.
            if (isExpectSet)
            {
                string expectHeader = headers.Expect.GetHeaderStringWithoutSpecial();
                // Was at least one non-special value set?
                if (!String.IsNullOrEmpty(expectHeader) || !headers.Expect.IsSpecialValueSet)
                {
#if NET_4
                    AddHeaderWithoutValidate(webRequestHeaders, HttpKnownHeaderNames.Expect, expectHeader);
#else
                    //webRequestHeaders.AddInternal(HttpKnownHeaderNames.Expect, expectHeader);
#endif
                }
            }

            if (isTransferEncodingSet)
            {
                string transferEncodingHeader = headers.TransferEncoding.GetHeaderStringWithoutSpecial();
                // Was at least one non-special value set?
                if (!String.IsNullOrEmpty(transferEncodingHeader) || !headers.TransferEncoding.IsSpecialValueSet)
                {
#if NET_4
                    AddHeaderWithoutValidate(webRequestHeaders, HttpKnownHeaderNames.TransferEncoding, transferEncodingHeader);
#else
                    //webRequestHeaders.AddInternal(HttpKnownHeaderNames.TransferEncoding, transferEncodingHeader);
#endif
                }
            }

            if (isConnectionSet)
            {
                string connectionHeader = headers.Connection.GetHeaderStringWithoutSpecial();
                // Was at least one non-special value set?
                if (!String.IsNullOrEmpty(connectionHeader) || !headers.Connection.IsSpecialValueSet)
                {
#if NET_4
                    AddHeaderWithoutValidate(webRequestHeaders, HttpKnownHeaderNames.Connection, connectionHeader);
#else
                    //webRequestHeaders.AddInternal(HttpKnownHeaderNames.Connection, connectionHeader);
#endif
                }
            }

            foreach (var header in request.Headers.GetHeaderStrings())
            {
                string headerName = header.Key;

                if ((isHostSet && AreEqual(HttpKnownHeaderNames.Host, headerName)) ||
                    (isExpectSet && AreEqual(HttpKnownHeaderNames.Expect, headerName)) ||
                    (isTransferEncodingSet && AreEqual(HttpKnownHeaderNames.TransferEncoding, headerName)) ||
#if NET_4
                    (isAcceptSet && AreEqual(HttpKnownHeaderNames.Accept, headerName)) ||
                    (isRangeSet && AreEqual(HttpKnownHeaderNames.Range, headerName)) ||
                    (isRefererSet && AreEqual(HttpKnownHeaderNames.Referer, headerName)) ||
                    (isUserAgentSet) && AreEqual(HttpKnownHeaderNames.UserAgent, headerName) ||
#endif
 (isConnectionSet && AreEqual(HttpKnownHeaderNames.Connection, headerName)))
                {
                    continue; // Header was already added.
                }

                // Use AddInternal() to skip validation.
#if NET_4
                AddHeaderWithoutValidate(webRequestHeaders, header.Key, header.Value);
#else
                //webRequestHeaders.AddInternal(header.Key, header.Value);
#endif
            }
        }

        private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request)
        {
            HttpRequestHeaders headers = request.Headers;
            bool? expectContinue = headers.ExpectContinue;
            if (expectContinue.HasValue)
            {
                //webRequest.ServicePoint.Expect100Continue = expectContinue.Value;
            }
        }

        private void StartGettingRequestStream(RequestState state)
        {
            // SL fixes
            //if (state.identity != null)
            //{
            //    using (state.identity.Impersonate())
            //    {
            //        state.webRequest.BeginGetRequestStream(this.getRequestStreamCallback, state);
            //        return;
            //    }
            //}
            state.webRequest.BeginGetRequestStream(GetRequestStreamCallback, state);
        }

        private void StartGettingResponse(RequestState state)
        {
            // SL fixes
            //if (state.identity != null)
            //{
            //    using (state.identity.Impersonate())
            //    {
            //        state.webRequest.BeginGetResponse(this.getResponseCallback, state);
            //        return;
            //    }
            //}
            state.webRequest.BeginGetResponse(GetResponseCallback, state);
        }

        private void StartRequest(object obj)
        {
            var state = obj as RequestState;
            try
            {
                if (state.requestMessage.Content != null)
                {
                    this.PrepareAndStartContentUpload(state);
                }
                else
                {
                    state.webRequest.ContentLength = 0;
                    this.StartGettingResponse(state);
                }
            }
            catch (Exception exception)
            {
                this.HandleAsyncException(state, exception);
            }
        }

        private bool TryGetExceptionResponse(WebException webException, HttpRequestMessage requestMessage, out HttpResponseMessage httpResponseMessage)
        {
            if ((webException != null) && (webException.Response != null))
            {
                HttpWebResponse webResponse = webException.Response as HttpWebResponse;
                if (webResponse != null)
                {
                    httpResponseMessage = this.CreateResponseMessage(webResponse, requestMessage);
                    return true;
                }
            }
            httpResponseMessage = null;
            return false;
        }

        /// <summary>Gets or sets a value that indicates whether the handler should follow redirection responses.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler should follow redirection responses; otherwise false. The default value is true.</returns>
        public bool AllowAutoRedirect
        {
            get
            {
                return this.allowAutoRedirect;
            }
            set
            {
                this.CheckDisposedOrStarted();
                this.allowAutoRedirect = value;
            }
        }
        
        /// <summary>Gets or sets the collection of security certificates that are associated with this handler.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.ClientCertificateOption" />.The collection of security certificates associated with this handler.</returns>
        public ClientCertificateOption ClientCertificateOptions
        {
            get
            {
                return this.clientCertOptions;
            }
            set
            {
                if ((value != ClientCertificateOption.Manual) && (value != ClientCertificateOption.Automatic))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.CheckDisposedOrStarted();
                this.clientCertOptions = value;
            }
        }

        /// <summary>Gets or sets the cookie container used to store server cookies by the handler.</summary>
        /// <returns>Returns <see cref="T:System.Net.CookieContainer" />.The cookie container used to store server cookies by the handler.</returns>
        public System.Net.CookieContainer CookieContainer
        {
            get
            {
                return this.cookieContainer;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!this.UseCookies)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, SR.net_http_invalid_enable_first, new object[] { "UseCookies", "true" }));
                }
                this.CheckDisposedOrStarted();
                this.cookieContainer = value;
            }
        }

        /// <summary>Gets or sets authentication information used by this handler.</summary>
        /// <returns>Returns <see cref="T:System.Net.ICredentials" />.The authentication credentials associated with the handler. The default is null.</returns>
        public ICredentials Credentials
        {
            get
            {
                return this.credentials;
            }
            set
            {
                this.CheckDisposedOrStarted();
                this.credentials = value;
            }
        }

        /// <summary>Gets or sets the maximum number of redirects that the handler follows.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.The maximum number of redirection responses that the handler follows. The default value is 50.</returns>
        public int MaxAutomaticRedirections
        {
            get
            {
                return this.maxAutomaticRedirections;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.CheckDisposedOrStarted();
                this.maxAutomaticRedirections = value;
            }
        }

        /// <summary>Gets or sets the maximum request content buffer size used by the handler.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.The maximum request content buffer size in bytes. The default value is 65,536 bytes.</returns>
        public long MaxRequestContentBufferSize
        {
            get
            {
                return this.maxRequestContentBufferSize;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value > 0x7fffffff)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, new object[] { (long) 0x7fffffff }));
                }
                this.CheckDisposedOrStarted();
                this.maxRequestContentBufferSize = value;
            }
        }

        /// <summary>Gets or sets a value that indicates whether the handler sends an Authorization header with the request.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true for the handler to send an HTTP Authorization header with requests after authentication has taken place; otherwise, false. The default is false.</returns>
        public bool PreAuthenticate
        {
            get
            {
                return this.preAuthenticate;
            }
            set
            {
                this.CheckDisposedOrStarted();
                this.preAuthenticate = value;
            }
        }    

        /// <summary>Gets a value that indicates whether the handler supports automatic response content decompression.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports automatic response content decompression; otherwise false. The default value is true.</returns>
        public virtual bool SupportsAutomaticDecompression
        {
            get
            {
                return true;
            }
        }

        /// <summary>Gets a value that indicates whether the handler supports proxy settings.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports proxy settings; otherwise false. The default value is true.</returns>
        public virtual bool SupportsProxy
        {
            get
            {
                return true;
            }
        }

        /// <summary>Gets a value that indicates whether the handler supports configuration settings for the <see cref="P:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler.MaxAutomaticRedirections" /> properties.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports configuration settings for the <see cref="P:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler.MaxAutomaticRedirections" /> properties; otherwise false. The default value is true.</returns>
        public virtual bool SupportsRedirectConfiguration
        {
            get
            {
                return true;
            }
        }

        /// <summary>Gets or sets a value that indicates whether the handler uses the  <see cref="P:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the if the handler supports uses the  <see cref="P:NMasters.Silverlight.Net.Http.Handlers.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests; otherwise false. The default value is true.</returns>
        public bool UseCookies
        {
            get
            {
                return this.useCookies;
            }
            set
            {
                this.CheckDisposedOrStarted();
                this.useCookies = value;
            }
        }

        /// <summary>Gets or sets a value that controls whether default credentials are sent with requests by the handler.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the default credentials are used; otherwise false. The default value is false.</returns>
        public bool UseDefaultCredentials
        {
            get
            {
                return this.useDefaultCredentials;
            }
            set
            {
                this.CheckDisposedOrStarted();
                this.useDefaultCredentials = value;
            }
        }
     
        private class RequestState
        {
            internal CancellationToken cancellationToken;            
            internal HttpRequestMessage requestMessage;
            internal Stream requestStream;
            internal TaskCompletionSource<HttpResponseMessage> tcs;
            internal HttpWebRequest webRequest;
        }

        private class WebExceptionWrapperStream : DelegatingStream
        {
            internal WebExceptionWrapperStream(Stream innerStream) : base(innerStream)
            {
            }

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                IAsyncResult result;
                try
                {
                    result = base.BeginRead(buffer, offset, count, callback, state);
                }
                catch (WebException exception)
                {
                    throw new IOException(SR.net_http_read_error, exception);
                }
                return result;
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                int num;
                try
                {
                    num = base.EndRead(asyncResult);
                }
                catch (WebException exception)
                {
                    throw new IOException(SR.net_http_read_error, exception);
                }
                return num;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int num;
                try
                {
                    num = base.Read(buffer, offset, count);
                }
                catch (WebException exception)
                {
                    throw new IOException(SR.net_http_read_error, exception);
                }
                return num;
            }

            public override int ReadByte()
            {
                int num;
                try
                {
                    num = base.ReadByte();
                }
                catch (WebException exception)
                {
                    throw new IOException(SR.net_http_read_error, exception);
                }
                return num;
            }
        }
    }
}

