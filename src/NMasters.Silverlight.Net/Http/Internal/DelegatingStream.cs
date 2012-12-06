using System;
using System.IO;

namespace NMasters.Silverlight.Net.Http.Internal
{
    internal abstract class DelegatingStream : Stream
    {
        private Stream innerStream;

        protected DelegatingStream(Stream innerStream)
        {
            this.innerStream = innerStream;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerStream.Dispose();
            }
            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.innerStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.innerStream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            this.innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.innerStream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return this.innerStream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.innerStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            this.innerStream.WriteByte(value);
        }

        public override bool CanRead
        {
            get
            {
                return this.innerStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.innerStream.CanSeek;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return this.innerStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.innerStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return this.innerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.innerStream.Position;
            }
            set
            {
                this.innerStream.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return this.innerStream.ReadTimeout;
            }
            set
            {
                this.innerStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return this.innerStream.WriteTimeout;
            }
            set
            {
                this.innerStream.WriteTimeout = value;
            }
        }
    }
}

