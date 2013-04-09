// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/PathEx.cs
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace libWyvernzora.IO
{
    /// <summary>
    ///     Extended System.IO.Path
    ///     Utilizes multiple extension support.
    /// </summary>
    public static class PathEx
    {
        /// <summary>
        ///     Gets the file name from a path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>File name component of the path, including file extension.</returns>
        public static String GetFileName(String path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;

            String tmp = path.Replace("/", "\\");
            Int32 i = 0;
            Int32 j = tmp.IndexOf('\\', i);

            while (j != -1)
            {
                i = j + 1;
                j = tmp.IndexOf('\\', i);
            }

            return path.Substring(i);
        }

        /// <summary>
        ///     Overloaded.
        ///     Gets the main file name from a path.
        ///     For multi-extension support, please use GetMainFileName(String, Int32, IFileExtValidator) overload.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>File name component of the path, not including the extension.</returns>
        public static String GetMainFileName(String path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;

            String tmp = path.Replace("/", "\\");
            Int32 i = 0;
            Int32 j = tmp.IndexOf('\\', i);

            while (j != -1)
            {
                i = j + 1;
                j = tmp.IndexOf('\\', i);
            }

            Int32 k = tmp.Length - 1;
            Int32 l = tmp.LastIndexOf('.', k);
            if (l != -1) k = l - 1;

            return path.Substring(i, k - i + 1);
        }

        /// <summary>
        ///     Overloaded.
        ///     Gets the main file name from a path.
        ///     If you only expect one single extension, please call GetMainFileName(String) overload.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="maxExtensions">Maximum number of extensions to consider</param>
        /// <param name="validator">IFileExtValidator that determines what extensions are valid</param>
        /// <returns>File name component of the path, not including the extension.</returns>
        public static String GetMainFileName(String path, Int32 maxExtensions, IFileExtValidator validator)
        {
            return (new FileNameDescriptor(path, maxExtensions, validator)).FileName;
        }

        /// <summary>
        ///     Overloaded.
        ///     Gets the extension of a file from path.
        ///     For multi-extension support, please use GetFileExtension(String, Int32, IFileExtValidator) overload.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>File extension component of the path, including the preceeding dot.</returns>
        public static String GetFileExtension(String path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;
            if (!path.Contains(".")) return String.Empty;

            return path.Substring(path.LastIndexOf('.') + 1);
        }

        /// <summary>
        ///     Overloaded.
        ///     Gets the extension of a file from path.
        ///     If you only expect one single extension, please call GetFileExtension(String) overload.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="maxExtensions">Maximum number of extensions to consider</param>
        /// <param name="validator">IFileExtValidator that determines what extensions are valid</param>
        /// <returns>File extension component of the path, including the preceeding dot.</returns>
        public static String GetFileExtension(String path, Int32 maxExtensions, IFileExtValidator validator)
        {
            return (new FileNameDescriptor(path, maxExtensions, validator)).Extensions;
        }

        /// <summary>
        ///     Gets parent directory name for a file or directory.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Path of the parent directory.</returns>
        public static String GetParentDirectory(String path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;

            String tmp = path.Replace("/", "\\");
            Int32 i = 0;
            Int32 j = tmp.IndexOf('\\', i);

            while (j != -1)
            {
                i = j + 1;
                j = tmp.IndexOf('\\', i);
            }

            return path.Substring(0, i - 1);
        }

        /// <summary>
        ///     Gets descriptor for the specified path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="maxExtensions">Maximum number of extensions to consider</param>
        /// <param name="validator">IFileExtValidator that determines what extensions are valid</param>
        /// <returns>FileNameDescriptor object for the specified path.</returns>
        public static FileNameDescriptor GetFileNameDescriptor(String path, Int32 maxExtensions,
                                                               IFileExtValidator validator)
        {
            return new FileNameDescriptor(path, maxExtensions, validator);
        }

        /// <summary>
        ///     Gets the relative path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="basePath">Base path of the relative path.</param>
        /// <returns>File path relative to the base path.</returns>
        public static String GetRelativePath(String path, String basePath)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(basePath)) return String.Empty;

            String a = path.TrimEnd('\\', '/');
            String b = basePath.TrimEnd('\\', '/');
            String c = PopFirstDirectory(ref a);
            String d = PopFirstDirectory(ref b);

            if (c != d) return path;

            while (c == d)
            {
                if (c.Length == 0) return ".";
                c = PopFirstDirectory(ref a);
                d = PopFirstDirectory(ref b);
            }

            a = (c + '\\' + a).TrimEnd('\\', '/');
            b = (d + '\\' + b).TrimEnd('\\', '/');

            while (PopFirstDirectory(ref b).Length != 0)
                a = "..\\" + a;

            return a.Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        ///     Gets reduced path, i.e. removes redundant elements in path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Reduced file path.</returns>
        public static String GetReducedPath(String path)
        {
            Stack<String> temp = new Stack<string>();

            if (path.Length > 0)
            {
                foreach (string d in Regex.Split(path, "\\|/").Where(d => d != "."))
                {
                    if (d == "..")
                    {
                        if (temp.Count > 0)
                        {
                            String p = temp.Pop();
                            if (p == "..")
                            {
                                temp.Push(p);
                                temp.Push(d);
                            }
                        }
                        else
                            temp.Push(d);
                    }
                    else if (d.Contains(":")) temp.Clear();
                    temp.Push(d);
                }
            }

            return String.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), temp.Reverse());
        }

        /// <summary>
        ///     Gets directory path with tailing separator.
        /// </summary>
        /// <param name="path">Directory path. Please do not use file paths.</param>
        /// <returns>Directory path with tailing separator.</returns>
        public static String GetPathWithoutSeparator(String path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;
            return path.TrimEnd('\\', '/');
        }

        /// <summary>
        ///     Gets directory path without tailing separator.
        /// </summary>
        /// <param name="path">Directory path. Please do not use file paths.</param>
        /// <returns>Directory path without tailing separator.</returns>
        public static String GetPathWithSeparator(String path)
        {
            String d = GetPathWithoutSeparator(path);
            if (d == String.Empty) return String.Empty;
            return d + Path.DirectorySeparatorChar;
        }

        /// <summary>
        ///     Gets the absolute path from a relative path and a base path.
        /// </summary>
        /// <param name="path">Relative path.</param>
        /// <param name="basePath">Base path.</param>
        /// <returns>Absolute path equivalent to the relative path.</returns>
        public static String GetAbsolutePath(String path, String basePath)
        {
            basePath = GetPathWithoutSeparator(basePath).Replace('/', '\\');
            if (!String.IsNullOrEmpty(path))
                path = path.TrimStart('/', '\\').Replace('/', '\\'); ;

            Stack<String> s = new Stack<string>();

            if (basePath != String.Empty)
            {
                foreach (string d in basePath.Split('\\').Where(d => d != "."))
                {
                    if (d == "..")
                    {
                        if (s.Count > 0)
                        {
                            String p = s.Pop();
                            if (p == "..")
                            {
                                s.Push(p);
                                s.Push(d);
                            }
                        }
                        else s.Push(d);
                        continue;
                    }
                    if (d.Contains(":")) s.Clear();
                    s.Push(d);
                }
            }

            if (path != String.Empty)
            {
                foreach (string d in path.Split('\\').Where(d => d != "."))
                {
                    if (d == "..")
                    {
                        if (s.Count > 0)
                        {
                            String p = s.Pop();
                            if (p == "..")
                            {
                                s.Push(p);
                                s.Push(d);
                            }
                        }
                        else s.Push(d);
                        continue;
                    }
                    if (d.Contains(":")) s.Clear();
                    s.Push(d);
                }
            }

            return String.Join(Path.DirectorySeparatorChar.ToString(), s.Reverse());
        }

        /// <summary>
        ///     Combines a directory path and a file name into a new path.
        ///     To combine a directory path and a relative path, please call GetAbsolutePath(String, String) instead.
        /// </summary>
        /// <param name="dir">Directory path.</param>
        /// <param name="file">File name.</param>
        /// <returns>File path.</returns>
        public static String CombinePath(String dir, String file)
        {
            if (String.IsNullOrEmpty(dir)) return file;
            dir = dir.TrimEnd('\\', '/');
            return (dir + '\\' + file).Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        ///     Overloaded.
        ///     Changes extension of a file to the specified one.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="newExt">New file extension.</param>
        /// <returns>New file path with changed extension.</returns>
        public static String ChangeExtension(String path, String newExt)
        {
            return Path.ChangeExtension(path, newExt);
        }

        /// <summary>
        ///     Overloaded.
        ///     Changes extension of a file to the specified one.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="newExt">New file extension.</param>
        /// <param name="maxExtensions">Maximum number of extensions to consider</param>
        /// <param name="validator">IFileExtValidator that determines what extensions are valid</param>
        /// <returns>New file path with changed extension.</returns>
        public static String ChangeExtension(String path, String newExt, Int32 maxExtensions,
                                             IFileExtValidator validator)
        {
            FileNameDescriptor fds = new FileNameDescriptor(path, maxExtensions, validator) {Extensions = newExt};
            return fds.ToString();
        }

        /// <summary>
        ///     Removes and returns the name of the first directory in a path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>First directory name.</returns>
        public static String PopFirstDirectory(ref String path)
        {
            if (String.IsNullOrEmpty(path)) return String.Empty;

            String result;
            Int32 i = 0;
            i = path.Replace("/", "\\").IndexOf("\\", i, StringComparison.Ordinal);
            if (i < 0)
            {
                result = path;
                path = String.Empty;
            }
            else
            {
                result = path.Substring(0, i);
                path = path.Substring(i + 1);
            }
            return result;
        }

        /// <summary>
        ///     Checks whether a file path matches the specified wildcard.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="wildcard">Wildcard, where '*' and '.' represent non-separator characters.</param>
        /// <returns>True if the path matches; false otherwise.</returns>
        public static Boolean MatchesWildcard(String path, String wildcard)
        {
            // Empty/null strings don't match anything
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(wildcard)) return false;

            wildcard = "^" + Regex.Escape(wildcard).Replace("\\*", @"([^\.\:\\]*)").Replace("\\?", @"([^\.\:\\])") + "$";
            return Regex.IsMatch(path, wildcard);
        }
    }
}