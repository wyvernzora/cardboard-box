// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/NumericOps.cs
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
    public static class NumericOps
    {
        /// <summary>
        ///     Picks the larger one of two IComparable objects.
        /// </summary>
        /// <typeparam name="T">Type of IComparable objects</typeparam>
        /// <param name="a">First object</param>
        /// <param name="b">Second object</param>
        /// <returns>Larger of two objects</returns>
        public static T Max<T>(T a, T b) where T : IComparable<T>
        {
            if (a != null && a.CompareTo(b) > 0) return a;
            return b;
        }

        /// <summary>
        ///     Picks the smaller one of two IComparable objects.
        /// </summary>
        /// <typeparam name="T">Type of IComparable objects</typeparam>
        /// <param name="a">First object</param>
        /// <param name="b">Second object</param>
        /// <returns>Smaller of two objects</returns>
        public static T Min<T>(T a, T b) where T : IComparable<T>
        {
            if (a != null && a.CompareTo(b) < 0) return a;
            return b;
        }

        /// <summary>
        ///     Picks the largest one of multiple IComparable objects.
        /// </summary>
        /// <typeparam name="T">Type of IComparable objects</typeparam>
        /// <param name="a">First object</param>
        /// <param name="b">All the other objects</param>
        /// <returns>Largest of multiple objects</returns>
        public static T Max<T>(T a, params T[] b) where T : IComparable<T>
        {
            return b.Aggregate(a, (current, i) => Max(current, i));
        }

        /// <summary>
        ///     Picks the smallest one of multiple IComparable objects.
        /// </summary>
        /// <typeparam name="T">Type of IComparable objects</typeparam>
        /// <param name="a">First object</param>
        /// <param name="b">All the other objects</param>
        /// <returns>Smallest of multiple objects</returns>
        public static T Min<T>(T a, params T[] b) where T : IComparable<T>
        {
            return b.Aggregate(a, (current, i) => Min(current, i));
        }

        /// <summary>
        ///     Exchanges two objects
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam>
        /// <param name="a">First object</param>
        /// <param name="b">Second object</param>
        public static void Exchange<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}