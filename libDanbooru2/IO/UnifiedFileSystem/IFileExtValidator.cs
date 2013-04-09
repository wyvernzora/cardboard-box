using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libWyvernzora.IO
{
    /// <summary>
    /// Exposes methods to determine whether a string can be treated as a file extension
    /// </summary>
    public interface IFileExtValidator
    {
        /// <summary>
        /// Determines whether a file extension is valid.
        /// </summary>
        /// <remarks>
        /// Example: if filename is test.abc.def, and extension .abc is being validated
        /// currentExt is current extension with the preceding dot, which is, .abc
        /// compositeExt is string consisting of all extensions after current one, in this case it is .abc.def
        /// </remarks>
        /// <param name="currentExt"></param>
        /// <param name="compositeExt"></param>
        /// <returns></returns>
        Boolean IsValid(String currentExt, String compositeExt);
    }
}
