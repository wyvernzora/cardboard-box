// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/StreamEx.cs
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
using System.IO;
using System.Text;
using libWyvernzora.Core;

namespace libWyvernzora.IO
{
    /// <summary>
    ///     Extended Stream Class.
    /// </summary>
    /// <remarks>
    ///     StreamEx is the unified stream for entire libWyvernzora.
    ///     It is a modified version of Firefly.Core.StreamEx. The key
    ///     difference is that StreamEx is fully compatible with
    ///     System.IO.Stream class without need of any adapters.
    /// </remarks>
    public class StreamEx : Stream
    {
        /// <summary>
        ///     Base stream wrapped by this StreamEx.
        /// </summary>
        protected Stream stream;

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="stream">Stream wrapped in this StreamEx.</param>
        public StreamEx(Stream stream) : this()
        {
            this.stream = stream;
        }

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="path">Path of the file to open.</param>
        /// <param name="fmode">File Mode.</param>
        /// <param name="faccess">File Access.</param>
        public StreamEx(String path, FileMode fmode = FileMode.Open, FileAccess faccess = FileAccess.ReadWrite) : this()
        {
            stream = new FileStream(path, fmode, faccess);
        }

        /// <summary>
        ///     Constructor.
        ///     Initializes an empty instance.
        ///     Only use in derived classes.
        /// </summary>
        protected StreamEx()
        {
            Mutex = new Object();
        }

        #region Virtual Properties

        /// <summary>
        ///     Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        /// <summary>
        ///     Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        /// <summary>
        ///     Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        /// <summary>
        ///     Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return stream.Length; }
        }

        /// <summary>
        ///     Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        #endregion

        #region Concrete Read/Write Methods

        /// <summary>
        ///     Reads an array of bytes from StreamEx.
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>Byte array containing bytes from stream.</returns>
        public Byte[] ReadBytes(Int32 count)
        {
            Byte[] temp = new Byte[count];
            Read(temp, 0, count);
            return temp;
        }

        /// <summary>
        ///     Reads next signed 8-bit integer from stream.
        /// </summary>
        /// <returns>8-bit signed integer.</returns>
        public SByte ReadSByte()
        {
            return ((byte) ReadByte()).Sign();
        }

        /// <summary>
        ///     Reads next unsigned 16-bit integer from stream.
        /// </summary>
        /// <returns>16-bit unsigned integer.</returns>
        public UInt16 ReadUInt16(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(2).ToUInt16(0, seq);
        }

        /// <summary>
        ///     Reads next signed 16-bit integer from stream.
        /// </summary>
        /// <returns>16-bit signed integer.</returns>
        public Int16 ReadInt16(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(2).ToInt16(0, seq);
        }

        /// <summary>
        ///     Reads next unsigned 32-bit integer from stream.
        /// </summary>
        /// <returns>32-bit unsigned integer.</returns>
        public UInt32 ReadUInt32(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(4).ToUInt32(0, seq);
        }

        /// <summary>
        ///     Reads next signed 32-bit integer from stream.
        /// </summary>
        /// <returns>32-bit signed integer.</returns>
        public Int32 ReadInt32(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(4).ToInt32(0, seq);
        }

        /// <summary>
        ///     Reads next unsigned 64-bit integer from stream.
        /// </summary>
        /// <returns>64-bit unsigned integer.</returns>
        public UInt64 ReadUInt64(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(8).ToUInt64(0, seq);
        }

        /// <summary>
        ///     Reads next signed 64-bit integer from stream.
        /// </summary>
        /// <returns>64-bit signed integer.</returns>
        public Int64 ReadInt64(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(8).ToInt64(0, seq);
        }

        /// <summary>
        /// Reads next variable-width integer from the stream.
        /// Supports VInt up to 64 bits long.
        /// </summary>
        /// <returns>Variable-width signed integer.</returns>
        public VInt ReadVInt()
        {
            Byte[] b = Peek(1);
            if (b.Length == 0) 
                throw new EndOfStreamException();

            Int32 width = VInt.GetWidth(b);
            Byte[] value = ReadBytes(width);
            return new VInt(value);
        }

