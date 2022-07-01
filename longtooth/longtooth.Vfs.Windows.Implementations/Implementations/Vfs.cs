using DokanNet;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Vfs.Windows.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace longtooth.Vfs.Windows.Implementations.Implementations
{
    /// <summary>
    /// Dokan VFS implementation
    /// </summary>
    public class Vfs : IDokanOperations, IVfs
    {
        /// <summary>
        /// Maximal length of path component
        /// </summary>
        private const int PathComponentMaxLength = 256;

        /// <summary>
        /// Filesystem name
        /// </summary>
        private const string FilesystemName = "longtooth";

        private IClientService _clientService;

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

        public Vfs(IClientService clientService)
        {
            _clientService = clientService;
        }

        public void Cleanup(string fileName, IDokanFileInfo info)
        {
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
        }

        public NtStatus CreateFile(string fileName,
            DokanNet.FileAccess access,
            FileShare share,
            FileMode mode,
            FileOptions options,
            FileAttributes attributes,
            IDokanFileInfo info)
        {
            // TODO: Implement me
            return DokanResult.Success;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            var unixPath = WindowsPathToUnixPath(fileName);
            UpdateCurrentDirectory(unixPath);

            if (!_currentItem.IsExist || !_currentItem.IsDirectory)
            {
                files = new List<FileInformation>();
                return DokanResult.Success;
            }

            files = new List<FileInformation>(GetFilesList(_currentItem.Content));

            return DokanResult.Success;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            var unixPath = WindowsPathToUnixPath(fileName);
            UpdateCurrentDirectory(unixPath);

            if (!_currentItem.IsExist || !_currentItem.IsDirectory)
            {
                files = new List<FileInformation>();
                return DokanResult.Success;
            }

            files = new List<FileInformation>(GetFilesList(_currentItem.Content));
            files = files
                .Where(f => DokanHelper.DokanIsNameInExpression(searchPattern, f.FileName, false))
                .ToList();

            return DokanResult.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new FileInformation[0];
            return DokanResult.NotImplemented;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            // TODO: Request disk space from phone
            freeBytesAvailable = 512 * 1024 * 1024;
            totalNumberOfBytes = 1024 * 1024 * 1024;
            totalNumberOfFreeBytes = 512 * 1024 * 1024;

            return DokanResult.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, IDokanFileInfo info)
        {
            fileInfo = new FileInformation { FileName = fileName };

            // Root directory
            if (fileName.Equals(@"\"))
            {
                fileInfo.Attributes = FileAttributes.Directory;

                var rootTime = DateTime.UtcNow;

                fileInfo.LastAccessTime = rootTime;
                fileInfo.LastWriteTime = rootTime;
                fileInfo.CreationTime = rootTime;

                return DokanResult.Success;
            }

            // Test file
            if (fileName.Equals("testfile.txt"))
            {
                fileInfo.LastAccessTime = DateTime.UtcNow;
                fileInfo.LastWriteTime = DateTime.UtcNow;
                fileInfo.CreationTime = DateTime.UtcNow;

                return DokanResult.Success;
            }

            return DokanResult.Error;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            security = null;
            return DokanResult.Error;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "longtooth"; // TODO: Put phone name here

            features = FileSystemFeatures.UnicodeOnDisk
                | FileSystemFeatures.CasePreservedNames
                | FileSystemFeatures.CaseSensitiveSearch
                | FileSystemFeatures.SupportsRemoteStorage;

            fileSystemName = FilesystemName;

            maximumComponentLength = PathComponentMaxLength;

            return DokanResult.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            bytesRead = 0;
            return DokanResult.Error;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            return DokanResult.Error;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, IDokanFileInfo info)
        {
            bytesWritten = 0;
            return DokanResult.Error;
        }

        private void UpdateCurrentDirectory(string path)
        {
            if (!_currentItem.Path.Equals(path))
            {
                _currentItem = _clientService.GetDirectoryContentAsync(path).Result;
            }
        }

        private FileAttributes GetAttributes(FilesystemItemDto item)
        {
            if (item.IsDirectory)
            {
                return FileAttributes.Directory;
            }

            return FileAttributes.Normal;
        }

        private IReadOnlyCollection<FileInformation> GetFilesList(IReadOnlyCollection<FilesystemItemDto> items)
        {
            var files = new List<FileInformation>();

            foreach (var item in _currentItem.Content)
            {
                files.Add(new FileInformation()
                {
                    FileName = item.Name,
                    Attributes = GetAttributes(item),
                    LastAccessTime = item.Atime,
                    CreationTime = item.Ctime,
                    LastWriteTime = item.Mtime,
                    Length = item.Size
                });
            }

            return files;
        }

        private string WindowsPathToUnixPath(string windowsPath)
        {
            return windowsPath.Replace(@"\", "/");
        }
    }
}
