using DokanNet;
using longtooth.Vfs.Windows.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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
            files = new List<FileInformation>();

            if (fileName.Equals(@"\"))
            {
                files.Add(new FileInformation
                {
                    FileName = "testfile.txt",
                    LastAccessTime = DateTime.UtcNow,
                    LastWriteTime = DateTime.UtcNow,
                    CreationTime = DateTime.UtcNow
                });

                return DokanResult.Success;
            }

            return DokanResult.Success;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new FileInformation[0];
            return DokanResult.NotImplemented;
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
    }
}
