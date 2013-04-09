// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/PartialStreamEx.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of libWyvernzora.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
// of the Software, and to permit persons to whom the Software is furnished to do 
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Diagnostics;
using System.IO;
using libWyvernzora.Core;
using libWyvernzora.Utilities;

namespace libWyvernzora.IO
{
    /// <summary>
    ///     Partial StringEx.
    ///     References a "substream" of another stream and behaves just like
    ///     a complete stream instance. Supports concurrent reading of the same
    ///     base StreamEx via multiple instances of PartialStreamEx on multiple
    ///     threads.
    /// </summary>
    public sealed class PartialStreamEx : StreamEx
    {
        private readonly FileAccess access;
        private readonly Int64 length;
        private readonly Int64 start;

        private Int64 position;
        private new StreamEx stream;


        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="stream">
        ///     Base stream for this PartialStreamEx.
        /// </param>
        /// <param name="start">Starting position of the substream to reference.</param>
        /// <param name="length">Length of the substream to reference.</param>
        /// <param name="access">Access rights to the substream.</param>
        public PartialStreamEx(StreamEx stream, Int64 start, Int64 length, FileAccess access = FileAccess.ReadWrite)
        {
            if (start < 0) throw new ArgumentOutOfRangeException("start");
            if (length < 0) throw new ArgumentOutOfRangeException("length");

            this.stream = stream;
            this.start = start;
            this.length = length;
            this.access = access;
        }

        /// <summary>
        ///     Utility method.
        ///     Makes sure base stream position corresponds to
        ///     the position of the PartialStreamEx.
        /// </summary>
        private void UpdatePosition()
        {
            // No validation here, everything done by caller.
            stream.Position = start + position;
        }

        #region Overrides

        /// <summary>
        ///     Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return (stream.CanRead && (access & FileAccess.Read) != 0); }
        }

        /// <summary>
        ///     Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return (stream.CanWrite && (access & FileAccess.Write) != 0); }
        }

        /// <summary>
        ///     Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        /// <summary>
        ///     Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return length; }
        }

        /// <summary>
        ///     Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get { return position; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Position value cannot be negative!");

                position = value;
                UpdatePosition();
            }
        }

        /// <summary>
        ///     Reads a sequence of bytes from the current stream and advances the
        ///     position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the
        ///     specified byte array with the values between offset and (offset + count - 1)
        ///     replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data
        ///     read from the current stream.
        /// </param>
        /// <param name="count">
        ///     The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <returns>
        ///     The total number of bytes read into the buffer. This can be less than the number
        ///     of bytes requested if that many bytes are not currently available, or zero (0) if
        ///     the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead) throw new InvalidOperationException("Cannot read from this stream!");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (count > buffer.Length) throw new ArgumentOutOfRangeException("count");
            if (offset + count > buffer.Length) throw new ArgumentOutOfRangeException("offset");

            lock (stream.Mutex)
            {
                using (new ActionLock(UpdatePosition, UpdatePosition))
                {
                    count = NumericOps.Min(count, (Int32) (length - position));
                    stream.Read(buffer, offset, count);
                    position += count;
                }
            }

            return count;
        }

        /// <summary>
        ///     writes a sequence of bytes to the current stream and advances the
        ///     current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. This method copies count bytes from buffer to
        ///     the current stream.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin copying
        ///     bytes to the current stream.
        /// </param>
        /// <param name="count">
        ///     The number of bytes to be written to the current stream.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((access & FileAccess.Write) == 0)
                throw new InvalidOperationException("Cannot write to this stream!");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            if (count > buffer.Length) throw new ArgumentOutOfRangeException("count");
            if (offset + count > buffer.Length) throw new ArgumentOutOfRangeException("offset");
            if (position + count > length) throw new NotSupportedException("Cannot write beyond this stream!");

            lock (stream.Mutex)
            {
                using (new ActionLock(UpdatePosition, UpdatePosition))
                {
                    stream.Write(buffer, offset, count);
                    position += count;
                }
            }
        }

        /// <summary>
        ///     Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">
        ///     A value of type SeekOrigin indicating the reference point used to
        ///     obtain the new position.
        /// </param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            Int64 temp = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    temp = offset;
                    break;
                case SeekOrigin.Current:
                    temp = position + offset;
                    break;
                case SeekOrigin.End:
                    temp = length - offset;
                    break;
            }

            if (temp < 0) throw new ArgumentOutOfRangeException("offset");

            lock (stream.Mutex)
            {
                using (new ActionLock(UpdatePosition, UpdatePosition))
                {
                    position = temp;
                }
            }

            return position;
        }

        /// <summary>
        ///     Sets the length of the current stream.
        ///     Not supported in PartialStreamEx.
        /// </summary>
        /// <param name="value">
        ///     The desired length of the current stream in bytes.
        /// </param>
        [Obsolete]
        [DebuggerHidden]
        public override void SetLength(long value)
        {
            throw new NotSupportedException(
                "PartialStreamEx is a fixed-length stream! Consider creating a new instance with different parameters.");
        }

        /// <summary>
        ///     Clears all buffers for this stream and causes any buffered
        ///     data to be written to the underlying device.
        /// </summary>
        [DebuggerHidden]
        public override void Flush()
        {
            stream.Flush();
        }

        #endregion

        #region IDisposable

        private Boolean closed;
        private Boolean disposed;

        public override void Close()
        {
            if (closed) throw new InvalidOperationException("Stream already closed!");
            Dispose();
            closed = true;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposed) return;
            disposed = true;
            if (disposing)
            {
                // Dispose Managed Resources Here
                stream = null; // Partial StreamEx does not dispose its base stream!
            }

            // Dispose Unmanaged Resources Here
        }

        #endregion
    }
}