using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;
using CardboardBox.API;
using libDanbooru2;

namespace CardboardBox
{
    class PostViewerTemplate
    {
        private String template;

        public PostViewerTemplate(String uri)
        {
            StreamResourceInfo animData =
                Application.GetResourceStream(new Uri(uri, UriKind.Relative));
            StreamReader sr = new StreamReader(animData.Stream);
            template = sr.ReadToEnd();
        }

        public String GeneratePage(String colorCode, Post post)
        {
            String fileUrl = post.HasLarge ?
                Session.SiteUrl + "/data/sample/sample-" + post.MD5 + "." + post.FileExtension
                : Session.SiteUrl + "/data/" + post.MD5 + "." + post.FileExtension;
            
            return String.Format(template, colorCode, fileUrl);
        }
    }
}
