using longtooth.Common.Abstractions.DTOs;
using longtooth.FilesManager.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace longtooth.FilesManager.Implementations.Implementations
{
    public class FilesManagerImplementation : IFilesManager
    {
        /// <summary>
        /// Shared directories
        /// </summary>
        private List<MountpointDto> _mountpoints;

        public FilesManagerImplementation()
        {
            // Mocked mountpoints
            _mountpoints = new List<MountpointDto>();
            _mountpoints.Add(new MountpointDto("Downloads", "/storage/sdcard0/Download"));
            _mountpoints.Add(new MountpointDto("DCIM", "/storage/sdcard0/DCIM"));
        }

        public async Task<List<MountpointDto>> GetMountpointsAsync()
        {
            return _mountpoints;
        }
    }
}
