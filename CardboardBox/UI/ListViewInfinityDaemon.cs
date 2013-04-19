using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace CardboardBox.UI
{
    /// <summary>
    /// Helper class that includes most of the logic to
    /// make a LongListSelector infinite (dynamic loading).
    /// </summary>
    public class ListViewInfinityDaemon
    {
        private readonly ScrollViewMonitor monitor;
        private readonly ICommand load;
        private readonly Int32 threshold;

        /// <summary>
        /// Constructor.
        /// Initializes a new instance.
        /// </summary>
        /// <param name="list">LongListSelector to monitor.</param>
        /// <param name="threshold">Item loading threshold.</param>
        /// <param name="load">Command to load item.</param>
        public ListViewInfinityDaemon(LongListSelector list, Int32 threshold, ICommand load)
        {
            monitor = new ScrollViewMonitor(list);
            this.load = load;
            this.threshold = threshold;

            monitor.Scroll += (@s, e) =>
                {
                    if (e.OffsetY > e.MaxY - threshold)
                        load.Execute(null);
                };
        }
    }
}
