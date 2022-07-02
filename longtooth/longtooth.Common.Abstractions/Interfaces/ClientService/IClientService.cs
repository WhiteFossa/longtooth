using System;
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
        /// Call this method when decoded message comes from server
        /// </summary>
        Task OnNewMessageAsync(IReadOnlyCollection<byte> decodedMessage);

        /// <summary>
        /// Called by VFS, returning the contents for given directory
        /// </summary>
        Task<FilesystemItemDto> GetDirectoryContentAsync(string path);

        /// <summary>
        /// Get file metadata by path
        /// </summary>
        Task<FileMetadataDto> GetFileMetadataAsync(string path);

        /// <summary>
        /// Tries to read up to maxLength bytes from file, starting at offset
        /// </summary>
        Task<FileContentDto> GetFileContentAsync(string path, long offset, long maxLength);

        /// <summary>
        /// Create directory on server. Returns true in case of success, false otherwise
        /// </summary>
        Task<bool> CreateDirectoryAsync(string path);

        /// <summary>
        /// Delete directory on server. Returns true in case of success, false otherwise
        /// </summary>
        Task<bool> DeleteDirectoryAsync(string path);

        /// <summary>
        /// Create a file at given path. Returns true if successful, false otherwise
        /// </summary>
        Task<bool> CreateFileAsync(string path);

        /// <summary>
        /// Delete file at given path. Returns true if successful, false otherwise
        /// </summary>
        Task<bool> DeleteFileAsync(string path);

        /// <summary>
        /// Write buffer to file at given offset
        /// </summary>
        Task<UpdateFileResultDto> UpdateFileContentAsync(string path, ulong offset, IReadOnlyCollection<byte> buffer);

        /// <summary>
        /// Truncate / grow file to given size
        /// </summary>
        Task<bool> TruncateFileAsync(string path, ulong newSize);

        /// <summary>
        /// Sets timestamps for given file / directory. Returns true in case of success
        /// </summary>
        Task<bool> SetTimestampsAsync(string path, DateTime atime, DateTime ctime, DateTime mtime);

        /// <summary>
        /// Move file / directory
        /// </summary>
        Task<bool> MoveAsync(string from, string to, bool isOverwrite);
    }
}
