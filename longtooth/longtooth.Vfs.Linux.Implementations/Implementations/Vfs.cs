using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private const int Permissions = 0b111_111_000;

        /// <summary>
        /// Reasonable read operation block size. Check Constants.MaxPacketSize when setting it.
        /// </summary>
        private const int ReadOperationBlockSize = 1000 * 1024;

        /// <summary>
        /// Cached content of current directory
        /// </summary>
        private FilesystemItemDto _currentItem =
            new FilesystemItemDto(false,
                false,
                string.Empty,
                string.Empty,
                0,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                new List<FilesystemItemDto>());

        /// <summary>
        /// User, who started the client
        /// </summary>
        private uid_t _curentUser;

        /// <summary>
        /// Group of user, who started the client
        /// </summary>
        private gid_t _currentGroup;

        public Vfs(IClientService clientService)
        {
            _clientService = clientService;

            _curentUser = getuid();
            _currentGroup = getgid();
        }

        public override int GetAttr(ReadOnlySpan<byte> path, ref stat stat, FuseFileInfoRef fiRef)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            UpdateCurrentDirectory(pathAsString);

            if (_currentItem == null || !_currentItem.IsExist)
            {
                return -ENOENT;
            }

            // Modification times
            stat.st_atim = _currentItem.Atime.ToTimespec();
            stat.st_ctim = _currentItem.Ctime.ToTimespec();
            stat.st_mtim = _currentItem.Mtime.ToTimespec();

            // Owner and group - always return the current user and group
            stat.st_uid = _curentUser;
            stat.st_gid = _currentGroup;

            if (_currentItem.IsDirectory)
            {
                var subdirectoriesCount = _currentItem
                    .Content
                    .Count(i => i.IsDirectory);

                stat.st_mode = S_IFDIR | Permissions;
                stat.st_nlink = (ulong)subdirectoriesCount + 2; // Number of subdirectories + 2 (because . and .. directories)

                return 0;
            }

            // File
            stat.st_mode = S_IFREG | Permissions;
            stat.st_nlink = 1;
            stat.st_size = _currentItem.Size;

            return 0;
        }

        public override int Open(ReadOnlySpan<byte> path, ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var metadata = _clientService.GetFileMetadataAsync(pathAsString).Result;

            if (!metadata.IsExist)
            {
                return -ENOENT;
            }

            // TODO: Put here access detection

            return 0;
        }

        public override int Read(ReadOnlySpan<byte> path, ulong offset, Span<byte> buffer, ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var toRead = Math.Min(buffer.Length, ReadOperationBlockSize);
            var fileContent = _clientService.GetFileContentAsync(pathAsString, (long)offset, toRead).Result;

            if (!fileContent.IsExist)
            {
                return -ENOENT;
            }

            if (!fileContent.IsInRagne)
            {
                return -EINVAL;
            }

            fileContent.Content.CopyTo(buffer);

            return fileContent.Content.Length;
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

        public override int MkDir(ReadOnlySpan<byte> path, mode_t mode)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var result = _clientService.CreateDirectoryAsync(pathAsString).Result;
            if (!result)
            {
                return -EEXIST;
            }

            InvalidateCurrentDirectoryCachedContent();

            return 0;
        }

        public override int RmDir(ReadOnlySpan<byte> path)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var result = _clientService.DeleteDirectoryAsync(pathAsString).Result;
            if (!result)
            {
                return -ENOENT;
            }

            InvalidateCurrentDirectoryCachedContent();

            return 0;
        }

        public override int Create(ReadOnlySpan<byte> path, mode_t mode, ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var result = _clientService.CreateFileAsync(pathAsString).Result;
            if (!result)
            {
                return -EEXIST;
            }

            InvalidateCurrentDirectoryCachedContent();

            return 0;
        }

        public override int Unlink(ReadOnlySpan<byte> path)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var result = _clientService.DeleteFileAsync(pathAsString).Result;
            if (!result)
            {
                return -ENOENT;
            }

            InvalidateCurrentDirectoryCachedContent();

            return 0;
        }

        public override int Write(
            ReadOnlySpan<byte> path, ulong offset, ReadOnlySpan<byte> span, ref FuseFileInfo fi)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var result = _clientService.UpdateFileContentAsync(pathAsString, offset, span.ToArray());
            if (!result.Result.IsSuccessful)
            {
                return -EIO;
            }

            return result.Result.BytesWritten;
        }

        public override int Truncate(ReadOnlySpan<byte> path, ulong length, FuseFileInfoRef fiRef)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var result = _clientService.TruncateFileAsync(pathAsString, length).Result;
            if (!result)
            {
                return -EIO;
            }

            return 0;
        }

        public override int UpdateTimestamps(
            ReadOnlySpan<byte> path,
            ref timespec atime,
            ref timespec mtime,
            FuseFileInfoRef fiRef)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            // Current times
            var item = _clientService.GetDirectoryContentAsync(pathAsString).Result;

            var newAtime = ProcessDateTimeForUtime(DateTime.UtcNow, item.Atime.ToTimespec());
            var newMtime = ProcessDateTimeForUtime(DateTime.UtcNow, item.Mtime.ToTimespec());

            var result = _clientService.SetTimestampsAsync(pathAsString, newAtime, newMtime, newMtime).Result;
            if (!result)
            {
                return -EIO;
            }

            return 0;
        }


        public override int Access(ReadOnlySpan<byte> path, mode_t mode)
        {
            var pathAsString = Encoding.UTF8.GetString(path);

            var item = _clientService.GetDirectoryContentAsync(pathAsString).Result;

            if (!item.IsExist)
            {
                return -ENOENT;
            }

            // We don't actually checking the permissions, 770 is for all directories and files, i.e. current user
            // always have full access
            return 0;
        }

        private DateTime ProcessDateTimeForUtime(DateTime currentDateTime, timespec newDateTime)
        {
            if (newDateTime.IsNow())
            {
                return DateTime.UtcNow;
            }

            if (newDateTime.IsOmit())
            {
                return currentDateTime;
            }

            return newDateTime.ToDateTime();
        }

        private void UpdateCurrentDirectory(string path)
        {
            if (!_currentItem.Path.Equals(path))
            {
                _currentItem = _clientService.GetDirectoryContentAsync(path).Result;
            }
        }

        /// <summary>
        /// Call this after changes in current directory (creating/removig files and directories and so on).
        /// </summary>
        private void InvalidateCurrentDirectoryCachedContent()
        {
            _currentItem = new FilesystemItemDto(false,
                true,
                string.Empty,
                string.Empty,
                0,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                new List<FilesystemItemDto>());
        }
    }
}
