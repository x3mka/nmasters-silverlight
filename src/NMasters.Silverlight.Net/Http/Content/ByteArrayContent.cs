using System;
using System.IO;
using System.Threading.Tasks;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>Provides HTTP content based on a byte array.</summary>
    public class ByteArrayContent : HttpContent
    {
        private byte[] content;
        private int count;
        private int offset;

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> parameter is null. </exception>
        public ByteArrayContent(byte[] content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            this.content = content;
            this.offset = 0;
            this.count = content.Length;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" />.</param>
        /// <param name="offset">The offset, in bytes, in the <paramref name="content" />  parameter used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" />.</param>
        /// <param name="count">The number of bytes in the <paramref name="content" /> starting from the <paramref name="offset" /> parameter used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> parameter is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> parameter is less than zero.-or-The <paramref name="offset" /> parameter is greater than the length of content specified by the <paramref name="content" /> parameter.-or-The <paramref name="count " /> parameter is less than zero.-or-The <paramref name="count" /> parameter is greater than the length of content specified by the <paramref name="content" /> parameter - minus the <paramref name="offset" /> parameter.</exception>
        public ByteArrayContent(byte[] content, int offset, int count)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if ((offset < 0) || (offset > content.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || (count > (content.Length - offset)))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            this.content = content;
            this.offset = offset;
            this.count = count;
        }

        /// <summary>Creates an HTTP content stream as an asynchronous operation for reading whose backing store is memory from the <see cref="T:NMasters.Silverlight.Net.Http.Content.ByteArrayContent" />.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />.The task object representing the asynchronous operation.</returns>
        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            TaskCompletionSource<Stream> source = new TaskCompletionSource<Stream>();
            source.TrySetResult(new MemoryStream(this.content, this.offset, this.count, false, false));
            return source.Task;
        }

        /// <summary>Serialize and write the byte array provided in the constructor to an HTTP content stream as an asynchronous operation.</summary>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task" />. The task object representing the asynchronous operation.</returns>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport, like channel binding token. This parameter may be null.</param>
        protected override Task SerializeToStreamAsync(Stream stream)//, TransportContext context)
        {
            return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, content, offset, count, null);
        }

        /// <summary>Determines whether a byte array has a valid length in bytes.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.</returns>
        /// <param name="length">The length in bytes of the byte array.</param>
        protected internal override bool TryComputeLength(out long length)
        {
            length = this.count;
            return true;
        }
    }
}

