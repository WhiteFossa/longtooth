using longtooth.Common.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace longtooth.FilesManager.Abstractions.Interfaces
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
    }
}
