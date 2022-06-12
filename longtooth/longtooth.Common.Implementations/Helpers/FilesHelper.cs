using System;
using System.IO;

namespace longtooth.Common.Implementations.Helpers
{
    /// <summary>
    /// Helper to work with files
    /// </summary>
    public static class FilesHelper
    {
        /// <summary>
        /// Normalize filesystem path
        /// </summary>
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath);
        }
    }
}
