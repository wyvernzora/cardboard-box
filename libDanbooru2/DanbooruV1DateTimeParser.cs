// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/DanbooruV1DateTimeParser.cs
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
using System.Text.RegularExpressions;

namespace libDanbooru2
{
    /// <summary>
    ///     DateTime parser for Danbooru API version 1.
    /// </summary>
    public class DanbooruV1DateTimeParser : IDateTimeParser
    {
        private const String REGEX =
            "(?<yr>[0-9]{4})-(?<mo>[0-9]{2})-(?<dy>[0-9]{2}) (?<hr>[0-9]{2}):(?<min>[0-9]{2})(|:(?<sec>[0-9]{2}))";

        public DateTime Parse(string str)
        {
            Match m = Regex.Match(str, REGEX);
            if (!m.Success) throw new Exception();

            Int32 yr = Int32.Parse(m.Result("${yr}"));
            Int32 mo = Int32.Parse(m.Result("${mo}"));
            Int32 dy = Int32.Parse(m.Result("${dy}"));
            Int32 hr = Int32.Parse(m.Result("${hr}"));
            Int32 min = Int32.Parse(m.Result("${min}"));

            String strsec = m.Result("${sec}");
            Int32 sec = String.IsNullOrEmpty(strsec) ? 0 : Int32.Parse(m.Result("${sec}"));

            return new DateTime(yr, mo, dy, hr, min, sec);
        }

        public string ToString(DateTime dt)
        {
            throw new NotSupportedException();
        }
    }
}