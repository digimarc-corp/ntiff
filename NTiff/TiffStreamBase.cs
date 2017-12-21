using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NTiff
{
    public abstract class TiffStreamBase : Stream, IDisposable
    {
        protected Stream _Stream;
        protected bool _IsWritable;
        public bool IsBigEndian { get; protected set; }

        /// <summary>
        /// If the active endianness conflicts with our architecture, reverse the entire byte array.
        /// </summary>
        /// <param name="bytes"></param>
        protected void CheckEndian(byte[] bytes)
        {
            if (IsBigEndian == BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
        }

        public byte[] ToArray()
        {
            _Stream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[_Stream.Length];
            _Stream.Read(bytes, 0, (int)_Stream.Length);
            return bytes;
        }

        #region abstracts inherited from Stream

        public override bool CanRead => _Stream.CanRead;

        public override bool CanSeek => _Stream.CanSeek;

        public override bool CanWrite => _IsWritable && _Stream.CanWrite;

        public override long Length => _Stream.Length;

        public override long Position { get => _Stream.Position; set => _Stream.Position = value; }

        public override void Flush()
        {
            _Stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _Stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            if (CanWrite) { _Stream.SetLength(value); }
            else { throw new IOException("Stream is in a read-only state."); }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite) { _Stream.Write(buffer, offset, count); }
            else { throw new IOException("Stream is not writable."); }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _Stream.Dispose();
                    _Stream = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
            base.Dispose(disposing);
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TiffStreamBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        #endregion
    }
}
