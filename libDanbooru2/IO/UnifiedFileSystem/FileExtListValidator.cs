// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/FileExtDictionaryValidator.cs
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
using System.Collections.Generic;
using System.Linq;

namespace libWyvernzora.IO
{
    /// <summary>
    ///     Validates whether strings can be file extension based on a list of extensions.
    ///     Validates composite extensions, not current ones!
    /// </summary>
    public class FileExtListValidator : IFileExtValidator
    {
        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="extensions">Collection of allowed extensions with preceeding dots.</param>
        public FileExtListValidator(IEnumerable<String> extensions)
        {
            // Copy the collection
            Extensions = extensions.ToArray();

            // Case insensitive!
            for (int i = 0; i < Extensions.Length; i++)
                Extensions[i] = Extensions[i].ToLower();
        }

        /// <summary>
        ///     Gets the array of allowed extensions with preceeding dots.
        /// </summary>
        public string[] Extensions { get; private set; }

        public bool IsValid(string currentExt, string compositeExt)
        {
            return Extensions.Contains(compositeExt, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}