using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using libDanbooru2;

namespace CardboardBox.UI
{
    /// <summary>
    /// Manages List Box Infinity
    /// </summary>
    class ListBoxInfinityHook
    {
        private const Int32 LoadPageThreshold = 10; // Rows
        private const Int32 ItemSize = 120;  // Pixels
        private const Int32 PageSize = 60;  // Posts
        private const Int32 MaxPages = 3;   // Pages

        private Int32 currentPage;
        private ListBox listbox;
        private ScrollViewMonitor monitor;
        private List<Post> posts;
        private Action loadMorePosts;
        private Boolean loading;
        
        public ListBoxInfinityHook(ListBox listbox, List<Post> posts, Action loadMorePosts)
        {
            monitor = new ScrollViewMonitor(listbox);
            monitor.Scroll += OnListBoxScroll;

            this.listbox = listbox;
            this.posts = posts;
            this.loadMorePosts = loadMorePosts;
        }

        void OnListBoxScroll(object sender, ScrollViewMonitor.ScrollEventArgs e)
        {
            loading = true;
            Int32 pxThreahold = LoadPageThreshold * ItemSize;

            ThreadPool.QueueUserWorkItem(@a =>
                {
                    if (e.OffsetY <= pxThreahold)
                    {
                        // Load Previous Page
                        

                    }
                    else if (e.OffsetY >= e.MaxY - pxThreahold)
                    {
                        // Load more posts if needed
                        if (posts.Count < (currentPage + 3) * PageSize)
                            loadMorePosts();

                        // Load posts into listbox

                    }
                });
        }
    }
}
