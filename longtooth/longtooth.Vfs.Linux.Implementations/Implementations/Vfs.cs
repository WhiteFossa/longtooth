using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Common.Implementations.Helpers;
using longtooth.Vfs.Linux.Abstractions.Interfaces;
using Tmds.Fuse;
using Tmds.Linux;
using static Tmds.Linux.LibC;

namespace longtooth.Vfs.Linux.Implementations.Implementations
{
    /// <summary>
    /// FUSE implementation
    /// </summary>
    public class Vfs : FuseFileSystemBase, IVfs
    {
        private IClientService _clientService;

        /// <summary>
        /// Permissions for items, exported by FUSE
        /// </summary>
        private const int Permissions = 0b111_111_111;

        /// <summary>
        /// Cached content of current directory
        /// </summary>
        private FilesystemItemDto _currentItem =
            new FilesystemItemDto(false, false, string.Empty, string.Empty, 0, new List<FilesystemItemDto>());

        private static readonly byte[] _helloFileContent = Encoding.UTF8.GetBytes("hello world!");


        public Vfs(IClientService clientService)
        {
            _clientService = clientService;
        }

        public override int GetAttr(ReadOnlySpan<byte> path, ref stat stat, FuseFileInfoRef fiRef)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            UpdateCurrentDirectory(pathAsString);

            if (_currentItem == null || !_currentItem.IsExist)
            {
                return -ENOENT;
            }

            if (_currentItem.IsDirectory)
            {
                var subdirectoriesCount = _currentItem
                    .Content
                    .Count(i => i.IsDirectory);

                stat.st_mode = S_IFDIR | Permissions;
                stat.st_nlink = (ulong)subdirectoriesCount + 2; // Number of subdirectories + 2 (because . and .. directories)
                return 0;
            }
            else
            {
                stat.st_mode = S_IFREG | Permissions;
                stat.st_nlink = 1;
                stat.st_size = _currentItem.Size;
                return 0;
            }
        }

        public override int Open(ReadOnlySpan<byte> path, ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var metadata = _clientService.GetFileMetadata(pathAsString).Result;

            if (!metadata.IsExist)
            {
                return -ENOENT;
            }

            // TODO: Temporarily read-only VFS
            if ((fi.flags & O_ACCMODE) != O_RDONLY)
            {
                return -EACCES;
            }

            return 0;
        }

        public override int Read(ReadOnlySpan<byte> path, ulong offset, Span<byte> buffer, ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var fileContent = _clientService.GetFileContent(pathAsString, (long)offset, buffer.Length).Result;

            if (!fileContent.IsExist || !fileContent.IsInRagne)
            {
                return 0;
            }

            fileContent.Content.ToArray().CopyTo(buffer);
            return fileContent.Content.Count;
        }

        public override int ReadDir(ReadOnlySpan<byte> path, ulong offset, ReadDirFlags flags, DirectoryContent content,
            ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            UpdateCurrentDirectory(pathAsString);

            if (!_currentItem.IsExist || !_currentItem.IsDirectory)
            {
                return -ENOENT;
            }

            foreach (var item in _currentItem.Content)
            {
                content.AddEntry((FilesHelper.GetFileOrDirectoryName(item.Path)));
            }

            return 0;
        }

        private void UpdateCurrentDirectory(string path)
        {
            if (!_currentItem.Path.Equals(path))
            {
                _currentItem = _clientService.GetDirectoryContentAsync(path).Result;
            }
        }
    }
}
