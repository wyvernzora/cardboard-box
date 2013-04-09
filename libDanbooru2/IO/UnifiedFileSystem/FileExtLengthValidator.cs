// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/FileExtLengthValidator.cs
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

namespace libWyvernzora.IO
{
    /// <summary>
    ///     Validates whether strings can be file extension based on their length.
    /// </summary>
    public class FileExtLengthValidator : IFileExtValidator
    {
        private Int32 max;
        private Int32 min;

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="minLength">Minimum allowed length for a file extension, excluding preceeding dot.</param>
        /// <param name="maxLength">Maximum allowed length for a file extension, excluding preceeding dot.</param>
        public FileExtLengthValidator(Int32 minLength, Int32 maxLength)
        {
            MinimumLength = minLength;
            MaximumLength = maxLength;
        }

        /// <summary>
        ///     Minimum allowed length for a file extension, excluding preceeding dot.
        /// </summary>
        public Int32 MinimumLength
        {
            get { return min - 1; }
            set { min = value + 1; }
        }

        /// <summary>
        ///     Maximum allowed length for a file extension, excluding preceeding dot.
        /// </summary>
        public Int32 MaximumLength
        {
            get { return max - 1; }
            set { max = value + 1; }
        }

        public bool IsValid(string currentExt, string compositeExt)
        {
            return (currentExt.Length >= min && currentExt.Length <= max);
        }
    }
}