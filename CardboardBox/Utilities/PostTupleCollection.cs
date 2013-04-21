using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CardboardBox.Model;
using CardboardBox.UI;
using CCEvArg = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using CCAction = System.Collections.Specialized.NotifyCollectionChangedAction;

namespace CardboardBox.Utilities
{
    public class PostTupleCollection : ObservableCollection<PostTuple>
    {
        public void Add(Post p)
        {
            if (Count > 0)
            {
                PostTuple lastItem = this[Count - 1];
                if (lastItem.Second == null)
                {
                    lastItem.Second = p;
                    OnCollectionChanged(new CCEvArg(CCAction.Replace, lastItem, lastItem, Count - 1));
                }
                else if (lastItem.Third == null)
                {
                    lastItem.Third = p;
                    OnCollectionChanged(new CCEvArg(CCAction.Replace, lastItem, lastItem, Count - 1));
                }
                else
                    Add(p, null, null);
            }
            else
                Add(p, null, null);
        }

        public void AddRange(IList<Post> p)
        {
            // Fill up the last element
            Int32 offset = 0;
            if (Count > 0)
            {
                PostTuple lastElement = this[Count - 1];
                if (lastElement.Second == null)
                    lastElement.Second = p[offset++];
                if (lastElement.Third == null)
                    lastElement.Third = p[offset++];
                if (offset != 0) // Notify change if last item changed
                    OnCollectionChanged(new CCEvArg(CCAction.Replace, lastElement, lastElement, Count - 1));
            }

            // Start filling up tuples until no more left
            for (; offset < p.Count - 2; offset += 3)
                Add(p[offset], p[offset + 1], p[offset + 2]);
            
            // Fill up the rest
            while (offset < p.Count)
                Add(p[offset++]);
        }

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
