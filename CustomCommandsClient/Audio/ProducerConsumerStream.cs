using System;
using System.IO;

namespace CustomCommandsClient.Audio
{
    public class ProducerConsumerStream : Stream
    {
        private readonly MemoryStream innerStream = new MemoryStream();
        private readonly object lockable = new object();

        private bool disposed = false;
        private long readPosition = 0;
        private long writePosition = 0;

        public ProducerConsumerStream()
        {
        }

        ~ProducerConsumerStream()
        {
            Dispose(false);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get
            {
                lock (lockable)
                {
                    return innerStream.Length;
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (lockable)
                {
                    return innerStream.Position;
                }
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
            lock (lockable)
            {
                innerStream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (lockable)
            {
                innerStream.Position = readPosition;
                var red = innerStream.Read(buffer, offset, count);
                readPosition = innerStream.Position;

                return red;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            // Seek is for read only
            return readPosition;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (lockable)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer, offset, count);
                writePosition = innerStream.Position;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free managed objects help by this instance
                if (innerStream != null)
                {
                    innerStream.Dispose();
                }
            }

            // Free any unmanaged objects here.

            disposed = true;

            // Call the base class implementation.
            base.Dispose(disposing);
        }
    }
}
