using Acr.UserDialogs;
using longtooth.Abstractions.Interfaces.Permissions;
using longtooth.Common.Abstractions.DTOs;
using longtooth.FilesManager.Abstractions.Interfaces;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace longtooth.FilesManager.Implementations.Implementations
{
    public class FilesManagerImplementation : IFilesManager
    {
        private readonly IPermissionsManager _permissionsManager;


        /// <summary>
        /// Shared directories
        /// </summary>
        private List<MountpointDto> _mountpoints;

        public FilesManagerImplementation(IPermissionsManager permissionsManager)
        {
            _permissionsManager = permissionsManager;

            // Mocked mountpoints
            _mountpoints = new List<MountpointDto>();
            _mountpoints.Add(new MountpointDto("Downloads", @"/storage/emulated/0/Download/"));
            _mountpoints.Add(new MountpointDto("DCIM", @"/storage/emulated/0/DCIM/"));
        }

        public async Task<List<MountpointDto>> GetMountpointsAsync()
        {
            return _mountpoints;
        }

        public async Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath)
        {
            _ = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));

            // Is directory belongs to one of mountpoints?
            var isChildDirectory = false;
            foreach(var mountpoint in _mountpoints)
            {
                if (IsDirectoryInsideAnotherOrTheSame(mountpoint.ServerSidePath, serverSidePath))
                {
                    isChildDirectory = true;
                    break;
                }
            }

            if (!isChildDirectory)
            {
                // Non-exported directory
                return new DirectoryContentDto(false, new List<DirectoryContentItemDto>());
            }

            // Working with permissions
            var doWeHavePermission = await _permissionsManager.RequestPermission<StoragePermission>(Permission.Storage, "Access to storage is required!");
            if (!doWeHavePermission)
            {
                return new DirectoryContentDto(false, new List<DirectoryContentItemDto>());
            }

            // Now we must have permission
            var directories = Directory.GetDirectories(serverSidePath);
            var files = Directory.GetFiles(serverSidePath);

            var result = directories
                    .Select(d => new DirectoryContentItemDto(true, d))
                     .Union(files.Select(f => new DirectoryContentItemDto(false, f)))
                     .ToList();

            // Removing base directory
            result = result
                .Select(dci => new DirectoryContentItemDto(dci.IsDirectory, dci.Name.Replace(serverSidePath, string.Empty))) // TODO: Not so fast, but reliable
                .ToList();

            return new DirectoryContentDto(true, result);
        }

        private bool IsDirectoryInsideAnotherOrTheSame(string upperDirectory, string lowerDirectory)
        {
            var upperDirectoryInfo = new DirectoryInfo(upperDirectory);
            var lowerDirectoryInfo = new DirectoryInfo(lowerDirectory);

            if (upperDirectoryInfo.FullName == lowerDirectoryInfo.FullName)
            {
                return true;
            }

            var isParent = false;
            while (lowerDirectoryInfo.Parent != null)
            {
                if (lowerDirectoryInfo.Parent.FullName == upperDirectoryInfo.FullName)
                {
                    isParent = true;
                    break;
                }
                else
                {
                    lowerDirectoryInfo = lowerDirectoryInfo.Parent;
                }
            }

            return isParent;
        }
    }
}