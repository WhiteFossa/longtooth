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
    }
}
