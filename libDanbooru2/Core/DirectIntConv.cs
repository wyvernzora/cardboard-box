// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/DirectIntConv.cs
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
using System.Runtime.InteropServices;

namespace libWyvernzora.Core
{
    /// <summary>
    ///     Represents "Endianness" of a number value
    /// </summary>
    public enum BitSequence
    {
        LittleEndian = 0,
        BigEndian = 1
    }

    /// <summary>
    ///     Direct Integer Conversions
    /// </summary>
    public static class DirectIntConv
    {
        /// <summary>
        ///     Converts the integer into its hexadecimal string representations.
        /// </summary>
        /// <param name="src">The integer to convert</param>
        /// <param name="padding">Expected number of characters in the resulting string (excluding 0x prefix)</param>
        /// <returns>Hexadecimal string representation of the integer</returns>
        public static String ToHexString(this Int64 src, Int32 padding)
        {
            string r = Convert.ToString(src, 16).ToUpper();
            r = r.PadLeft(padding, '0');
            return r;
        }

        #region Internal Structures

        //Decimal -> Hexadecimal

        [StructLayout(LayoutKind.Explicit)]
        private struct MidDouble
        {
            [FieldOffset(0)] public UInt64 intValue;
            [FieldOffset(0)] public Double doubleValue;

