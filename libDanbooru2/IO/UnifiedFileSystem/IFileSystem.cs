// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/IFileSystemService.cs
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

namespace libWyvernzora.IO.UnifiedFileSystem
{
    /// <summary>
    ///     libWyvernzora File System Service Interface
    /// </summary>
    public interface IFileSystem<T> : IDisposable where T : FileSystemObject
    {
        /// <summary>
        /// Gets a value indicating whether the IFileSystemService is readonly.
        /// </summary>
        Boolean IsReadOnly { get; }

        /// <summary>
        ///     List of all FileSystemObjects of File type.
        /// </summary>
        IEnumerable<T> Files { get; }

        /// <summary>
        ///     Root FileSystemObject of the File System.
        /// </summary>
        T Root { get; }

        /// <summary>
        ///     Tries to get the FileSystemObject with the specified path.
        /// </summary>
        /// <param name="path">Path of the FileSystemObject.</param>
        /// <returns>A FileSystemObject if found; null otherwise.</returns>
        T GetFileSystemObject(String path);

        /// <summary>
        /// Tries to open the FileSystemObject as a stream.
        /// </summary>
        /// <param name="obj">FileSystemObject to open.</param>
        /// <param name="mode">FileMode to open FileSystemObject with.</param>
        /// <returns>StreamEx if successful; null otherwise.</returns>
        StreamEx OpenFileSystemObject(T obj, FileAccess mode);
        
        /// <summary>
        ///     Closes the File System.
        /// </summary>
        void Close();
    }
}