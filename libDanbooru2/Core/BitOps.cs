// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/BitOps.cs
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

// ReSharper disable InconsistentNaming

namespace libWyvernzora.Core
{
    /// <summary>
    ///     Utility class with frequently used bit operations.
    /// </summary>
    public static class BitOps
    {
        #region Safe Bit Shifts

        // Unsigned
        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Byte</param>
        /// <param name="n">Number of bits to shift</param>
        public static Byte SHL(this Byte b, Int32 n)
        {
            if (n >= 8) return 0;
            if (n < 0) return SHR(b, -n);
            return (byte) (b << n);
        }

        /// <summary>
        ///     Safe right shift.
        /// </summary>
        /// <param name="b">Byte</param>
        /// <param name="n">Number of bits to shift</param>
        public static Byte SHR(this Byte b, Int32 n)
        {
            if (n >= 8) return 0;
            if (n < 0) return SHL(b, -n);
            return (byte) (b >> n);
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">UInt16</param>
        /// <param name="n">Number of bits to shift</param>
        public static UInt16 SHL(this UInt16 b, Int32 n)
        {
            if (n >= 16) return 0;
            if (n < 0) return SHR(b, -n);
            return (ushort) (b << n);
        }

        /// <summary>
        ///     Safe right shift.
        /// </summary>
        /// <param name="b">UInt16</param>
        /// <param name="n">Number of bits to shift</param>
        public static UInt16 SHR(this UInt16 b, Int32 n)
        {
            if (n >= 16) return 0;
            if (n < 0) return SHL(b, -n);
            return (ushort) (b >> n);
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">UInt32</param>
        /// <param name="n">Number of bits to shift</param>
        public static UInt32 SHL(this UInt32 b, Int32 n)
        {
            if (n >= 32) return 0;
            if (n < 0) return SHR(b, -n);
            return b << n;
        }

        /// <summary>
        ///     Safe right shift.
        /// </summary>
        /// <param name="b">UInt32</param>
        /// <param name="n">Number of bits to shift</param>
        public static UInt32 SHR(this UInt32 b, Int32 n)
        {
            if (n >= 32) return 0;
            if (n < 0) return SHL(b, -n);
            return b >> n;
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">UInt64</param>
        /// <param name="n">Number of bits to shift</param>
        public static UInt64 SHL(this UInt64 b, Int32 n)
        {
            if (n >= 64) return 0;
            if (n < 0) return SHR(b, -n);
            return b << n;
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">UInt64</param>
        /// <param name="n">Number of bits to shift</param>
        public static UInt64 SHR(this UInt64 b, Int32 n)
        {
            if (n >= 64) return 0;
            if (n < 0) return SHL(b, -n);
            return b >> n;
        }


        // Signed
        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">SByte</param>
        /// <param name="n">Number of bits to shift</param>
        public static SByte SAL(this SByte b, Int32 n)
        {
            if (n >= 8) return 0;
            if (n < 0) return SAR(b, -n);
            return (sbyte) (b << n);
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">SByte</param>
        /// <param name="n">Number of bits to shift</param>
        public static SByte SAR(this SByte b, Int32 n)
        {
            if (n >= 8)
            {
                if ((b & (1 << 7)) != 0) return -1;
                return 0;
            }

            if (n < 0) return SAL(b, -n);
            return (sbyte) (b >> n);
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Int16</param>
        /// <param name="n">Number of bits to shift</param>
        public static Int16 SAL(this Int16 b, Int32 n)
        {
            if (n >= 16) return 0;
            if (n < 0) return SAR(b, -n);
            return (short) (b << n);
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Int16</param>
        /// <param name="n">Number of bits to shift</param>
        public static Int16 SAR(this Int16 b, Int32 n)
        {
            if (n >= 16)
            {
                if ((b & (1 << 15)) != 0) return -1;
                return 0;
            }

            if (n < 0) return SAL(b, -n);
            return (short) (b >> n);
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Int32</param>
        /// <param name="n">Number of bits to shift</param>
        public static Int32 SAL(this Int32 b, Int32 n)
        {
            if (n >= 32) return 0;
            if (n < 0) return SAR(b, -n);
            return b << n;
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Int32</param>
        /// <param name="n">Number of bits to shift</param>
        public static Int32 SAR(this Int32 b, Int32 n)
        {
            if (n >= 32)
            {
                if ((b & (1 << 31)) != 0) return -1;
                return 0;
            }

            if (n < 0) return SAL(b, -n);
            return b >> n;
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Int64</param>
        /// <param name="n">Number of bits to shift</param>
        public static Int64 SAL(this Int64 b, Int32 n)
        {
            if (n >= 64) return 0;
            if (n < 0) return SAR(b, -n);
            return b << n;
        }

        /// <summary>
        ///     Safe left shift.
        /// </summary>
        /// <param name="b">Int64</param>
        /// <param name="n">Number of bits to shift</param>
        public static Int64 SAR(this Int64 b, Int32 n)
        {
            if (n >= 64)
            {
                if ((b & (1 << 63)) != 0) return -1;
                return 0;
            }

            if (n < 0) return SAL(b, -n);
            return b >> n;
        }

        #endregion

        #region Bit Extraction

        // Unsigned
        /// <summary>
        ///     Extracts specified bits from a byte.
        /// </summary>
        /// <param name="b">Byte</param>
        /// <param name="U">High Index (7-0)</param>
        /// <param name="L">Low Index (7-0)</param>
        public static Byte Bits(this Byte b, Int32 U, Int32 L)
        {
            Int32 bitNum = U - L + 1;
            Byte mask;

            if (bitNum <= 0) mask = 0;
            else if (bitNum >= 8) mask = Byte.MaxValue;
            else mask = (byte) (SHL(1, bitNum) - 1);

            return (byte) (b.SHR(L) & mask);
        }

        /// <summary>
        ///     Extracts specified bits from a 16-bit unsigned integer.
        /// </summary>
        /// <param name="b">UInt16</param>
        /// <param name="U">High Index (15-0)</param>
        /// <param name="L">Low Index (15-0)</param>
        public static UInt16 Bits(this UInt16 b, Int32 U, Int32 L)
        {
            Int32 bitNum = U - L + 1;
            UInt16 mask;

            if (bitNum <= 0) mask = 0;
            else if (bitNum >= 16) mask = UInt16.MaxValue;
            else mask = (ushort) (SHL((UInt16) 1, bitNum) - 1);

            return (ushort) (b.SHR(L) & mask);
        }

        /// <summary>
        ///     Extracts specified bits from a 32-bit unsigned integer.
        /// </summary>
        /// <param name="b">UInt32</param>
        /// <param name="U">High Index (31-0)</param>
        /// <param name="L">Low Index (31-0)</param>
        public static UInt32 Bits(this UInt32 b, Int32 U, Int32 L)
        {
            Int32 bitNum = U - L + 1;
            UInt32 mask;

            if (bitNum <= 0) mask = 0;
            else if (bitNum >= 32) mask = UInt32.MaxValue;
            else mask = SHL(1U, bitNum) - 1;

            return b.SHR(L) & mask;
        }

        /// <summary>
        ///     Extracts specified bits from a 64-bit unsigned integer.
        /// </summary>
        /// <param name="b">UInt64</param>
        /// <param name="U">High Index (63-0)</param>
        /// <param name="L">Low Index (63-0)</param>
        public static UInt64 Bits(this UInt64 b, Int32 U, Int32 L)
        {
            Int32 bitNum = U - L + 1;
            UInt64 mask;

            if (bitNum <= 0) mask = 0;
            else if (bitNum >= 64) mask = UInt64.MaxValue;
            else mask = SHL(1UL, bitNum) - 1;

            return b.SHR(L) & mask;
        }

        // Signed
        /// <summary>
        ///     Extracts specified bits from a signed byte.
        /// </summary>
        /// <param name="b">SByte</param>
        /// <param name="U">High Index (7-0)</param>
        /// <param name="L">Low Index (7-0)</param>
        public static SByte Bits(this SByte b, Int32 U, Int32 L)
        {
            return b.Unsign().Bits(U, L).Sign();
        }

        /// <summary>
        ///     Extracts specified bits from a 16-bit signed integer.
        /// </summary>
        /// <param name="b">Int16</param>
        /// <param name="U">High Index (15-0)</param>
        /// <param name="L">Low Index (15-0)</param>
        public static Int16 Bits(this Int16 b, Int32 U, Int32 L)
        {
            return b.Unsign().Bits(U, L).Sign();
        }

        /// <summary>
        ///     Extracts specified bits from a 32-bit signed integer.
        /// </summary>
        /// <param name="b">Int32</param>
        /// <param name="U">High Index (31-0)</param>
        /// <param name="L">Low Index (31-0)</param>
        public static Int32 Bits(this Int32 b, Int32 U, Int32 L)
        {
            return b.Unsign().Bits(U, L).Sign();
        }

        /// <summary>
        ///     Extracts specified bits from a 64-bit signed integer.
        /// </summary>
        /// <param name="b">Int64</param>
        /// <param name="U">High Index (63-0)</param>
        /// <param name="L">Low Index (63-0)</param>
        public static Int64 Bits(this Int64 b, Int32 U, Int32 L)
        {
            return b.Unsign().Bits(U, L).Sign();
        }

        #endregion

        #region Bit Composition

        /// <summary>
        ///     Overloaded.
        ///     Assembles an integer from specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="S">Second Byte</param>
        /// <param name="SU">Upper Index of the Second Byte (7-0)</param>
        /// <param name="SL">Lower Index of the Second Byte (7-0)</param>
        /// <param name="T">Third Byte</param>
        /// <param name="TU">Upper Index of the Third Byte (7-0)</param>
        /// <param name="TL">Lower Index of the Third Byte (7-0)</param>
        /// <param name="Q">Fourth Byte</param>
        /// <param name="QU">Upper Index of the Fourth Byte (7-0)</param>
        /// <param name="QL">Lower Index of the Fourth Byte (7-0)</param>
        /// <returns>Integer resulting from contacting all the bits from high to low.</returns>
        public static Int32 ComposeBits(Byte H, Int32 HU, Int32 HL,
                                        Byte S, Int32 SU, Int32 SL,
                                        Byte T, Int32 TU, Int32 TL,
                                        Byte Q, Int32 QU, Int32 QL)
        {
            Int32 Hp = H.Bits(HU, HL);
            Int32 Sp = S.Bits(SU, SL);
            Int32 Tp = T.Bits(TU, TL);
            Int32 Qp = Q.Bits(QU, QL);

            return Hp.SAL(SU - SL + 1 + TU - TL + 1 + QU - QL + 1) |
                   Sp.SAL(TU - TL + 1 + QU - QL + 1) |
                   Tp.SAL(QU - QL + 1) |
                   Qp;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles an integer from specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="S">Second Byte</param>
        /// <param name="SU">Upper Index of the Second Byte (7-0)</param>
        /// <param name="SL">Lower Index of the Second Byte (7-0)</param>
        /// <param name="T">Third Byte</param>
        /// <param name="TU">Upper Index of the Third Byte (7-0)</param>
        /// <param name="TL">Lower Index of the Third Byte (7-0)</param>
        /// <returns>Integer resulting from contacting all the bits from high to low.</returns>
        public static Int32 ComposeBits(Byte H, Int32 HU, Int32 HL,
                                        Byte S, Int32 SU, Int32 SL,
                                        Byte T, Int32 TU, Int32 TL)
        {
            Int32 Hp = H.Bits(HU, HL);
            Int32 Sp = S.Bits(SU, SL);
            Int32 Tp = T.Bits(TU, TL);

            return Hp.SAL(SU - SL + 1 + TU - TL + 1) | Sp.SAL(TU - TL + 1) | Tp;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles an integer from specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="S">Second Byte</param>
        /// <param name="SU">Upper Index of the Second Byte (7-0)</param>
        /// <param name="SL">Lower Index of the Second Byte (7-0)</param>
        /// <returns>Integer resulting from contacting all the bits from high to low.</returns>
        public static Int32 ComposeBits(Byte H, Int32 HU, Int32 HL,
                                        Byte S, Int32 SU, Int32 SL)
        {
            Int32 Hp = H.Bits(HU, HL);
            Int32 Sp = S.Bits(SU, SL);

            return Hp.SAL(SU - SL + 1) | Sp;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles an integer from specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <returns>Integer resulting from contacting all the bits from high to low.</returns>
        public static Int32 ComposeBits(Byte H, Int32 HU, Int32 HL)
        {
            return H.Bits(HU, HL);
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles an integer from specified bits.
        /// </summary>
        /// <param name="H">First Integer</param>
        /// <param name="HU">Upper Index of the First Integer (31-0)</param>
        /// <param name="HL">Lower Index of the First Integer (31-0)</param>
        /// <param name="S">Second Integer</param>
        /// <param name="SU">Upper Index of the Second Integer (31-0)</param>
        /// <param name="SL">Lower Index of the Second Integer (31-0)</param>
        /// <returns>Integer resulting from contacting all the bits from high to low.</returns>
        public static Int32 ComposeBits(Int32 H, Int32 HU, Int32 HL,
                                        Int32 S, Int32 SU, Int32 SL)
        {
            Int32 Hp = H.Bits(HU, HL);
            Int32 Sp = S.Bits(SU, SL);

            return Hp.SAL(SU - SL + 1) | Sp;
        }

        /// <summary>
        ///     Overloaded.
        ///     Assembles an integer from specified bits.
        /// </summary>
        /// <param name="H">First Integer</param>
        /// <param name="HU">Upper Index of the First Integer (31-0)</param>
        /// <param name="HL">Lower Index of the First Integer (31-0)</param>
        /// <returns>Integer resulting from contacting all the bits from high to low.</returns>
        public static Int32 ComposeBits(Int32 H, Int32 HU, Int32 HL)
        {
            return H.Bits(HU, HL);
        }

        #endregion

        #region Bit Decomposition

        /// <summary>
        ///     Overloaded.
        ///     Disassembles an integer to specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="S">Second Byte</param>
        /// <param name="SU">Upper Index of the Second Byte (7-0)</param>
        /// <param name="SL">Lower Index of the Second Byte (7-0)</param>
        /// <param name="T">Third Byte</param>
        /// <param name="TU">Upper Index of the Third Byte (7-0)</param>
        /// <param name="TL">Lower Index of the Third Byte (7-0)</param>
        /// <param name="Q">Fourth Byte</param>
        /// <param name="QU">Upper Index of the Fourth Byte (7-0)</param>
        /// <param name="QL">Lower Index of the Fourth Byte (7-0)</param>
        /// <param name="value">Value to be disassembled</param>
        public static void DecomposeBits(Int32 value, ref Byte H, Int32 HU, Int32 HL,
                                         ref Byte S, Int32 SU, Int32 SL,
                                         ref Byte T, Int32 TU, Int32 TL,
                                         ref Byte Q, Int32 QU, Int32 QL)
        {
            Int32 Hp = value.SAR(SU - SL + 1 + TU - TL + 1 + QU - QL + 1) & (1.SAL(HU - HL + 1) - 1);
            H = (byte) (H & ~Convert.ToByte((1.SAL(HU - HL + 1) - 1).SAL(HL)));
            H = (byte) (H | Convert.ToByte(Hp.SAL(HL)));
            Int32 Sp = value.SAR(TU - TL + 1 + QU - QL + 1) & (1.SAL(SU - SL + 1) - 1);
            S = (byte) (S & ~Convert.ToByte((1.SAL(SU - SL + 1) - 1).SAL(SL)));
            S = (byte) (S | Convert.ToByte(Sp.SAL(SL)));
            Int32 Tp = value.SAR(QU - QL + 1) & (1.SAL(TU - TL + 1) - 1);
            T = (byte) (T & ~Convert.ToByte((1.SAL(TU - TL + 1) - 1).SAL(TL)));
            T = (byte) (T | Convert.ToByte(Tp.SAL(TL)));
            Int32 Qp = value & (1.SAL(QU - QL + 1) - 1);
            Q = (byte) (Q & ~Convert.ToByte((1.SAL(QU - QL + 1) - 1).SAL(QL)));
            Q = (byte) (Q | Convert.ToByte(Qp.SAL(QL)));
        }

        /// <summary>
        ///     Overloaded.
        ///     Disassembles an integer to specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="S">Second Byte</param>
        /// <param name="SU">Upper Index of the Second Byte (7-0)</param>
        /// <param name="SL">Lower Index of the Second Byte (7-0)</param>
        /// <param name="T">Third Byte</param>
        /// <param name="TU">Upper Index of the Third Byte (7-0)</param>
        /// <param name="TL">Lower Index of the Third Byte (7-0)</param>
        /// <param name="value">Value to be disassembled</param>
        public static void DecomposeBits(Int32 value, ref Byte H, Int32 HU, Int32 HL,
                                         ref Byte S, Int32 SU, Int32 SL,
                                         ref Byte T, Int32 TU, Int32 TL)
        {
            Int32 Hp = value.SAR(SU - SL + 1 + TU - TL + 1) & (1.SAL(HU - HL + 1) - 1);
            H = (byte) (H & ~Convert.ToByte((1.SAL(HU - HL + 1) - 1).SAL(HL)));
            H = (byte) (H | Convert.ToByte(Hp.SAL(HL)));
            Int32 Sp = value.SAR(TU - TL + 1) & (1.SAL(SU - SL + 1) - 1);
            S = (byte) (S & ~Convert.ToByte((1.SAL(SU - SL + 1) - 1).SAL(SL)));
            S = (byte) (S | Convert.ToByte(Sp.SAL(SL)));
            Int32 Tp = value & (1.SAL(TU - TL + 1) - 1);
            T = (byte) (T & ~Convert.ToByte((1.SAL(TU - TL + 1) - 1).SAL(TL)));
            T = (byte) (T | Convert.ToByte(Tp.SAL(TL)));
        }

        /// <summary>
        ///     Overloaded.
        ///     Disassembles an integer to specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="S">Second Byte</param>
        /// <param name="SU">Upper Index of the Second Byte (7-0)</param>
        /// <param name="SL">Lower Index of the Second Byte (7-0)</param>
        /// <param name="value">Value to be disassembled</param>
        public static void DecomposeBits(Int32 value, ref Byte H, Int32 HU, Int32 HL,
                                         ref Byte S, Int32 SU, Int32 SL)
        {
            Int32 Hp = value.SAR(SU - SL + 1) & (1.SAL(HU - HL + 1) - 1);
            H = (byte) (H & ~Convert.ToByte((1.SAL(HU - HL + 1) - 1).SAL(HL)));
            H = (byte) (H | Convert.ToByte(Hp.SAL(HL)));
            Int32 SPart = value & (1.SAL(SU - SL + 1) - 1);
            S = (byte) (S & ~Convert.ToByte((1.SAL(SU - SL + 1) - 1).SAL(SL)));
            S = (byte) (S | Convert.ToByte(SPart.SAL(SL)));
        }

        /// <summary>
        ///     Overloaded.
        ///     Disassembles an integer to specified bits.
        /// </summary>
        /// <param name="H">First Byte</param>
        /// <param name="HU">Upper Index of the First Byte (7-0)</param>
        /// <param name="HL">Lower Index of the First Byte (7-0)</param>
        /// <param name="value">Value to be disassembled</param>
        public static void DecomposeBits(Int32 value, ref Byte H, Int32 HU, Int32 HL)
        {
            Int32 Hp = value & (1.SAL(HU - HL + 1) - 1);
            H = (byte) (H & ~Convert.ToByte((1.SAL(HU - HL + 1) - 1).SAL(HL)));
            H = (byte) (H | Convert.ToByte(Hp.SAL(HL)));
        }

        /// <summary>
        ///     Overloaded.
        ///     Disassembles an integer to specified bits.
        /// </summary>
        /// <param name="H">First Integer</param>
        /// <param name="HU">Upper Index of the First Integer (31-0)</param>
        /// <param name="HL">Lower Index of the First Integer (31-0)</param>
        /// <param name="S">Second Integer</param>
        /// <param name="SU">Upper Index of the Second Integer (31-0)</param>
        /// <param name="SL">Lower Index of the Second Integer (31-0)</param>
        /// <param name="value">Value to be disassembled</param>
        public static void DecomposeBits(Int32 value, ref Int32 H, Int32 HU, Int32 HL,
                                         ref Int32 S, Int32 SU, Int32 SL)
        {
            Int32 Hp = (value.SAR(SU - SL + 1)) & ((1.SAL(HU - HL + 1)) - 1);
            H = H & ~(1.SAL(HU - HL + 1) - 1).SAL(HL);
            H = H | Hp.SAL(HL);
            Int32 Sp = value & ((1.SAL(SU - SL + 1)) - 1);
            S = S & ~(1.SAL(SU - SL + 1) - 1).SAL(SL);
            S = S | Sp.SAL(SL);
        }

        /// <summary>
        ///     Overloaded.
        ///     Disassembles an integer to specified bits.
        /// </summary>
        /// <param name="H">First Integer</param>
        /// <param name="HU">Upper Index of the First Integer (31-0)</param>
        /// <param name="HL">Lower Index of the First Integer (31-0)</param>
        /// <param name="value">Value to be disassembled</param>
        public static void DecomposeBits(Int32 value, ref Int32 H, Int32 HU, Int32 HL)
        {
            Int32 Hp = value & ((1.SAL(HU - HL + 1)) - 1);
            H = H & ~(1.SAL(HU - HL + 1) - 1).SAL(HL);
            H = H | Hp.SAL(HL);
        }

        #endregion

        #region Bit Concateration

        #endregion

        #region Bit Split

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
            if (sign) return (SByte)(value - (Byte)Math.Pow(2, width - 1)); // Get 2's complement of the value

            // if it is positive, return value
            return (SByte)value;
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
            if (sign) return (short)(value - Math.Pow(2, width - 1)); // Get 2's complement of the value

            // if it is positive, return value
            return (Int16)value;
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
            if (sign) return (Int32)(value - Math.Pow(2, width - 1)); // Get 2's complement of the value

            // if it is positive, return value
            return (Int32)value;
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
                return (long)(value - ((ulong)Math.Pow(2, width - 1))); // Get 2's complement of the value

            // if it is positive, return value
            return (Int64)value;
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
                        "IntUtils.ShrinkSigned(Uint64, Int32) : Compact representation of {2} is {0} bit wide, but only {1} bits are available.",
                        CompactWidthSigned(dat), width, dat));

            // Shrink the value
            if (dat >= 0) return (UInt64)dat; // if the value is positive no firther action needed
            UInt64 result = ((UInt64)dat) & (UInt64.MaxValue >> (64 - width)); // Clip off redundant stuff

            return result;
        }

        /// <summary>
        ///     Calculates the minimum number of bits necessary to represent a signed integer.
        /// </summary>
        /// <param name="dat">Integer</param>
        /// <returns>Minimum number of bits necessary to represent the integer</returns>
        public static Int32 CompactWidthSigned(Int64 dat)
        {
            return (Int32)Math.Ceiling(Math.Log(Math.Abs(dat) + 1, 2)) + 1;
        }

        /// <summary>
        ///     Calculates the minimum number of bits necessary to represent an unsigned integer.
        /// </summary>
        /// <param name="dat">Integer</param>
        /// <returns>Minimum number of bits necessary to represent the integer</returns>
        public static Int32 CompactWidthUnsigned(UInt64 dat)
        {
            return (Int32)Math.Ceiling(Math.Log(dat + 1, 2));
        }

        #endregion
    }
}