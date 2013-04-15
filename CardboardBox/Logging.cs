using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardboardBox
{
    /// <summary>
    /// Logging wrapper.
    /// </summary>
    public static class Logging
    {
        public static void D(String format, params Object[] args)
        {
            System.Diagnostics.Debug.WriteLine(format, args);
        }
    }
}
