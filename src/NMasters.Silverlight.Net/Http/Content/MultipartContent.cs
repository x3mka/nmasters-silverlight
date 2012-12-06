using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>Provides a collection of <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification.</summary>
    public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
    {
        private string boundary;
        private const string crlf = "\r\n";
        private List<HttpContent> nestedContent;
        private int nextContentIndex;
        private Stream outputStream;
        private TaskCompletionSource<object> tcs;

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.MultipartContent" /> class.</summary>
        public MultipartContent() : this("mixed", GetDefaultBoundary())
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.MultipartContent" /> class.</summary>
        /// <param name="subtype">The subtype of the multipart content.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="subtype" /> was null or contains only white space characters.</exception>
        public MultipartContent(string subtype) : this(subtype, GetDefaultBoundary())
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartContent" /> class.</summary>
        /// <param name="subtype">The subtype of the multipart content.</param>
        /// <param name="boundary">The boundary string for the multipart content.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="subtype" /> was null or an empty string.The <paramref name="boundary" /> was null or contains only white space characters.-or-The <paramref name="boundary" /> ends with a space character.</exception>
        /// <exception cref="T:System.OutOfRangeException">The length of the <paramref name="boundary" /> was greater than 70.</exception>
        public MultipartContent(string subtype, string boundary)
        {
            if (string.IsNullOrWhiteSpace(subtype))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "subtype");
            }
            ValidateBoundary(boundary);
            this.boundary = boundary;
            string str = boundary;
            if (!str.StartsWith("\"", StringComparison.Ordinal))
            {
                str = "\"" + str + "\"";
            }
            MediaTypeHeaderValue value2 = new MediaTypeHeaderValue("multipart/" + subtype);
            value2.Parameters.Add(new NameValueHeaderValue("boundary", str));
            base.Headers.ContentType = value2;
            this.nestedContent = new List<HttpContent>();
        }

        /// <summary>Add multipart HTTP content to a collection of <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification.</summary>
        /// <param name="content">The HTTP content to add to the collection.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was null.</exception>
        public virtual void Add(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            this.nestedContent.Add(content);
        }

        private TaskCompletionSource<object> CleanupAsync()
        {
            TaskCompletionSource<object> tcs = this.tcs;
            this.outputStream = null;
            this.nextContentIndex = 0;
            this.tcs = null;
            return tcs;
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Content.MultipartContent" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (HttpContent content in this.nestedContent)
                {
                    content.Dispose();
                }
                this.nestedContent.Clear();
            }
            base.Dispose(disposing);
        }

        private static Task EncodeStringToStreamAsync(Stream stream, string input)
        {
            byte[] bytes = HttpRuleParser.DefaultHttpEncoding.GetBytes(input);
            return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, bytes, 0, bytes.Length, null);
        }

        private static string GetDefaultBoundary()
        {
            return Guid.NewGuid().ToString();
        }

        private static int GetEncodedLength(string input)
        {
            return HttpRuleParser.DefaultHttpEncoding.GetByteCount(input);
        }

        /// <summary>Returns an enumerator that iterates through the collection of <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification..</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerator`1" />.An object that can be used to iterate through the collection.</returns>
        public IEnumerator<HttpContent> GetEnumerator()
        {
            return this.nestedContent.GetEnumerator();
        }

        private void HandleAsyncException(string method, Exception ex)
        {
            if (Logging.On)
            {
                Logging.Exception(Logging.Http, this, method, ex);
            }
            this.CleanupAsync().TrySetException(ex);
        }

        /// <summary>Serialize the multipart HTTP content to a stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        protected override Task SerializeToStreamAsync(Stream stream)
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();
            this.tcs = source;
            this.outputStream = stream;
            this.nextContentIndex = 0;
            EncodeStringToStreamAsync(this.outputStream, "--" + this.boundary + "\r\n").ContinueWithStandard(new Action<Task>(this.WriteNextContentHeadersAsync));
            return source.Task;
        }

        /// <summary>The explicit implementation of the <see cref="M:NMasters.Silverlight.Net.Http.Content.MultipartContent.GetEnumerator" /> method.</summary>
        /// <returns>Returns <see cref="T:System.Collections.IEnumerator" />.An object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.nestedContent.GetEnumerator();
        }

        /// <summary>Determines whether the HTTP multipart content has a valid length in bytes.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.</returns>
        /// <param name="length">The length in bytes of the HHTP content.</param>
        protected internal override bool TryComputeLength(out long length)
        {
            long num = 0;
            long encodedLength = GetEncodedLength("\r\n--" + this.boundary + "\r\n");
            num += GetEncodedLength("--" + this.boundary + "\r\n");
            bool flag = true;
            foreach (HttpContent content in this.nestedContent)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    num += encodedLength;
                }
                foreach (KeyValuePair<string, IEnumerable<string>> pair in content.Headers)
                {
                    num += GetEncodedLength(pair.Key + ": " + string.Join(", ", pair.Value) + "\r\n");
                }
                num += "\r\n".Length;
                long num3 = 0;
                if (!content.TryComputeLength(out num3))
                {
                    length = 0;
                    return false;
                }
                num += num3;
            }
            num += GetEncodedLength("\r\n--" + this.boundary + "--\r\n");
            length = num;
            return true;
        }

        private static void ValidateBoundary(string boundary)
        {
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "boundary");
            }
            if (boundary.Length > 70)
            {
                throw new ArgumentOutOfRangeException("boundary", string.Format(CultureInfo.InvariantCulture, SR.net_http_content_field_too_long, new object[] { 70 }));
            }
            if (boundary.EndsWith(" ", StringComparison.Ordinal))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { boundary }), "boundary");
            }
            string str = "'()+_,-./:=? ";
            foreach (char ch in boundary)
            {
                if (((('0' > ch) || (ch > '9')) && (('a' > ch) || (ch > 'z'))) && ((('A' > ch) || (ch > 'Z')) && (str.IndexOf(ch) < 0)))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, new object[] { boundary }), "boundary");
                }
            }
        }

        private void WriteNextContentAsync(Task task)
        {
            if (task.IsFaulted)
            {
                this.HandleAsyncException("WriteNextContentAsync", task.Exception.GetBaseException());
            }
            else
            {
                try
                {
                    HttpContent content = this.nestedContent[this.nextContentIndex];
                    this.nextContentIndex++;
                    content.CopyToAsync(this.outputStream).ContinueWithStandard(new Action<Task>(this.WriteNextContentHeadersAsync));
                }
                catch (Exception exception)
                {
                    this.HandleAsyncException("WriteNextContentAsync", exception);
                }
            }
        }

        private void WriteNextContentHeadersAsync(Task task)
        {
            if (task.IsFaulted)
            {
                this.HandleAsyncException("WriteNextContentHeadersAsync", task.Exception.GetBaseException());
            }
            else
            {
                try
                {
                    if (this.nextContentIndex >= this.nestedContent.Count)
                    {
                        this.WriteTerminatingBoundaryAsync();
                    }
                    else
                    {
                        string str = "\r\n--" + this.boundary + "\r\n";
                        StringBuilder builder = new StringBuilder();
                        if (this.nextContentIndex != 0)
                        {
                            builder.Append(str);
                        }
                        HttpContent content = this.nestedContent[this.nextContentIndex];
                        foreach (KeyValuePair<string, IEnumerable<string>> pair in content.Headers)
                        {
                            builder.Append(pair.Key + ": " + string.Join(", ", pair.Value) + "\r\n");
                        }
                        builder.Append("\r\n");
                        EncodeStringToStreamAsync(this.outputStream, builder.ToString()).ContinueWithStandard(new Action<Task>(this.WriteNextContentAsync));
                    }
                }
                catch (Exception exception)
                {
                    this.HandleAsyncException("WriteNextContentHeadersAsync", exception);
                }
            }
        }

        private void WriteTerminatingBoundaryAsync()
        {
            Action<Task> continuation = null;
            try
            {
                if (continuation == null)
                {
                    continuation = delegate (Task task) {
                        if (task.IsFaulted)
                        {
                            this.HandleAsyncException("WriteTerminatingBoundaryAsync", task.Exception.GetBaseException());
                        }
                        else
                        {
                            this.CleanupAsync().TrySetResult(null);
                        }
                    };
                }
                EncodeStringToStreamAsync(this.outputStream, "\r\n--" + this.boundary + "--\r\n").ContinueWithStandard(continuation);
            }
            catch (Exception exception)
            {
                this.HandleAsyncException("WriteTerminatingBoundaryAsync", exception);
            }
        }
    }
}

