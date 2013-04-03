// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/DanbooruCredentials.cs
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using libDanbooru2.Core;

namespace libDanbooru2
{
    /// <summary>
    ///     Danbooru Credentials
    /// </summary>
    public sealed class DanbooruCredentials
    {
        private readonly String hash;
        private readonly String uname;

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="username">Plaintext username.</param>
        /// <param name="password">Plaintext unsalted password.</param>
        public DanbooruCredentials(String username, String password)
        {
            uname = username;

            SHA1 hasher = new SHA1Managed();
            hash = String.Join(String.Empty,
                               from b in
                                   hasher.ComputeHash(
                                       Encoding.UTF8.GetBytes(String.Format(Constants.SaltString, password)))
                               select DirectIntConv.ToHexString(b, 2).ToLower());
        }

        /// <summary>
        ///     Gets the username of the credentials.
        /// </summary>
        public String Username
        {
            get { return uname; }
        }

        /// <summary>
        ///     Gets the hashed salted password of the credentials.
        /// </summary>
        public String Hash
        {
            get { return hash; }
        }
    }
}