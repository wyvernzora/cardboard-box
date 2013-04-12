using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libDanbooru2;

namespace CardboardBox.UI
{
    /// <summary>
    /// Utility class to aid in listbox virtualization
    /// </summary>
    public class PostTuple
    {
        public Post First { get; set; }

        public Post Second { get; set; }

        public Post Third { get; set; }
    }
}
