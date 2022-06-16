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
        /// Returns mountpoints, shared on server
        /// </summary>
        Task<List<MountpointDto>> GetMountpointsAsync();

        /// <summary>
        /// Returns directory content. Directory name MUST end with slash
        /// </summary>
        Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath);

        /// <summary>
        /// Downloads part of file
        /// </summary>
        Task<DownloadedFileWithContentDto> DownloadFileAsync(string path, ulong start, uint length);

        /// <summary>
        /// Create new empty file
        /// </summary>
        Task<CreateFileResultDto> CreateNewFileAsync(string newFilePath);

        /// <summary>
        /// Update file starting from given position
        /// </summary>
        Task<UpdateFileResultDto> UpdateFileAsync(string path, ulong start, List<byte> data);

        /// <summary>
        /// Delete file
        /// </summary>
        Task<DeleteFileResultDto> DeleteFileAsync(string path);
    }
}