        /// <summary>
        /// Reads next UTF-8 encoded string, whose length is specified by a VInt
        /// before it.
        /// </summary>
        /// <returns></returns>
        public String ReadString()
        {
            VInt length = ReadVInt();
            Byte[] temp = ReadBytes(length);
            return new string(Encoding.UTF8.GetChars(temp));
        }

        /// <summary>
        ///     Writes array of bytes to the StreamEx.
        /// </summary>
        /// <param name="b">Byte array to write.</param>
        public void WriteBytes(Byte[] b)
        {
            Write(b, 0, b.Length);
        }

        /// <summary>
        ///     Writes a signed 8-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">8-bit signed integer.</param>
        public void WriteSByte(SByte b)
        {
            WriteByte(b.Unsign());
        }

        /// <summary>
        ///     Writes a unsigned 16-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">16-bit unsigned integer.</param>
        /// <param name="seq">Byte sequence a.k.a endianness.</param>
        public void WriteUInt16(UInt16 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 2);
        }

        /// <summary>
        ///     Writes a signed 16-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">16-bit signed integer.</param>
        /// <param name="seq">Byte sequence a.k.a endianness.</param>
        public void WriteInt16(Int16 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 2);
        }

        /// <summary>
        ///     Writes a unsigned 32-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">32-bit unsigned integer.</param>
        /// <param name="seq">Byte sequence a.k.a endianness.</param>
        public void WriteUInt32(UInt32 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 4);
        }

        /// <summary>
        ///     Writes a signed 32-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">32-bit signed integer.</param>
        /// <param name="seq">Byte sequence a.k.a endianness.</param>
        public void WriteInt32(Int32 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 4);
        }

        /// <summary>
        ///     Writes a unsigned 64-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">64-bit unsigned integer.</param>
        /// <param name="seq">Byte sequence a.k.a endianness.</param>
        public void WriteUInt64(UInt64 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 8);
        }

        /// <summary>
        ///     Writes a signed 64-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">64-bit signed integer.</param>
        /// <param name="seq">Byte sequence a.k.a endianness.</param>
        public void WriteInt64(Int64 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 8);
        }

        /// <summary>
        /// Writes a signed variable-bit integer to the StreamEx.
        /// </summary>
        /// <param name="b">Variable-bit integer.</param>
        /// <param name="width">Desired width of the VInt representation; if negative, default width will be used.</param>
        /// <exception cref="ArgumentException">Throws ArgumentException if the specified width is positive and less than necessary width to contain the VInt value.</exception>
        public void WriteVInt(VInt b, Int32 width = -1)
        {
            WriteBytes(b.Encode(width));
        }

        /// <summary>
        /// Writes a UTF-8 encoded string to the stream.
        /// Also writes a VInt with the string length.
        /// </summary>
        /// <param name="str">String to write.</param>
        public void WriteString(String str)
        {
            Byte[] temp = Encoding.UTF8.GetBytes(str);
            WriteVInt(new VInt(temp.Length));
            WriteBytes(temp);
        }

        /// <summary>
        /// Reads an array of bytes from the stream without advancing stream position.
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>Array of bytes read from the stream; array may be shorter than <paramref name="count"/> if stream reaches the end.</returns>
        public Byte[] Peek(Int32 count)
        {
            if (!CanSeek) throw new NotSupportedException();

            Byte[] temp = new byte[count];
            Int32 readCount = Read(temp, 0, count);
            Position -= readCount;

            return readCount == count ? temp : temp.SubArray(0, readCount);
        }

        #endregion

        #region Virtual Methods

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
            return stream.Read(buffer, offset, count);
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
            stream.Write(buffer, offset, count);
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
            return stream.Seek(offset, origin);
        }

        /// <summary>
        ///     Sets the length of the current stream.
        /// </summary>
        /// <param name="value">
        ///     The desired length of the current stream in bytes.
        /// </param>
        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        /// <summary>
        ///     Clears all buffers for this stream and causes any buffered
        ///     data to be written to the underlying device.
        /// </summary>
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
            stream.Close();
            closed = true;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposed) return;
            disposed = true;
            if (disposing)
            {
                // Dispose Managed Resources Here
                if (stream != null) stream.Dispose();
                stream = null;
            }

            // Dispose Unmanaged Resources Here
        }

        #endregion

        #region Multithreading Support

        internal Object Mutex { get; set; }

        #endregion
    }
}