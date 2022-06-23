using longtooth.Common.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace longtooth.Common.Abstractions.Interfaces.FilesManager
{
    /// <summary>
    /// Interface for work with server filesystem
    /// </summary>
    public interface IFilesManager
    {
        /// <summary>
        /// Add mountpoint to mountpoints list
        /// </summary>
        void AddMountpoint(MountpointDto mountpoint);

        /// <summary>
        /// Remove mountpoint with given path
        /// </summary>
        void RemoveMountpoint(string path);

        /// <summary>
        /// Removes all mountpoints
        /// </summary>
        void ClearAllMountpoints();

        /// <summary>
        /// Lists current mountpoints
        /// </summary>
        IReadOnlyCollection<MountpointDto> ListMountpoints();

        /// <summary>
        /// Returns mountpoints, shared on server
        /// </summary>
        Task<IReadOnlyCollection<MountpointDto>> GetMountpointsAsync();

        /// <summary>
        /// Returns directory content. Directory name MUST end with slash
        /// </summary>
        Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath);

        /// <summary>
        /// Downloads part of file
        /// </summary>
        Task<DownloadedFileWithContentDto> DownloadFileAsync(string path, long start, int length);

        /// <summary>
        /// Create new empty file
        /// </summary>
        Task<CreateFileResultDto> CreateNewFileAsync(string newFilePath);

        /// <summary>
        /// Update file starting from given position
        /// </summary>
        Task<UpdateFileResultDto> UpdateFileAsync(string path, long start, IReadOnlyCollection<byte> data);

        /// <summary>
        /// Delete file
        /// </summary>
        Task<DeleteFileResultDto> DeleteFileAsync(string path);

        /// <summary>
        /// Delete directory
        /// </summary>
        Task<DeleteDirectoryResultDto> DeleteDirectoryAsync(string path);

        /// <summary>
        /// Create directory
        /// </summary>
        Task<CreateDirectoryResultDto> CreateDirectoryAsync(string path);

        /// <summary>
        /// Get information about file
        /// </summary>
        Task<GetFileInfoResultDto> GetFileInfoAsync(string path);


    }
}
