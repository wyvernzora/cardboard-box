using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Practices.Prism.Interactivity;

namespace CardboardBox.UI
{
    public class ScrollViewMonitor
    {
        private readonly ScrollViewer target;

        public ScrollViewMonitor(FrameworkElement element)
        {
            target = FindChildOfType<ScrollViewer>(element);
            if (target == null) 
                throw new NotSupportedException("FrameworkElement not supported: ScrollViewer not found!");

            var listener = new DependencyPropertyListener();
            listener.Changed += (@s, e) => OnScroll(target.HorizontalOffset, target.VerticalOffset, target.ViewportWidth, target.ScrollableHeight);
            Binding binding = new Binding("VerticalOffset") {Source = target};
            listener.Attach(target, binding);
        }

        #region Event

        public class ScrollEventArgs : EventArgs
        {
            public Double OffsetX { get; set; }
            public Double OffsetY { get; set; }
            public Double MaxX { get; set; }
            public Double MaxY { get; set; }
        }

        private EventHandler<ScrollEventArgs> scroll;
        public event EventHandler<ScrollEventArgs> Scroll
        { add { scroll += value; } remove { scroll -= value; } }

        private void OnScroll(Double x, Double y, Double mx, Double my)
        {
            if (scroll != null)
                scroll(target, new ScrollEventArgs {OffsetX = x, OffsetY = y, MaxX = mx, MaxY = my});
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Performs a BFS on the visual tree in search for an element of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the child to look for.</typeparam>
        /// <param name="visual">Root visual of the search.</param>
        /// <returns>DependencyObject if found; null otherwise.</returns>
        public static T FindChildOfType<T>(DependencyObject visual) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(visual);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; i >= 0; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typed = child as T;
                    if (typed != null) return typed;
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        #endregion

    }
}
