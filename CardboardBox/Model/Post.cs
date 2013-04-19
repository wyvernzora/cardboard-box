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
using System.Runtime.Serialization;
using libDanbooru2;

namespace CardboardBox.Model
{
    [DataContract(Name = "post")]
    public class Post
    {
        [DataMember(Name = "id")]
        public Int32 ID { get; set; }

        [DataMember(Name = "parent_id")]
        public Int32? ParentID { get; set; }

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


        [DataMember(Name = "uploader_name")]
        public String Author { get; set; }

        [DataMember(Name = "uploader_id")]
        public Int32 AuthorID { get; set; }

        [DataMember(Name = "md5")]
        public String MD5 { get; set; }


        // File
        [DataMember(Name = "image_height")]
        public Int32 Height { get; set; }

        [DataMember(Name = "image_width")]
        public Int32 Width { get; set; }

        [DataMember(Name = "file_size")]
        public Int32 FileSize { get; set; }

        [DataMember(Name = "file_ext")]
        public String FileExtension { get; set; }

        [DataMember(Name = "tag_string_artist")]
        public String ArtistTagsString { get; set; }

        [DataMember(Name = "tag_string_copyright")]
        public String CopyrightTagsString { get; set; }

        [DataMember(Name = "tag_string_character")]
        public String CharacterTagsString { get; set; }

        [DataMember(Name = "tag_string_general")]
        public String GeneralTagsString { get; set; }

        [DataMember(Name = "has_large")]
        public Boolean HasLarge { get; set; }

        #region Additional properties (not serialized)

        [IgnoreDataMember]
        public Uri PreviewUrl
        {
            get { return new Uri(Constants.SiteUrl + Constants.PreviewDir + MD5 + ".jpg"); }
        }

        #endregion
    }
}