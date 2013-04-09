// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/UserLevel.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of CardboardBox.
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

namespace libDanbooru2
{
    /// <summary>
    ///     Represents user level and permissions.
    /// </summary>
    public sealed class UserLevel
    {
        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="level">
        ///     <see cref="Level" />
        /// </param>
        /// <param name="name">
        ///     <see cref="Name" />
        /// </param>
        /// <param name="tagLimit">
        ///     <see cref="TagLimit" />
        /// </param>
        public UserLevel(Int32 level, String name, Int32 tagLimit)
        {
            Level = level;
            Name = name;
            TagLimit = tagLimit;
        }

        /// <summary>
        ///     Gets the numeric representation of the level.
        /// </summary>
        public Int32 Level { get; private set; }

        /// <summary>
        ///     Gets the human readable description of the level.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        ///     Gets the maximum number of tags this level permits.
        /// </summary>
        public Int32 TagLimit { get; private set; }
    }
}