// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/ArrayOps.cs
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
using System.Linq;

namespace libWyvernzora.Core
{
    /// <summary>
    ///     Array Operations
    /// </summary>
    public static class ArrayOps
    {
        /// <summary>
        ///     Overloaded.
        ///     Gets sub array of an existing array.
        /// </summary>
        /// <typeparam name="T">Type of array elements.</typeparam>
        /// <param name="array">Array</param>
        /// <param name="startIndex">Starting index of the sub array</param>
        /// <param name="length">Length of the sub array</param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, Int32 startIndex, Int32 length)
        {
            T[] tmp = new T[length];
            Array.Copy(array, startIndex, tmp, 0, length);
            return tmp;
        }

        /// <summary>
        ///     Overloaded.
        ///     Gets sub array of an existing array.
        /// </summary>
        /// <typeparam name="T">Type of array elements.</typeparam>
        /// <param name="array">Array</param>
        /// <param name="startIndex">Starting index of the sub array</param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, Int32 startIndex)
        {
            return array.SubArray(startIndex, array.Length - startIndex);
        }

        /// <summary>
        ///     Extends the array by padding its end and initializes extended part with default value
        /// </summary>
        /// <typeparam name="T">Type of array elements</typeparam>
        /// <param name="array">Array</param>
        /// <param name="length">New length, must be greater than current length</param>
        /// <param name="value">Default value</param>
        /// <returns>Extended array</returns>
        public static T[] PadEnd<T>(this T[] array, Int32 length, T value)
        {
            if (array.Length > length)
                throw new ArgumentOutOfRangeException("length",
                                                      "ArrayOps.Extend<T>(this T, Int32, Int32): New array length must be greater than current length.");

            T[] tmp = new T[length];
            Array.Copy(array, tmp, NumericOps.Min(array.Length, length));
            for (int i = NumericOps.Min(array.Length, length); i >= 0; i--)
                tmp[i] = value;
            return tmp;
        }

        /// <summary>
        ///     Extends the array by padding its beginning and initializes extended part with default value
        /// </summary>
        /// <typeparam name="T">Type of array elements</typeparam>
        /// <param name="array">Array</param>
        /// <param name="length">New length, must be greater than current length</param>
        /// <param name="value">Default value</param>
        /// <returns>Extended array</returns>
        public static T[] PadStart<T>(this T[] array, Int32 length, T value)
        {
            if (array.Length > length)
                throw new ArgumentOutOfRangeException("length",
                                                      "ArrayOps.ExtendBeginning<T>(this T, Int32, Int32): New array length must be greater than current length.");

            T[] tmp = new T[length];
            Array.Copy(array, 0, tmp, length - array.Length, array.Length);
            for (int i = 0; i < length - array.Length; i++)
                tmp[i] = value;
            return tmp;
        }

        /// <summary>
        ///     Performs the specified action on each element of the array
        /// </summary>
        /// <typeparam name="T">Type of the array elements</typeparam>
        /// <param name="array">Array</param>
        /// <param name="action">Action</param>
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
        }

        /// <summary>
        ///     Determines whether two arrays contain identical elements
        /// </summary>
        /// <typeparam name="T">Type of the array elements</typeparam>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <returns>True if identical; False otherwise</returns>
        public static Boolean ArrayEquals<T>(T[] a, T[] b)
        {
            if (a.Length != b.Length) return false;
            return !a.Where((t, i) => !t.Equals(b[i])).Any();
        }

        /// <summary>
        ///     Determines whether two arrays contain identical elements
        /// </summary>
        /// <typeparam name="T">Type of the array elements</typeparam>
        /// <param name="a">First array</param>
        /// <param name="b">Second array</param>
        /// <param name="bIndex">Starting index in the second array</param>
        /// <param name="bCount">Number of elements to compare</param>
        /// <returns>True if identical; False otherwise</returns>
        public static Boolean ArrayEquals<T>(T[] a, T[] b, Int32 bIndex, Int32 bCount)
        {
            if (a.Length != bCount) return false;
            for (int i = 0; i < a.Length - 1; i++)
                if (!a[i].Equals(b[i + bIndex])) return false;
            return true;
        }
    }
}