using DokanNet;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Common.Implementations.Helpers;
using longtooth.Vfs.Windows.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

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

        /// <summary>
        /// Reasonable read operation block size. Check Constants.MaxPacketSize when setting it.
        /// </summary>
        private const int ReadOperationBlockSize = 1000 * 1024;

        /// <summary>
        /// Reasonable write operation block size. Check Constants.MaxPacketSize when setting it.
        /// </summary>
        private const int WriteOperationBlockSize = 1000 * 1024;

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
            var unixPath = WindowsPathToUnixPath(fileName);
            var fileInfo = _clientService.GetDirectoryContentAsync(unixPath).Result;

            if (info.IsDirectory && fileInfo.IsExist && !fileInfo.IsDirectory)
            {
                return DokanResult.NotADirectory;
            }

            if (mode == FileMode.Create)
            {
                // Create file, if exist - overwrite it
                if (!fileInfo.IsExist)
                {
                    if (!CreateFileOrDirectory(info.IsDirectory, unixPath))
                    {
                        return DokanResult.Error;
                    }

                    InvalidateCurrentDirectoryCachedContent();
                    info.Context = new FileContext()
                    {
                        Position = 0
                    };

                    return DokanResult.Success;
                }
                else
                {
                    // Already exist, overwriting if not a directory
                    if (fileInfo.IsDirectory)
                    {
                        info.IsDirectory = true;
                        return DokanResult.Success;
                    }

                    if (!_clientService.TruncateFileAsync(unixPath, 0).Result)
                    {
                        return DokanResult.Error;
                    }

                    info.Context = new FileContext()
                    {
                        Position = 0
                    };
                    return DokanResult.Success;
                }
            }
            else if (mode == FileMode.OpenOrCreate)
            {
                // Open file, create if not exist
                if (!fileInfo.IsExist)
                {
                    if (!CreateFileOrDirectory(info.IsDirectory, unixPath))
                    {
                        return DokanResult.Error;
                    }

                    InvalidateCurrentDirectoryCachedContent();
                }

                info.Context = new FileContext()
                {
                    Position = 0
                };
                return DokanResult.Success;
            }
            else if (mode == FileMode.CreateNew)
            {
                // Create only if not exist
                if (fileInfo.IsExist)
                {
                    return DokanResult.Error;
                }
                else
                {
                    if (!CreateFileOrDirectory(info.IsDirectory, unixPath))
                    {
                        return DokanResult.Error;
                    }
                    InvalidateCurrentDirectoryCachedContent();

                    info.Context = new FileContext()
                    {
                        Position = 0
                    };
                    return DokanResult.Success;
                }
            }
            else if (mode == FileMode.Open)
            {
                // Just open file
                if (!fileInfo.IsExist)
                {
                    return DokanResult.Error;
                }

                info.Context = new FileContext()
                {
                    Position = 0
                };
                return DokanResult.Success;
            }
            else if (mode == FileMode.Truncate)
            {
                // Truncate an existing file
                if (info.IsDirectory)
                {
                    return DokanResult.Error;
                }

                if (!fileInfo.IsExist)
                {
                    return DokanResult.Error;
                }

                if (!_clientService.TruncateFileAsync(unixPath, 0).Result)
                {
                    return DokanResult.Error;
                }

                info.Context = new FileContext()
                {
                    Position = 0
                };
                return DokanResult.Success;
            }
            else if (mode == FileMode.Append)
            {
                // Create if not exist, then seek to the end
                if (fileInfo.IsDirectory)
                {
                    return DokanResult.Error;
                }

                if (!fileInfo.IsExist)
                {
                    if (!CreateFileOrDirectory(false, unixPath))
                    {
                        return DokanResult.Error;
                    }
                    InvalidateCurrentDirectoryCachedContent();
                }

                info.Context = new FileContext()
                {
                    Position = fileInfo.Size - 1
                };
                return DokanResult.Success;
            }
            else
            {
                throw new ArgumentException(nameof(mode));
            }
        }

        private bool CreateFileOrDirectory(bool isDirectory, string unixPath)
        {
            bool result;

            if (isDirectory)
            {
                result = _clientService.CreateDirectoryAsync(unixPath).Result;
            }
            else
            {
                result = _clientService.CreateFileAsync(unixPath).Result;
            }

            InvalidateCurrentDirectoryCachedContent();

            return result;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            var unixPath = WindowsPathToUnixPath(fileName);

            if (!_clientService.DeleteDirectoryAsync(unixPath).Result)
            {
                return DokanResult.Error;
            }

            return DokanResult.Success;
        }

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            var unixPath = WindowsPathToUnixPath(fileName);

            if(!_clientService.DeleteFileAsync(unixPath).Result)
            {
                return DokanResult.Error;
            }

            return DokanResult.Success;
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

            var unixPath = WindowsPathToUnixPath(fileName);
            UpdateCurrentDirectory(unixPath);

            if (_currentItem == null || !_currentItem.IsExist)
            {
                return DokanResult.FileNotFound;
            }

            fileInfo.LastAccessTime = _currentItem.Atime;
            fileInfo.CreationTime = _currentItem.Ctime;
            fileInfo.LastWriteTime = _currentItem.Mtime;

            if (_currentItem.IsDirectory)
            {
                fileInfo.Attributes = FileAttributes.Directory;
            }
            else
            {
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Length = _currentItem.Size;
            }

            return DokanResult.Success;
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
            var fromUnixPath = WindowsPathToUnixPath(oldName);
            var toUnixPath = WindowsPathToUnixPath(newName);

            if (!_clientService.MoveAsync(fromUnixPath, toUnixPath, replace).Result)
            {
                return DokanResult.Error;
            }

            return DokanResult.Success;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            var unixPath = WindowsPathToUnixPath(fileName);

            var toReadTotal = buffer.Count();

            var alreadyRead = 0;

            while (alreadyRead < toReadTotal)
            {
                var toRead = Math.Min(toReadTotal - alreadyRead, ReadOperationBlockSize);

                var content = _clientService.GetFileContentAsync(unixPath, offset + alreadyRead, toRead).Result;
                if (!content.IsExist || !content.IsInRagne)
                {
                    bytesRead = 0;
                    return DokanResult.Error;
                }

                if (content.Content.Count == 0)
                {
                    // End of file
                    break;
                }

                Array.Copy(content.Content.ToArray(), 0, buffer, alreadyRead, content.Content.Count);

                alreadyRead += content.Content.Count;
            }

            bytesRead = alreadyRead;
            return DokanResult.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
        {
            // Imitating success but doing nothing, it's because we need SetFileAttributes to be implemented for SetFileTime calls
            return DokanResult.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime, IDokanFileInfo info)
        {
            var unixPath = WindowsPathToUnixPath(fileName);

            // Current times
            var currentItem = _clientService.GetDirectoryContentAsync(unixPath).Result;
            if (!currentItem.IsExist)
            {
                return DokanResult.Error;
            }

            var newAtime = lastAccessTime ?? currentItem.Atime;
            var newCtime = creationTime ?? currentItem.Ctime;
            var newMtime = lastWriteTime ?? currentItem.Mtime;

            var result = _clientService.SetTimestampsAsync(unixPath, newAtime, newCtime, newMtime).Result;
            if (!result)
            {
                return DokanResult.Error;
            }

            InvalidateCurrentDirectoryCachedContent();

            return DokanResult.Success;
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
            var unixPath = WindowsPathToUnixPath(fileName);

            var toWriteTotal = buffer.Count();

            var alreadyWritten = 0;

            while (alreadyWritten < toWriteTotal)
            {
                var toWrite = Math.Min(toWriteTotal - alreadyWritten, ReadOperationBlockSize);

                if (toWrite == 0)
                {
                    // End of file
                    break;
                }

                var bufferToWrite = new byte[toWrite];
                Array.Copy(buffer, alreadyWritten, bufferToWrite, 0, toWrite);

                var result = _clientService.UpdateFileContentAsync(unixPath, (ulong)(alreadyWritten + offset), bufferToWrite).Result;
                if (!result.IsSuccessful)
                {
                    bytesWritten = 0;
                    return DokanResult.Error;
                }

                if (result.BytesWritten != toWrite)
                {
                    bytesWritten = 0;
                    return DokanResult.Error;
                }

                alreadyWritten += result.BytesWritten;
            }

            bytesWritten = alreadyWritten;
            return DokanResult.Success;
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
                    FileName = FilesHelper.GetFileOrDirectoryName(item.Path),
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
