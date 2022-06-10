using longtooth.Common.Abstractions.DTOs;
using longtooth.FilesManager.Abstractions.Interfaces;
using System;
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

        public async Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath)
        {
            _ = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));

            var mockedContent = new List<DirectoryContentItemDto>();
            mockedContent.Add(new DirectoryContentItemDto(true, "Test directory 1"));
            mockedContent.Add(new DirectoryContentItemDto(true, "Test directory 2"));

            mockedContent.Add(new DirectoryContentItemDto(false, "Test file 1"));
            mockedContent.Add(new DirectoryContentItemDto(false, "Test file 2"));
            mockedContent.Add(new DirectoryContentItemDto(false, "Test file 3"));

            return new DirectoryContentDto(true, mockedContent);
        }
    }
}