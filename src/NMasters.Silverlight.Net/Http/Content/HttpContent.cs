using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>A base class representing an HTTP entity body and content headers.</summary>
    public abstract class HttpContent : IDisposable
    {
        private MemoryStream bufferedContent;
        private bool canCalculateLength;
        private Stream contentReadStream;
        internal static readonly Encoding DefaultStringEncoding = Encoding.UTF8;
        private bool disposed;
        private static Encoding[] EncodingsWithBom = new Encoding[] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode };
        private HttpContentHeaders headers;
        internal const long MaxBufferSize = 0x7fffffffL;

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.HttpContent" /> class.</summary>
        protected HttpContent()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Http, this, ".ctor", (string) null);
            }
            this.canCalculateLength = true;
            if (Logging.On)
            {
                Logging.Exit(Logging.Http, this, ".ctor", (string) null);
            }
        }

        private static bool ByteArrayHasPrefix(byte[] byteArray, int dataLength, byte[] prefix)
        {
            if (((prefix == null) || (byteArray == null)) || ((prefix.Length > dataLength) || (prefix.Length == 0)))
            {
                return false;
            }
            for (int i = 0; i < prefix.Length; i++)
            {
                if (prefix[i] != byteArray[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        private void CheckTaskNotNull(Task task)
        {
            if (task == null)
            {
                if (Logging.On)
                {
                    Logging.PrintError(Logging.Http, string.Format(CultureInfo.InvariantCulture, SR.net_http_log_content_no_task_returned_copytoasync, new object[] { base.GetType().FullName }));
                }
                throw new InvalidOperationException(SR.net_http_content_no_task_returned);
            }
        }

        internal void CopyTo(Stream stream)
        {
            this.CopyToAsync(stream).Wait();
        }     

        /// <summary>Write the HTTP content to a stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        public Task CopyToAsync(Stream stream)
        {
            Action<Task> continuation = null;
            this.CheckDisposed();
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            try
            {
                Task task = null;
                if (this.IsBuffered)
                {
                    task = Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, this.bufferedContent.GetBuffer(), 0, (int) this.bufferedContent.Length, null);
                }
                else
                {
                    task = this.SerializeToStreamAsync(stream);
                    this.CheckTaskNotNull(task);
                }
                if (continuation == null)
                {
                    continuation = delegate (Task copyTask) {
                        if (copyTask.IsFaulted)
                        {
                            tcs.TrySetException(GetStreamCopyException(copyTask.Exception.GetBaseException()));
                        }
                        else if (copyTask.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            tcs.TrySetResult(null);
                        }
                    };
                }
                task.ContinueWithStandard(continuation);
            }
            catch (IOException exception)
            {
                tcs.TrySetException(GetStreamCopyException(exception));
            }
            catch (ObjectDisposedException exception2)
            {
                tcs.TrySetException(GetStreamCopyException(exception2));
            }
            return tcs.Task;
        }

        private static Task CreateCompletedTask()
        {
            TaskCompletionSource<object> source = new TaskCompletionSource<object>();
            source.TrySetResult(null);
            return source.Task;
        }

        /// <summary>Write the HTTP content to a memory stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        protected virtual Task<Stream> CreateContentReadStreamAsync()
        {
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
            this.LoadIntoBufferAsync().ContinueWithStandard(delegate (Task task) {
                if (!HttpUtilities.HandleFaultsAndCancelation<Stream>(task, tcs))
                {
                    tcs.TrySetResult(this.bufferedContent);
                }
            });
            return tcs.Task;
        }

        private MemoryStream CreateMemoryStream(long maxBufferSize, out Exception error)
        {
            error = null;
            long? contentLength = this.Headers.ContentLength;
            if (!contentLength.HasValue)
            {
                return new LimitMemoryStream((int) maxBufferSize, 0);
            }
            long? nullable2 = contentLength;
            long num = maxBufferSize;
            if ((nullable2.GetValueOrDefault() > num) && nullable2.HasValue)
            {
                error = new HttpRequestException(string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, new object[] { maxBufferSize }));
                return null;
            }
            return new LimitMemoryStream((int) maxBufferSize, (int) contentLength.Value);
        }

        /// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Content.HttpContent" />.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Content.HttpContent" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                if (this.contentReadStream != null)
                {
                    this.contentReadStream.Dispose();
                }
                if (this.IsBuffered)
                {
                    this.bufferedContent.Dispose();
                }
            }
        }

        private long? GetComputedOrBufferLength()
        {
            this.CheckDisposed();
            if (this.IsBuffered)
            {
                return new long?(this.bufferedContent.Length);
            }
            if (this.canCalculateLength)
            {
                long length = 0;
                if (this.TryComputeLength(out length))
                {
                    return new long?(length);
                }
                this.canCalculateLength = false;
            }
            return null;
        }

        private static Exception GetStreamCopyException(Exception originalException)
        {
            Exception inner = originalException;
            if ((inner is IOException) || (inner is ObjectDisposedException))
            {
                return new HttpRequestException(SR.net_http_content_stream_copy_error, inner);
            }
            return inner;
        }

        /// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
        public Task LoadIntoBufferAsync()
        {
            return this.LoadIntoBufferAsync(0x7fffffff);
        }

        /// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
        /// <param name="maxBufferSize">The maximum size, in bytes, of the buffer to use.</param>
        public Task LoadIntoBufferAsync(long maxBufferSize)
        {
            Action<Task> continuation = null;
            this.CheckDisposed();
            if (maxBufferSize > 0x7fffffff)
            {
                throw new ArgumentOutOfRangeException("maxBufferSize", string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, new object[] { (long) 0x7fffffff }));
            }
            if (this.IsBuffered)
            {
                return CreateCompletedTask();
            }
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Exception error = null;
            MemoryStream tempBuffer = this.CreateMemoryStream(maxBufferSize, out error);
            if (tempBuffer == null)
            {
                tcs.TrySetException(error);
            }
            else
            {
                try
                {
                    Task task = this.SerializeToStreamAsync(tempBuffer);
                    this.CheckTaskNotNull(task);
                    if (continuation == null)
                    {
                        continuation = delegate (Task copyTask) {
                            try
                            {
                                if (copyTask.IsFaulted)
                                {
                                    tempBuffer.Dispose();
                                    tcs.TrySetException(GetStreamCopyException(copyTask.Exception.GetBaseException()));
                                }
                                else if (copyTask.IsCanceled)
                                {
                                    tempBuffer.Dispose();
                                    tcs.TrySetCanceled();
                                }
                                else
                                {
                                    tempBuffer.Seek(0, SeekOrigin.Begin);
                                    this.bufferedContent = tempBuffer;
                                    tcs.TrySetResult(null);
                                }
                            }
                            catch (Exception exception)
                            {
                                tcs.TrySetException(exception);
                                if (Logging.On)
                                {
                                    Logging.Exception(Logging.Http, this, "LoadIntoBufferAsync", exception);
                                }
                            }
                        };
                    }
                    task.ContinueWithStandard(continuation);
                }
                catch (IOException exception2)
                {
                    tcs.TrySetException(GetStreamCopyException(exception2));
                }
                catch (ObjectDisposedException exception3)
                {
                    tcs.TrySetException(GetStreamCopyException(exception3));
                }
            }
            return tcs.Task;
        }

        /// <summary>Write the HTTP content to a byte array as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        public Task<byte[]> ReadAsByteArrayAsync()
        {
            this.CheckDisposed();
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
            this.LoadIntoBufferAsync().ContinueWithStandard(delegate (Task task) {
                if (!HttpUtilities.HandleFaultsAndCancelation<byte[]>(task, tcs))
                {
                    tcs.TrySetResult(this.bufferedContent.ToArray());
                }
            });
            return tcs.Task;
        }

        /// <summary>Write the HTTP content to a stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        public Task<Stream> ReadAsStreamAsync()
        {
            this.CheckDisposed();
            TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
            if ((this.contentReadStream == null) && this.IsBuffered)
            {
                this.contentReadStream = new MemoryStream(this.bufferedContent.GetBuffer(), 0, (int) this.bufferedContent.Length, false, false);
            }
            if (this.contentReadStream != null)
            {
                tcs.TrySetResult(this.contentReadStream);
                return tcs.Task;
            }
            this.CreateContentReadStreamAsync().ContinueWithStandard<Stream>(delegate (Task<Stream> task) {
                if (!HttpUtilities.HandleFaultsAndCancelation<Stream>(task, tcs))
                {
                    this.contentReadStream = task.Result;
                    tcs.TrySetResult(this.contentReadStream);
                }
            });
            return tcs.Task;
        }

        /// <summary>Write the HTTP content to a string as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        public Task<string> ReadAsStringAsync()
        {
            this.CheckDisposed();
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            this.LoadIntoBufferAsync().ContinueWithStandard(delegate (Task task) {
                if (!HttpUtilities.HandleFaultsAndCancelation<string>(task, tcs))
                {
                    if (this.bufferedContent.Length == 0)
                    {
                        tcs.TrySetResult(string.Empty);
                    }
                    else
                    {
                        Encoding encoding = null;
                        int length = -1;
                        byte[] buffer = this.bufferedContent.GetBuffer();
                        int dataLength = (int) this.bufferedContent.Length;
                        if ((this.Headers.ContentType != null) && (this.Headers.ContentType.CharSet != null))
                        {
                            try
                            {
                                encoding = Encoding.GetEncoding(this.Headers.ContentType.CharSet);
                            }
                            catch (ArgumentException exception)
                            {
                                tcs.TrySetException(new InvalidOperationException(SR.net_http_content_invalid_charset, exception));
                                return;
                            }
                        }
                        if (encoding == null)
                        {
                            foreach (Encoding encoding2 in EncodingsWithBom)
                            {
                                byte[] preamble = encoding2.GetPreamble();
                                if (ByteArrayHasPrefix(buffer, dataLength, preamble))
                                {
                                    encoding = encoding2;
                                    length = preamble.Length;
                                    break;
                                }
                            }
                        }
                        encoding = encoding ?? DefaultStringEncoding;
                        if (length == -1)
                        {
                            byte[] prefix = encoding.GetPreamble();
                            if (ByteArrayHasPrefix(buffer, dataLength, prefix))
                            {
                                length = prefix.Length;
                            }
                            else
                            {
                                length = 0;
                            }
                        }
                        try
                        {
                            string str = encoding.GetString(buffer, length, dataLength - length);
                            tcs.TrySetResult(str);
                        }
                        catch (Exception exception2)
                        {
                            tcs.TrySetException(exception2);
                        }
                    }
                }
            });
            return tcs.Task;
        }

        /// <summary>Serialize the HTTP content to a stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        protected abstract Task SerializeToStreamAsync(Stream stream);
        /// <summary>Determines whether the HTTP content has a valid length in bytes.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.</returns>
        /// <param name="length">The length in bytes of the HHTP content.</param>
        protected internal abstract bool TryComputeLength(out long length);

        /// <summary>Gets the HTTP content headers as defined in RFC 2616.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.HttpContentHeaders" />.The content headers as defined in RFC 2616.</returns>
        public HttpContentHeaders Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HttpContentHeaders(this.GetComputedOrBufferLength);
                }
                return this.headers;
            }
        }

        private bool IsBuffered
        {
            get
            {
                return (this.bufferedContent != null);
            }
        }

        private class LimitMemoryStream : MemoryStream
        {
            private int maxSize;

            public LimitMemoryStream(int maxSize, int capacity) : base(capacity)
            {
                this.maxSize = maxSize;
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                this.CheckSize(count);
                return base.BeginWrite(buffer, offset, count, callback, state);
            }

            private void CheckSize(int countToAdd)
            {
                if ((this.maxSize - this.Length) < countToAdd)
                {
                    throw new HttpRequestException(string.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_exceeded, new object[] { this.maxSize }));
                }
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                this.CheckSize(count);
                base.Write(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                this.CheckSize(1);
                base.WriteByte(value);
            }
        }
    }
}

