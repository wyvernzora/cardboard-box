// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/Post.cs
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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace libDanbooru2
{
    [DataContract(Name = "post")]
    public class Post
    {
        [DataMember(Name = "id")]
        public Int32 ID { get; set; }

        [DataMember(Name = "parent_id")]
        public Int32? ParentID { get; set; }


        [DataMember(Name = "has_notes")]
        public Boolean HasNotes { get; set; }

        [DataMember(Name = "has_comments")]
        public Boolean HasComments { get; set; }

        [DataMember(Name = "has_children")]
        public Boolean HasChildren { get; set; }


        [DataMember(Name = "rating")] 
        public String RatingString { get; set; }

        [IgnoreDataMember]
        public Rating Rating
        {
            get { return RatingEx.Parse(RatingString); }
            set { RatingString = value.ToString().Substring(0, 1).ToLower(); }
        }

        [DataMember(Name = "status")]
        public String Status { get; set; }

        [DataMember(Name = "author")]
        public String Author { get; set; }

        [DataMember(Name = "score")]
        public Int32 Score { get; set; }

        [DataMember(Name = "md5")]
        public String MD5 { get; set; }

        [DataMember(Name = "source")]
        public Uri Source { get; set; }

        // File
        [DataMember(Name = "file_url")]
        public Uri FileUrl { get; set; }

        [DataMember(Name = "height")]
        public Int32 Height { get; set; }

        [DataMember(Name = "width")]
        public Int32 Width { get; set; }

        [DataMember(Name = "file_size")]
        public Int32 FileSize { get; set; }



        [DataMember(Name = "tags")]
        public String RawTags { get; set; }


        #region Additional properties (not serialized)

        [IgnoreDataMember]
        public BitmapSource ImageSource { get; set; }

        [IgnoreDataMember]
        public BitmapSource LargeImageSource { get; set; }

        #endregion
    }
}