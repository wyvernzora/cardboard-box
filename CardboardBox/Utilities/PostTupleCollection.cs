using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CardboardBox.UI;
using libDanbooru2;

namespace CardboardBox.Utilities
{
    public class PostTupleCollection : ObservableCollection<PostTuple>
    {
        public void Add(Post a, Post b, Post c)
        {
            Add(new PostTuple {First = a, Second = b, Third = c});
        }

        public Post GetPostAt(Int32 position)
        {
            Int32 tuple = position / 3;
            Int32 item = position % 3;

            if (tuple < 0 || tuple >= Count) throw new IndexOutOfRangeException();
            PostTuple t = this[tuple];

            if (item == 0) return t.First;
            if (item == 1) return t.Second;
            return t.Third;
        }
    }
}
