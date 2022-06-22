using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.Interfaces.ClientService;
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
        private const int Permissions = 0b111_111_000;

        /// <summary>
        /// Cached content of current directory
        /// </summary>
        private FilesystemItemDto _currentItem =
            new FilesystemItemDto(false, false, string.Empty, string.Empty, 0, new List<FilesystemItemDto>());

        private static readonly byte[] _helloFilePath = Encoding.UTF8.GetBytes("/hello");
        private static readonly byte[] _helloFileContent = Encoding.UTF8.GetBytes("hello world!");

        private static readonly byte[] _yiffDirPath = Encoding.UTF8.GetBytes("/YiffDir");

        public Vfs(IClientService clientService)
        {
            _clientService = clientService;
        }

        public override int GetAttr(ReadOnlySpan<byte> path, ref stat stat, FuseFileInfoRef fiRef)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            UpdateCurrentDirectory(pathAsString);

            if (!_currentItem.IsExist)
            {
                return -ENOENT;
            }

            if (_currentItem == null || !_currentItem.IsExist)
            {
                return -ENOENT;
            }

            if (_currentItem.IsDirectory)
            {
                stat.st_mode = S_IFDIR | Permissions;
                stat.st_nlink = 2; // 2 + nr of subdirectories
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
            if (!path.SequenceEqual(_helloFilePath))
            {
                return -ENOENT;
            }

            if ((fi.flags & O_ACCMODE) != O_RDONLY)
            {
                return -EACCES;
            }

            return 0;
        }

        public override int Read(ReadOnlySpan<byte> path, ulong offset, Span<byte> buffer, ref FuseFileInfo fi)
        {
            if (offset > (ulong)_helloFileContent.Length)
            {
                return 0;
            }

            int intOffset = (int)offset;
            int length = (int)Math.Min(_helloFileContent.Length - intOffset, buffer.Length);
            _helloFileContent.AsSpan().Slice(intOffset, length).CopyTo(buffer);
            return length;
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
                content.AddEntry((item.Name));
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
