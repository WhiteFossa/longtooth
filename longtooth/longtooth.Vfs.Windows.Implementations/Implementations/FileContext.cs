using System;
using System.Collections.Generic;
using System.Text;

namespace longtooth.Vfs.Windows.Implementations.Implementations
{
    /// <summary>
    /// Context, associated with opened file
    /// </summary>
    public class FileContext
    {
        /// <summary>
        /// File position
        /// </summary>
        public long Position { get; set; }
    }
}
