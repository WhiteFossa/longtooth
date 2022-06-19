using Android.Content;
using longtooth.Mobile.Abstractions.FilesPicke;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace longtooth.Mobile.Abstractions.FilesPicker
{
    /// <summary>
    /// Interface for picking files and directories
    /// </summary>
    public interface IFilesPicker
    {
        /// <summary>
        /// Setup dialog for GetFileOrDirectoryAsync() call
        /// </summary>
        void Setup(FileSelectionMode mode);

        /// <summary>
        /// Get file or directory
        /// </summary>
        Task<string> GetFileOrDirectoryAsync(string dir);
    }
}