            public static MidDouble Init()
            {
                MidDouble m;
                m.intValue = 0;
                m.doubleValue = 0;
                return m;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MidSingle
        {
            [FieldOffset(0)] public UInt32 intValue;
            [FieldOffset(0)] public Single singleValue;

            public static MidSingle Init()
            {
                MidSingle m;
                m.intValue = 0;
                m.singleValue = 0;
                return m;
            }
        }

        #endregion

        #region Direct Sign/Unsign

        /// <summary>
        ///     Overloaded.
        ///     Force-casts an unsigned integer to its signed version.
        /// </summary>
        /// <param name="i">Unsigned integer</param>
        /// <returns>Signed integer with identical binary representation</returns>
        public static SByte Sign(this Byte i)
        {
            return (SByte) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts an unsigned integer to its signed version.
        /// </summary>
        /// <param name="i">Unsigned integer</param>
        /// <returns>Signed integer with identical binary representation</returns>
        public static Int16 Sign(this UInt16 i)
        {
            return (Int16) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts an unsigned integer to its signed version.
        /// </summary>
        /// <param name="i">Unsigned integer</param>
        /// <returns>Signed integer with identical binary representation</returns>
        public static Int32 Sign(this UInt32 i)
        {
            return (Int32) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts an unsigned integer to its signed version.
        /// </summary>
        /// <param name="i">Unsigned integer</param>
        /// <returns>Signed integer with identical binary representation</returns>
        public static Int64 Sign(this UInt64 i)
        {
            return (Int64) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts a signed integer to its unsigned version.
        /// </summary>
        /// <param name="i">Signed integer</param>
        /// <returns>Unsigned integer with identical binary representation</returns>
        public static Byte Unsign(this SByte i)
        {
            return (Byte) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts a signed integer to its unsigned version.
        /// </summary>
        /// <param name="i">Signed integer</param>
        /// <returns>Unsigned integer with identical binary representation</returns>
        public static UInt16 Unsign(this Int16 i)
        {
            return (UInt16) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts a signed integer to its unsigned version.
        /// </summary>
        /// <param name="i">Signed integer</param>
        /// <returns>Unsigned integer with identical binary representation</returns>
        public static UInt32 Unsign(this Int32 i)
        {
            return (UInt32) i;
        }

        /// <summary>
        ///     Overloaded.
        ///     Force-casts a signed integer to its unsigned version.
        /// </summary>
        /// <param name="i">Signed integer</param>
        /// <returns>Unsigned integer with identical binary representation</returns>
        public static UInt64 Unsign(this Int64 i)
        {
            return (UInt64) i;
        }

        #endregion

        #region Direct Convert

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this UInt16 src, BitSequence seq = BitSequence.LittleEndian)
        {
            Byte[] result = new Byte[2];
            result[1] = Convert.ToByte(src & 0xFF);
            result[0] = Convert.ToByte((src >> 8) & 0xFF);

            if (seq == BitSequence.LittleEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this Int16 src, BitSequence seq = BitSequence.LittleEndian)
        {
            return Unsign(src).ToBinary(seq);
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this UInt32 src, BitSequence seq = BitSequence.LittleEndian)
        {
            Byte[] result = new Byte[4];
            for (int i = 0; i < 4; i++)
            {
                result[3 - i] = Convert.ToByte(src & 0xFF);
                src >>= 8;
            }

            if (seq == BitSequence.LittleEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this Int32 src, BitSequence seq = BitSequence.LittleEndian)
        {
            return Unsign(src).ToBinary(seq);
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this UInt64 src, BitSequence seq = BitSequence.LittleEndian)
        {
            Byte[] result = new Byte[8];
            for (int i = 0; i < 8; i++)
            {
                result[7 - i] = Convert.ToByte(src & 0xFF);
                src >>= 8;
            }

            if (seq == BitSequence.LittleEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this Int64 src, BitSequence seq = BitSequence.LittleEndian)
        {
            return Unsign(src).ToBinary(seq);
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this Single src, BitSequence seq = BitSequence.LittleEndian)
        {
            MidSingle m = MidSingle.Init();
            m.singleValue = src;
            return m.intValue.ToBinary(seq);
        }

        /// <summary>
        ///     Overloaded.
        ///     Breaks an integer into an array of bytes.
        /// </summary>
        /// <param name="src">Integer to break</param>
        /// <param name="seq">Byte order of the resulting array</param>
        /// <returns>An array of bytes representing the integer</returns>
        public static Byte[] ToBinary(this Double src, BitSequence seq = BitSequence.LittleEndian)
        {
            MidDouble m = MidDouble.Init();
            m.doubleValue = src;
            return m.intValue.ToBinary(seq);
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static UInt16 ToUInt16(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 2)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToUInt16(): Offset out of source array bounds");

            UInt16 result;
            if (seq == BitSequence.LittleEndian)
            {
                result = src[0];
                result |= Convert.ToUInt16(Convert.ToUInt16(src[1]) << 8);
            }
            else
            {
                result = src[1];
                result |= Convert.ToUInt16(Convert.ToUInt16(src[0]) << 8);
            }

            return result;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static Int16 ToInt16(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 2)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToInt16(): Offset out of source array bounds");

            return Sign(src.ToUInt16(offset, seq));
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static UInt32 ToUInt32(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 4)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToUInt32(): Offset out of source array bounds");

            UInt32 result;
            if (seq == BitSequence.LittleEndian)
            {
                result = src[0];
                for (int i = 1; i < 4; i++)
                {
                    result |= Convert.ToUInt32(Convert.ToUInt32(src[i]) << (i * 8));
                }
            }
            else
            {
                result = src[3];
                for (int i = 1; i < 4; i++)
                {
                    result |= Convert.ToUInt32(Convert.ToUInt32(src[3 - i]) << (i * 8));
                }
            }
            return result;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static Int32 ToInt32(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 4)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToInt32(): Offset out of source array bounds");

            return Sign(src.ToUInt32(offset, seq));
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static UInt64 ToUInt64(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 8)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToUInt64(): Offset out of source array bounds");

            UInt64 result;
            if (seq == BitSequence.LittleEndian)
            {
                result = src[0];
                for (int i = 1; i < 8; i++)
                {
                    result |= Convert.ToUInt64(Convert.ToUInt64(src[i]) << (i * 8));
                }
            }
            else
            {
                result = src[7];
                for (int i = 1; i < 8; i++)
                {
                    result |= Convert.ToUInt64(Convert.ToUInt64(src[7 - i]) << (i * 8));
                }
            }

            return result;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static Int64 ToInt64(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 8)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToInt64(): Offset out of source array bounds");

            return Sign(src.ToUInt64(offset, seq));
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static Single ToSingle(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 4)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToSingle(): Offset out of source array bounds");

            MidSingle m = MidSingle.Init();
            m.intValue = src.ToUInt32(offset, seq);
            return m.singleValue;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles a byte array into an integer.
        /// </summary>
        /// <param name="src">Byte array representing the integer</param>
        /// <param name="offset">Byte offset of the integer</param>
        /// <param name="seq">Byte order of the array</param>
        /// <returns>Integer assembled from the byte array</returns>
        public static Double ToDouble(this Byte[] src, Int32 offset, BitSequence seq = BitSequence.LittleEndian)
        {
            if (offset < 0 || offset > src.Length - 4)
                throw new ArgumentOutOfRangeException("offset", "Byte[].ToDouble(): Offset out of source array bounds");

            MidDouble m = MidDouble.Init();
            m.intValue = src.ToUInt64(offset, seq);
            return m.doubleValue;
        }

        #endregion

        #region Padding

        // Unsigned
        /// <summary>
        ///     Pads a non standard unsigned integer to 8 bit.
        /// </summary>
        /// <param name="dat">Byte containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static Byte PadU8(Byte dat, Int32 width)
        {
            return dat.Bits(width - 1, 0);
        }

        /// <summary>
        ///     Pads a non standard unsigned integer to 16 bit.
        /// </summary>
        /// <param name="dat">UInt16 containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static UInt16 PadU16(UInt16 dat, Int32 width)
        {
            return dat.Bits(width - 1, 0);
        }

        /// <summary>
        ///     Pads a non standard unsigned integer to 32 bit.
        /// </summary>
        /// <param name="dat">UInt32 containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static UInt32 PadU32(UInt32 dat, Int32 width)
        {
            return dat.Bits(width - 1, 0);
        }

        /// <summary>
        ///     Pads a non standard unsigned integer to 64 bit.
        /// </summary>
        /// <param name="dat">UInt64 containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static UInt64 PadU64(UInt64 dat, Int32 width)
        {
            return dat.Bits(width - 1, 0);
        }

        // Signed
        /// <summary>
        ///     Pads a non standard signed integer to 8 bit.
        /// </summary>
        /// <param name="dat">Byte containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static SByte PadS8(Byte dat, Int32 width)
        {
            // Assuming that highest bit is sign bit
            Boolean sign = (dat & (1 << width - 1)) != 0;
            Byte value = dat.Bits(width - 2, 0);

            // if the value is negative...
            if (sign) return (SByte) (value - (Byte) Math.Pow(2, width - 1)); // Get 2's complement of the value

            // if it is positive, return value
            return (SByte) value;
        }

        /// <summary>
        ///     Pads a non standard signed integer to 16 bit.
        /// </summary>
        /// <param name="dat">UInt16 containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static Int16 PadS16(UInt16 dat, Int32 width)
        {
            // Assuming that highest bit is sign bit
            Boolean sign = (dat & (1 << width - 1)) != 0;
            UInt16 value = dat.Bits(width - 2, 0);

            // if the value is negative...
            if (sign) return (short) (value - Math.Pow(2, width - 1)); // Get 2's complement of the value

            // if it is positive, return value
            return (Int16) value;
        }

        /// <summary>
        ///     Pads a non standard signed integer to 32 bit.
        /// </summary>
        /// <param name="dat">UInt32 containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static Int32 PadS32(UInt32 dat, Int32 width)
        {
            // Assuming that highest bit is sign bit
            Boolean sign = (dat & (1 << width - 1)) != 0;
            UInt32 value = dat.Bits(width - 2, 0);

            // if the value is negative...
            if (sign) return (Int32) (value - Math.Pow(2, width - 1)); // Get 2's complement of the value

            // if it is positive, return value
            return (Int32) value;
        }

        /// <summary>
        ///     Pads a non standard signed integer to 64 bit.
        /// </summary>
        /// <param name="dat">UInt64 containing non standard integer</param>
        /// <param name="width">Width of the non standard integer</param>
        /// <returns></returns>
        public static Int64 PadS64(UInt64 dat, Int32 width)
        {
            // Assuming that highest bit is sign bit
            Boolean sign = (dat & (1UL << width - 1)) != 0;
            UInt64 value = dat.Bits(width - 2, 0);

            // if the value is negative...
            if (sign)
                return (long) (value - ((ulong) Math.Pow(2, width - 1))); // Get 2's complement of the value

            // if it is positive, return value
            return (Int64) value;
        }

        #endregion

        #region Shrinking

        /// <summary>
        ///     Shrinks an unsigned integer to the specified width.
        /// </summary>
        /// <param name="dat">Integer to shrink</param>
        /// <param name="width">Bit width of the new representation</param>
        /// <exception cref="ArgumentException">Throws ArgumentException when value cannot fit in the specified width</exception>
        /// <returns>UInt64 containing shrinked representation of the integer</returns>
        public static UInt64 ShrinkUnsigned(UInt64 dat, Int32 width)
        {
            // Check if the value fits
            if (CompactWidthUnsigned(dat) > width)
                throw new ArgumentException(
                    String.Format(
                        "IntUtils.ShrinkUnsigned(Uint64, Int32) : Compact representation of the value is {0} bit wide, but only {1} bits are available.",
                        CompactWidthUnsigned(dat), width));

            // Shrink the value (do nothing since it's unsigned)
            return dat;
        }

        /// <summary>
        ///     Shrinks a signed integer to the specified width.
        /// </summary>
        /// <param name="dat">Integer to shrink</param>
        /// <param name="width">Bit width of the new representation</param>
        /// <exception cref="ArgumentException">Throws ArgumentException when value cannot fit in the specified width</exception>
        /// <returns>UInt64 containing shrinked representation of the integer</returns>
        public static UInt64 ShrinkSigned(Int64 dat, Int32 width)
        {
            // Check if the value fits
            if (CompactWidthSigned(dat) > width)
                throw new ArgumentException(
                    String.Format(
                        "IntUtils.ShrinkSigned(Uint64, Int32) : Compact representation of the value is {0} bit wide, but only {1} bits are available.",
                        CompactWidthSigned(dat), width));

            // Shrink the value
            if (dat >= 0) return (UInt64) dat; // if the value is positive no firther action needed
            UInt64 result = (UInt64) (Math.Pow(2, width - 1) + dat); // 2's complement of the value
            result |= 1UL << width - 1; // Put sign bit in place

            return result;
        }

        /// <summary>
        ///     Calculates the minimum number of bits necessary to represent a signed integer.
        /// </summary>
        /// <param name="dat">Integer</param>
        /// <returns>Minimum number of bits necessary to represent the integer</returns>
        public static Int32 CompactWidthSigned(Int64 dat)
        {
            return (Int32) Math.Ceiling(Math.Log(Math.Abs(dat) + 1, 2)) + 1;
        }

        /// <summary>
        ///     Calculates the minimum number of bits necessary to represent an unsigned integer.
        /// </summary>
        /// <param name="dat">Integer</param>
        /// <returns>Minimum number of bits necessary to represent the integer</returns>
        public static Int32 CompactWidthUnsigned(UInt64 dat)
        {
            return (Int32) Math.Ceiling(Math.Log(dat + 1, 2));
        }

        #endregion
    }
}