﻿using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Implementations.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace longtooth.Droid.Implementations.FilesManager
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
            _mountpoints.Add(new MountpointDto("Downloads", FilesHelper.NormalizePath(@"/storage/emulated/0/Download")));
            _mountpoints.Add(new MountpointDto("DCIM", FilesHelper.NormalizePath(@"/storage/emulated/0/DCIM")));
            //_mountpoints.Add(new MountpointDto("Root", FilesHelper.NormalizePath(@"/")));
        }

        public async Task<List<MountpointDto>> GetMountpointsAsync()
        {
            return _mountpoints;
        }

        public async Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath)
        {
            _ = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));

            var normalizedPath = FilesHelper.NormalizePath(serverSidePath);

            // Is directory belongs to one of mountpoints?
            var isChildDirectory = false;
            foreach (var mountpoint in _mountpoints)
            {
                if (IsDirectoryInsideAnotherOrTheSame(mountpoint.ServerSidePath, normalizedPath))
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

            string[] directories = null;
            string[] files = null;

            try
            {
                directories = Directory.GetDirectories(normalizedPath);
                files = Directory.GetFiles(normalizedPath);
            }
            catch (Exception ex)
            {
                int a = 10;
            }

            var result = directories
                    .Select(d => new DirectoryContentItemDto(true, d))
                     .Union(files.Select(f => new DirectoryContentItemDto(false, f)))
                     .ToList();

            // Removing base directory
            result = result
                .Select(dci => new DirectoryContentItemDto(dci.IsDirectory, dci.Name.Substring(normalizedPath.Length + 1))) // +1 for trailing / of normalized path
                .ToList();

            // Adding "move up" if not root directory
            var isMountpoint = false;
            foreach (var mountpoint in _mountpoints)
            {
                if (serverSidePath.Equals(mountpoint.ServerSidePath))
                {
                    isMountpoint = true;
                    break;
                }
            }

            if (!isMountpoint)
            {
                result = new List<DirectoryContentItemDto>() { new DirectoryContentItemDto(true, "..") }
                    .Union(result)
                    .ToList();
            }

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