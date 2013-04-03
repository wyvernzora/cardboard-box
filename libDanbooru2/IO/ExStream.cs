// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora.Legacy/ExStream.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of the old implementation of libWyvernzora.
// This file remains in libWyvernzora for legacy support, if you seek to use
// extended stream functionality please use libWyvernzora.IO.StreamEx class.
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

namespace libDanbooru2.IO
{
    public class ExStream : ExStreamBase
    {
        public ExStream(Stream bSream)
        {
            m_base = bSream;
        }

        public ExStream(String path, FileMode fmode = FileMode.Open, FileAccess faccess = FileAccess.ReadWrite)
        {
            m_base = new FileStream(path, fmode, faccess);
        }

        public ExStream()
        {
            m_base = new MemoryStream();
        }

        public override bool CanRead
        {
            get { return m_base.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_base.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_base.CanWrite; }
        }

        public override long Length
        {
            get { return m_base.Length; }
        }

        public override long Position
        {
            get { return m_base.Position; }
            set { m_base.Position = value; }
        }

        public override void Flush()
        {
            m_base.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_base.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_base.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_base.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_base.Write(buffer, offset, count);
        }

        public override void Close()
        {
            m_base.Close();
        }

    }
}