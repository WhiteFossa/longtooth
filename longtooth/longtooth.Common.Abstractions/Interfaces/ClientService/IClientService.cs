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
    }
}
