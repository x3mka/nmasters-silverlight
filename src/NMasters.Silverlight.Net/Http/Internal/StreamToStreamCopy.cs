using System;
using System.IO;
using System.Threading.Tasks;

namespace NMasters.Silverlight.Net.Http.Internal
{
    internal class StreamToStreamCopy
    {
        private byte[] buffer;
        private AsyncCallback bufferReadCallback;
        private int bufferSize;
        private AsyncCallback bufferWrittenCallback;
        private Stream destination;
        private bool destinationIsMemoryStream;
        private bool disposeSource;
        private Stream source;
        private bool sourceIsMemoryStream;
        private TaskCompletionSource<object> tcs;

        public StreamToStreamCopy(Stream source, Stream destination, int bufferSize, bool disposeSource)
        {
            this.buffer = new byte[bufferSize];
            this.source = source;
            this.destination = destination;
            this.sourceIsMemoryStream = source is MemoryStream;
            this.destinationIsMemoryStream = destination is MemoryStream;
            this.bufferSize = bufferSize;
            this.bufferReadCallback = new AsyncCallback(this.BufferReadCallback);
            this.bufferWrittenCallback = new AsyncCallback(this.BufferWrittenCallback);
            this.disposeSource = disposeSource;
            this.tcs = new TaskCompletionSource<object>();
        }

        private void BufferReadCallback(IAsyncResult ar)
        {
            if (!ar.CompletedSynchronously)
            {
                try
                {
                    int bytesRead = this.source.EndRead(ar);
                    if (bytesRead == 0)
                    {
                        this.SetCompleted(null);
                    }
                    else if (this.TryStartWriteSync(bytesRead))
                    {
                        this.StartRead();
                    }
                }
                catch (Exception exception)
                {
                    this.SetCompleted(exception);
                }
            }
        }

        private void BufferWrittenCallback(IAsyncResult ar)
        {
            if (!ar.CompletedSynchronously)
            {
                try
                {
                    this.destination.EndWrite(ar);
                    this.StartRead();
                }
                catch (Exception exception)
                {
                    this.SetCompleted(exception);
                }
            }
        }

        private void SetCompleted(Exception error)
        {
            try
            {
                if (this.disposeSource)
                {
                    this.source.Dispose();
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.Http, this, "SetCompleted", exception);
                }
            }
            if (error == null)
            {
                this.tcs.TrySetResult(null);
            }
            else
            {
                this.tcs.TrySetException(error);
            }
        }

        public Task StartAsync()
        {
            if (this.sourceIsMemoryStream && this.destinationIsMemoryStream)
            {
                MemoryStream source = this.source as MemoryStream;
                try
                {
                    int position = (int) source.Position;
                    this.destination.Write(source.ToArray(), position, ((int) this.source.Length) - position);
                    this.SetCompleted(null);
                }
                catch (Exception exception)
                {
                    this.SetCompleted(exception);
                }
            }
            else
            {
                this.StartRead();
            }
            return this.tcs.Task;
        }

        private void StartRead()
        {
            int bytesRead = 0;
            bool completedSynchronously = false;
            try
            {
                do
                {
                    if (this.sourceIsMemoryStream)
                    {
                        bytesRead = this.source.Read(this.buffer, 0, this.bufferSize);
                        if (bytesRead == 0)
                        {
                            this.SetCompleted(null);
                            return;
                        }
                        completedSynchronously = this.TryStartWriteSync(bytesRead);
                    }
                    else
                    {
                        IAsyncResult asyncResult = this.source.BeginRead(this.buffer, 0, this.bufferSize, this.bufferReadCallback, null);
                        completedSynchronously = asyncResult.CompletedSynchronously;
                        if (completedSynchronously)
                        {
                            bytesRead = this.source.EndRead(asyncResult);
                            if (bytesRead == 0)
                            {
                                this.SetCompleted(null);
                                return;
                            }
                            completedSynchronously = this.TryStartWriteSync(bytesRead);
                        }
                    }
                }
                while (completedSynchronously);
            }
            catch (Exception exception)
            {
                this.SetCompleted(exception);
            }
        }

        private bool TryStartWriteSync(int bytesRead)
        {
            if (this.destinationIsMemoryStream)
            {
                this.destination.Write(this.buffer, 0, bytesRead);
                return true;
            }
            IAsyncResult asyncResult = this.destination.BeginWrite(this.buffer, 0, bytesRead, this.bufferWrittenCallback, null);
            if (asyncResult.CompletedSynchronously)
            {
                this.destination.EndWrite(asyncResult);
                return true;
            }
            return false;
        }
    }
}

