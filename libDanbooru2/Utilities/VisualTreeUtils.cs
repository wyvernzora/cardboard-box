using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace libDanbooru2.Utilities
{
    public static class VisualTreeUtils
    {
        public static T[] FindChildrenOfType<T>(FrameworkElement e) where T : class
        {
            var queue = new Queue<DependencyObject>();
            var result = new List<T>();
            queue.Enqueue(e);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; i >= 0; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typed = child as T;
                    if (typed != null) result.Add(typed);
                    queue.Enqueue(child);
                }
            }

            return result.ToArray();
        }
    }
}
