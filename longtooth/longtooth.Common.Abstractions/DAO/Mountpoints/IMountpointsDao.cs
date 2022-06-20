using longtooth.Common.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace longtooth.Common.Abstractions.DAO.Mountpoints
{
    /// <summary>
    /// DAO to work with mountpoints
    /// </summary>
    public interface IMountpointsDao
    {
        /// <summary>
        /// Add mountpoint to storage
        /// </summary>
        Task AddMountpointAsync(MountpointDto mountpoint);

        /// <summary>
        /// Tries to get mountpoint by path and name. Returns null if not found, mountpoint otherwise
        /// </summary>
        Task<MountpointDto> GetMountpointAsync(string name, string serverSidePath);

        /// <summary>
        /// Remove all mountpoints with given path and name
        /// </summary>
        Task DeleteMountpointsAsync(MountpointDto mountpoint);

        /// <summary>
        /// Get all existing mountpoints
        /// </summary>
        Task<IReadOnlyCollection<MountpointDto>> GetAllMountpointsAsync();
    }
}
