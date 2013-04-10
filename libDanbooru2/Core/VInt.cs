// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/VInt.cs
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

namespace libWyvernzora.Core
{
    /// <summary>
    ///     Variable Width Integer
    /// </summary>
    public struct VInt : IComparable<VInt>, IEquatable<VInt>
    {
        #region Constants

        /// <summary>
        ///     Maximum defined value of a variable width integer
        /// </summary>
        public const Int64 MaxValue = 36028797018963967;

        /// <summary>
        ///     Minimum defined value of a variable width integer
        /// </summary>
        public const Int64 MinValue = -36028797018963968;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="value">Int64 value to store in the VInt.</param>
        public VInt(Int64 value)
        {
            if (value > MaxValue || value < MinValue)
                throw new ArgumentOutOfRangeException();


            this.value = value;
            width = GetOptimalWidth(value);
        }

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="data">Data containing VInt to decode.</param>
        public VInt(Byte[] data)
        {
            Int32 w = GetWidth(data);
            if (data.Length < w) throw new ArgumentException("Not enough bytes to decode!");

            UInt64 temp = data.PadStart<byte>(8, 0).ToUInt64(0, BitSequence.BigEndian);

            width = w * 8;
            value = BitOps.PadS64(temp, width - w);
        }

        #endregion

        #region Properties

        private Int64 value;
        private Int32 width;

        /// <summary>
        ///     Value of the variable width integer.
        /// </summary>
        /// <remarks>
        ///     As of March 2013,Variable Width Integer has only been defined up to 56 bits.
        /// </remarks>
        public Int64 Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        ///     Bit width of the entire variable width integer.
        ///     Includes both width descriptor and value parts.
        /// </summary>
        public Int32 Width
        {
            get { return width; }
            private set { width = value; }
        }

        #endregion

        #region Operators

        // VInt -> Decimal
        public static implicit operator Int64(VInt b)
        {
            return b.Value;
        }

        public static implicit operator UInt64(VInt b)
        {
            return Convert.ToUInt64(b.Value);
        }

        public static implicit operator Int32(VInt b)
        {
            return Convert.ToInt32(b.Value);
        }

        public static implicit operator UInt32(VInt b)
        {
            return Convert.ToUInt32(b.Value);
        }

        public static implicit operator Int16(VInt b)
        {
            return Convert.ToInt16(b.Value);
        }

        public static implicit operator UInt16(VInt b)
        {
            return Convert.ToUInt16(b.Value);
        }

        public static implicit operator SByte(VInt b)
        {
            return Convert.ToSByte(b.Value);
        }

        public static implicit operator Byte(VInt b)
        {
            return Convert.ToByte(b.Value);
        }

        public static implicit operator Decimal(VInt b)
        {
            return Convert.ToDecimal(b.Value);
        }

        // Decimal -> VInt

        // Math Operations
        public static VInt operator +(VInt lhs, VInt rhs)
        {
            Int64 value = lhs.Value + rhs.Value;
            return new VInt(value);
        }

        public static VInt operator -(VInt lhs, VInt rhs)
        {
            Int64 value = lhs.Value - rhs.Value;
            return new VInt(value);
        }

        public static VInt operator *(VInt lhs, VInt rhs)
        {
            Int64 value = lhs.Value * rhs.Value;
            return new VInt(value);
        }

        public static VInt operator /(VInt lhs, VInt rhs)
        {
            Int64 value = lhs.Value / rhs.Value;
            return new VInt(value);
        }

        #endregion

        #region Interfaces

        public int CompareTo(VInt other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(VInt other)
        {
            return Value.Equals(other.Value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Encodes VInt into a byte array of the specified width.
        ///     If the width is negative then the width specified when creating the VInt is used.
        ///     If VInt is created from a value without specifying width, the most compact width is used.
        /// </summary>
        /// <param name="w">Width, defaults to -1.</param>
        /// <returns></returns>
        public Byte[] Encode(Int32 w = -1)
        {
            if (w < 0) w = width;
            if (w >= 0 && w % 8 != 0)
                throw new ArgumentException(
                    "Width cannot be divided by 8, therefore it does not represent complete bytes.");
            Int32 optw = GetOptimalWidth(value);

            if (w < optw)
                throw new InvalidOperationException(
                    "Cannot shrink value to the specified length without precision loss!");

            Int32 bwidth = w - (Int32) Math.Ceiling(w / 8.0);
            UInt64 temp = BitOps.ShrinkSigned(value, bwidth);
            temp |= 1UL << (bwidth);

            return temp.ToBinary(BitSequence.BigEndian).SubArray(8 - w / 8);
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Gets the width of the VInt from a Byte array.
        /// </summary>
        /// <param name="peek">Byte array containing the size descriptor of VInt.</param>
        /// <returns>Width of the VInt in bits, including size descriptor.</returns>
        public static Int32 GetWidth(Byte[] peek)
        {
            Int32 bwidth;
            for (bwidth = 0; bwidth < peek.Length && peek[bwidth] == 0; bwidth++) ;

            Int32 iwidth = 0;
            while ((peek[bwidth] & (0x80 >> iwidth++)) == 0) ;

            iwidth = iwidth + bwidth * 8;
            if (iwidth == 0) throw new Exception("Could not determine the width of the VInt.");

            return iwidth;
        }

        /// <summary>
        ///     Gets the optimal width for a VInt.
        /// </summary>
        /// <param name="value">Value of the VInt</param>
        /// <returns></returns>
        public static Int32 GetOptimalWidth(Int64 value)
        {
            Int32 bwidth = BitOps.CompactWidthSigned(value);
            Int32 hwidth = (bwidth).Align(8) / 8;
            return (bwidth + hwidth).Align(8);
        }

        #endregion
    }
}