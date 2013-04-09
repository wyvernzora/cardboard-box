// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/FileNameDescriptor.cs
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
using System.IO;

namespace libWyvernzora.IO
{
    /// <summary>
    ///     File Name Descriptor.
    ///     Splits file paths into directory, name and extensions.
    ///     Advanced support for multiple extensions.
    /// </summary>
    public class FileNameDescriptor
    {
        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="path">File path to analyze</param>
        /// <param name="maxExt">Maximum number of extensions to consider</param>
        /// <param name="validator">IFileExtValidator that determines what extensions are valid</param>
        public FileNameDescriptor(String path, Int32 maxExt = 2, IFileExtValidator validator = null)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (maxExt < 0) throw new ArgumentOutOfRangeException("maxExt");

            // Get Directory and remove it from path
            Directory = PathEx.GetParentDirectory(path);
            path = PathEx.GetFileName(path);

            if (String.IsNullOrEmpty(path)) throw new InvalidOperationException();

            // Start manual file name analysis
            List<Int32> dots = new List<Int32>(); // All occurences of dots
            for (Int32 n = path.IndexOf('.'); n != -1; n = path.IndexOf('.', n + 1))
                dots.Add(n); // Add occurence of the dot to list
            dots.Add(path.Length); // Add a 'virtual' dot to handle no extension case

            // Get minimum file name count, overrides maxExt if there is no file name (e.g. '.patch')
            Int32 minDot = 0;
            for (int i = 0; i < dots.Count; i++)
                if (dots[i] > i)
                {
                    minDot = i;
                    break;
                }

            // Get the most optimal splitter index
            Int32 splitter = dots.Count - minDot <= maxExt ? minDot : dots.Count - maxExt - 1;

            // Verify extensions
            if (validator != null)
            {
                for (int i = dots.Count - 1; i > splitter; i--) // Note that last dot is 'virtual' i.e. it is not there
                {
                    if (validator.IsValid(path.Substring(dots[i - 1], dots[i] - dots[i - 1]),
                                          path.Substring(dots[i - 1]))) continue;
                    // If current extension is invalid, adjust splitter and stop validation
                    splitter = i;
                    break;
                }
            }

            // Split
            FileName = path.Substring(0, dots[splitter]);
            Extensions = path.Substring(dots[splitter]);
        }

        /// <summary>
        ///     Gets parent directory of the file.
        /// </summary>
        public String Directory { get; set; }

        /// <summary>
        ///     Gets the name of the file without extension.
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        ///     Gets extensions of the file.
        /// </summary>
        public String Extensions { get; set; }

        public override string ToString()
        {
            return Directory + "\\" + FileName + Extensions;
        }
    }
}