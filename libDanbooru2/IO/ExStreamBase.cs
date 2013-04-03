// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora.Legacy/ExStreamBase.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of the old implementation of libWyvernzora.
// This file remains in libWyvernzora for legacy support.
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
using libDanbooru2.Core;

namespace libDanbooru2.IO
{
    public abstract class ExStreamBase : Stream
    {
        //Base Stream
        protected Stream m_base;

        //Concrete Functions
        public Byte[] ReadBytes(Int32 count)
        {
            Byte[] t_b = new Byte[count];
            Read(t_b, 0, count);
            return t_b;
        }

        public SByte ReadSByte()
        {
            return ((byte) ReadByte()).Sign();
        }

        public UInt16 ReadUShort(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(2).ToUInt16(0, seq);
        }

        public Int16 ReadShort(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(2).ToInt16(0, seq);
        }

        public UInt32 ReadUInt(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(4).ToUInt32(0, seq);
        }

        public Int32 ReadInt(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(4).ToInt32(0, seq);
        }

        public UInt64 ReadULong(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(8).ToUInt64(0, seq);
        }

        public Int64 ReadLong(BitSequence seq = BitSequence.LittleEndian)
        {
            return ReadBytes(8).ToInt64(0, seq);
        }

        public void WriteBytes(Byte[] b)
        {
            Write(b, 0, b.Length);
        }

        public void WriteSByte(SByte b)
        {
            WriteByte(b.Unsign());
        }

        public void WriteUShort(UInt16 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 2);
        }

        public void WriteShort(Int16 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 2);
        }

        public void WriteUInt(UInt32 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 4);
        }

        public void WriteInt(Int32 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 4);
        }

        public void WriteULong(UInt64 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 8);
        }

        public void WriteLong(Int64 b, BitSequence seq = BitSequence.LittleEndian)
        {
            Write(b.ToBinary(seq), 0, 8);
        }

        public void WriteTo(Stream dest, int buffer = 0x1000)
        {
            if (CanSeek) Seek(0, SeekOrigin.Begin);
            Byte[] BUFFER;
            while (true)
            {
                BUFFER = new Byte[buffer];
                Int32 READ_COUNT = Read(BUFFER, 0, buffer);
                dest.Write(BUFFER, 0, READ_COUNT);
                if (READ_COUNT < buffer) break;
            }
        }

        public override void Close()
        {
            if (m_base != null) m_base.Close();
        }

        public new void Dispose()
        {
            if (m_base != null) m_base.Close();
        }
    }
}