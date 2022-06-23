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
        /// Move down by directories tree
        /// </summary>
        public static string MoveDown(string directory, string subdirectory)
        {
            if (directory.Equals(string.Empty))
            {
                throw new ArgumentException(nameof(directory));
            }

            if (subdirectory.Equals(string.Empty))
            {
                throw new ArgumentException(nameof(subdirectory));
            }

            if (directory[directory.Length - 1] != '/')
            {
                directory = directory + @"/";
            }

            if (subdirectory[subdirectory.Length - 1] == '/')
            {
                subdirectory = subdirectory.Substring(0, subdirectory.Length - 1);
            }

            return directory + subdirectory;
        }

        // Move up
        public static string MoveUp(string directory)
        {
            if (directory == @"/")
            {
                throw new InvalidOperationException("Can't go higher than root!");
            }

            if (directory[directory.Length - 1] == '/')
            {
                directory = directory.Substring(0, directory.Length - 1);
            }

            var lastSlashIndex = directory.LastIndexOf('/');

            return directory.Substring(0, lastSlashIndex);
        }

        /// <summary>
        /// Normalize filesystem path
        /// </summary>
        public static string NormalizePath(string path)
        {
            // TODO: Implement me
            return path;
        }

        /// <summary>
        /// Get everything after rightmost slash
        /// </summary>
        public static string GetFileOrDirectoryName(string path)
        {
            var lastSlashIndex = path.LastIndexOf('/');
            var result = path.Substring(lastSlashIndex + 1, path.Length - lastSlashIndex - 1);
            return result;
        }

        /// <summary>
        /// Append filename to directory name to get full path
        /// </summary>
        public static string AppendFilename(string directory, string filename)
        {
            if (directory.Equals(string.Empty))
            {
                throw new ArgumentException(nameof(directory));
            }

            if (directory[directory.Length - 1] != '/')
            {
                directory = directory + @"/";
            }

            return directory + filename;
        }
    }
}
