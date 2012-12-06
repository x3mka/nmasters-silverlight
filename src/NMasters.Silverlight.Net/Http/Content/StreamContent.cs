using System;
using System.IO;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>Provides HTTP content based on a stream.</summary>
    public class StreamContent : HttpContent
    {
        private int bufferSize;
        private Stream content;
        private bool contentConsumed;
        private const int defaultBufferSize = 0x1000;
        private long start;

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.StreamContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.StreamContent" />.</param>
        public StreamContent(Stream content) : this(content, 0x1000)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Net.Http.StreamContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.StreamContent" />.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer for the <see cref="T:System.Net.Http.StreamContent" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was null.</exception>
        /// <exception cref="T:System.OutOfRangeException">The <paramref name="bufferSize" /> was less than or equal to zero. </exception>
        public StreamContent(Stream content, int bufferSize)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }
            this.content = content;
            this.bufferSize = bufferSize;
            if (content.CanSeek)
            {
                this.start = content.Position;
            }
            if (Logging.On)
            {
                Logging.Associate(Logging.Http, this, content);
            }
        }

        /// <summary>Write the HTTP stream content to a memory stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            TaskCompletionSource<Stream> source = new TaskCompletionSource<Stream>();
            source.TrySetResult(new ReadOnlyStream(this.content));
            return source.Task;
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:NMasters.Silverlight.Net.Http.Content.StreamContent" /> and optionally disposes of the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.content.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PrepareContent()
        {
            if (this.contentConsumed)
            {
                if (!this.content.CanSeek)
                {
                    throw new InvalidOperationException(SR.net_http_content_stream_already_read);
                }
                this.content.Position = this.start;
            }
            this.contentConsumed = true;
        }

        /// <summary>Serialize the HTTP content to a stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.</returns>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        protected override Task SerializeToStreamAsync(Stream stream)
        {
            this.PrepareContent();
            StreamToStreamCopy copy = new StreamToStreamCopy(this.content, stream, this.bufferSize, !this.content.CanSeek);
            return copy.StartAsync();
        }

        /// <summary>Determines whether the stream content has a valid length in bytes.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.</returns>
        /// <param name="length">The length in bytes of the stream content.</param>
        protected internal override bool TryComputeLength(out long length)
        {
            if (this.content.CanSeek)
            {
                length = this.content.Length - this.start;
                return true;
            }
            length = 0;
            return false;
        }

        private class ReadOnlyStream : DelegatingStream
        {
            public ReadOnlyStream(Stream innerStream) : base(innerStream)
            {
            }

            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void Flush()
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void WriteByte(byte value)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            public override int WriteTimeout
            {
                get
                {
                    throw new NotSupportedException(SR.net_http_content_readonly_stream);
                }
                set
                {
                    throw new NotSupportedException(SR.net_http_content_readonly_stream);
                }
            }
        }
    }
}

