using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;

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

        public String GeneratePage(String colorCode, String postUri)
        {
            return String.Format(template, colorCode, postUri);
        }
    }
}
