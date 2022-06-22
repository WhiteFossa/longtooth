using System.Collections.Generic;
using System.Threading.Tasks;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.ClientService;

namespace longtooth.Common.Abstractions.Interfaces.ClientService
{
    /// <summary>
    /// Service, acting as a proxy between VFS (FUSE) and client
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Called by VFS, returning the contents for given directory
        /// </summary>
        Task<FilesystemItemDto> GetDirectoryContentAsync(string path);

        /// <summary>
        /// Get file metadata by path
        /// </summary>
        Task<FileMetadata> GetFileMetadata(string path);

        /// <summary>
        /// Tries to read up to maxLength bytes from file, starting at offset
        /// </summary>
        Task<FileContent> GetFileContent(string path, long offset, long maxLength);
    }
}
