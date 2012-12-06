using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Configuration;
using NMasters.Silverlight.Net.Http.Content;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Helpers;

namespace NMasters.Silverlight.Net.Http
{
    public static class HttpClientExtensions
    {
        #region Get Extensions
        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri)
        {
            return client.GetAsync(CreateUri(requestUri));
        }

        /// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, Uri requestUri)
        {
            return client.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri, HttpCompletionOption completionOption)
        {
            return client.GetAsync(CreateUri(requestUri), completionOption);
        }

        /// <summary>Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            return client.GetAsync(CreateUri(requestUri), cancellationToken);
        }

        /// <summary>Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, Uri requestUri, HttpCompletionOption completionOption)
        {
            return client.GetAsync(requestUri, completionOption, CancellationToken.None);
        }

        /// <summary>Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, Uri requestUri, CancellationToken cancellationToken)
        {
            return client.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        /// <summary>Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return client.GetAsync(CreateUri(requestUri), completionOption, cancellationToken);
        }

        /// <summary>Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> GetAsync(this HttpClient client, Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            return client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
        }

        #endregion

        #region Post Extensions
        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            return client.PostAsync(CreateUri(requestUri), content);
        }

        /// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, Uri requestUri, HttpContent content)
        {
            return client.PostAsync(requestUri, content, CancellationToken.None);
        }

        /// <summary>Send a POST request with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return client.PostAsync(CreateUri(requestUri), content, cancellationToken);
        }

        /// <summary>Send a POST request with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            return client.SendAsync(request, cancellationToken);
        }
        #endregion

        #region Put Extensions
        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PutAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            return client.PutAsync(CreateUri(requestUri), content);
        }

        /// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PutAsync(this HttpClient client, Uri requestUri, HttpContent content)
        {
            return client.PutAsync(requestUri, content, CancellationToken.None);
        }

        /// <summary>Send a PUT request with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PutAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            return client.PutAsync(CreateUri(requestUri), content, cancellationToken);
        }

        /// <summary>Send a PUT request with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> PutAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content };
            return client.SendAsync(request, cancellationToken);
        }
        #endregion

        #region Delete Extensions
        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri)
        {
            return client.DeleteAsync(CreateUri(requestUri));
        }

        /// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, Uri requestUri)
        {
            return client.DeleteAsync(requestUri, CancellationToken.None);
        }

        /// <summary>Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            return client.DeleteAsync(CreateUri(requestUri), cancellationToken);
        }

        /// <summary>Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<HttpResponseMessage> DeleteAsync(this HttpClient client, Uri requestUri, CancellationToken cancellationToken)
        {
            return client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
        }
        #endregion

        #region Send Extensions
        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="request">The HTTP request message to send.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> instance.</exception>
        public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request)
        {
            return client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }

        /// <summary>Send an HTTP request as an asynchronous operation. </summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> instance.</exception>
        public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            return client.SendAsync(request, completionOption, CancellationToken.None);
        }

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> instance.</exception>
        public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        #endregion

        #region Content Extensions

        /// <summary>Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<byte[]> GetByteArrayAsync(this HttpClient client, string requestUri)
        {
            return client.GetByteArrayAsync(CreateUri(requestUri));
        }

        /// <summary>Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<byte[]> GetByteArrayAsync(this HttpClient client, Uri requestUri)
        {
            return client.GetContentAsync(requestUri, HttpCompletionOption.ResponseContentRead, HttpUtilities.EmptyByteArray, content => content.ReadAsByteArrayAsync());
        }

        private static Task<T> GetContentAsync<T>(this HttpClient client, Uri requestUri, HttpCompletionOption completionOption, T defaultValue, Func<HttpContent, Task<T>> readAs)
        {
            var tcs = new TaskCompletionSource<T>();
            client.GetAsync(requestUri, completionOption).ContinueWithStandard(delegate(Task<HttpResponseMessage> requestTask)
            {
                if (!HandleRequestFaultsAndCancelation<T>(requestTask, tcs))
                {
                    HttpResponseMessage message = requestTask.Result;
                    if (message.Content == null)
                    {
                        tcs.TrySetResult(defaultValue);
                    }
                    else
                    {
                        try
                        {
                            Action<Task<T>> continuation = (contentTask) =>
                            {
                                if (!HttpUtilities.HandleFaultsAndCancelation<T>(contentTask, tcs))
                                {
                                    tcs.TrySetResult(contentTask.Result);
                                }
                            };
                            readAs.Invoke(message.Content).ContinueWithStandard<T>(continuation);
                        }
                        catch (Exception exception)
                        {
                            tcs.TrySetException(exception);
                        }
                    }
                }
            });
            return tcs.Task;
        }

        private static bool HandleRequestFaultsAndCancelation<T>(Task<HttpResponseMessage> task, TaskCompletionSource<T> tcs)
        {
            if (!HttpUtilities.HandleFaultsAndCancelation(task, tcs))
            {
                HttpResponseMessage message = task.Result;
                if (message.IsSuccessStatusCode)
                {
                    return false;
                }
                if (message.Content != null)
                {
                    message.Content.Dispose();
                }
                tcs.TrySetException(new HttpRequestException(string.Format(CultureInfo.InvariantCulture, SR.net_http_message_not_success_statuscode, new object[] { (int)message.StatusCode, message.ReasonPhrase })));
            }
            return true;
        }

        /// <summary>Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<Stream> GetStreamAsync(this HttpClient client, string requestUri)
        {
            return client.GetStreamAsync(CreateUri(requestUri));
        }

        /// <summary>Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<Stream> GetStreamAsync(this HttpClient client, Uri requestUri)
        {
            return client.GetContentAsync<Stream>(requestUri, HttpCompletionOption.ResponseHeadersRead, Stream.Null, content => content.ReadAsStreamAsync());
        }

        /// <summary>Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<string> GetStringAsync(this HttpClient client, string requestUri)
        {
            return client.GetStringAsync(CreateUri(requestUri));
        }

        /// <summary>Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        /// <param name="client"> </param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> was null.</exception>
        public static Task<string> GetStringAsync(this HttpClient client, Uri requestUri)
        {
            return client.GetContentAsync(requestUri, HttpCompletionOption.ResponseContentRead, string.Empty, content => content.ReadAsStringAsync());
        }

        #endregion

        private static Uri CreateUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }
            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }
    }
}