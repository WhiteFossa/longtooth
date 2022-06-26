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
        /// Maximal read block size. Check Constants.MaxPacketSize when setting this setting.
        /// </summary>
        private const int MaxReadBlockSize = 1000 * 1024;

        /// <summary>
        /// Shared directories
        /// </summary>
        private List<MountpointDto> _mountpoints;

        public FilesManagerImplementation()
        {
            _mountpoints = new List<MountpointDto>();
        }

        public async Task<IReadOnlyCollection<MountpointDto>> GetMountpointsAsync()
        {
            return _mountpoints.AsReadOnly();
        }

        public async Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath)
        {
            try
            {
                _ = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));

                var normalizedPath = FilesHelper.NormalizePath(serverSidePath);

                if (!IsDirectoryBelongsToMountpoints(normalizedPath))
                {
                    // Non-exported directory
                    return new DirectoryContentDto(false, new List<DirectoryContentItemDto>());
                }

                var directories = Directory.GetDirectories(normalizedPath);
                var files = (new DirectoryInfo(normalizedPath)).GetFiles();

                var result = directories
                        .Select(d => new DirectoryContentItemDto(true, d, 0)) // Directories have no size
                         .Union(files.Select(f => new DirectoryContentItemDto(false, f.Name, f.Length)))
                         .ToList();

                // Removing base directory
                result = result
                    .Select(dci => new DirectoryContentItemDto(
                        dci.IsDirectory,
                        dci.IsDirectory ? dci.Name.Substring(normalizedPath.Length + 1) : dci.Name, // +1 for trailing / of normalized path
                        dci.Size))
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
                    result = new List<DirectoryContentItemDto>() { new DirectoryContentItemDto(true, "..", 0) }
                        .Union(result)
                        .ToList();
                }

                return new DirectoryContentDto(true, result);
            }
            catch (Exception)
            {
                return new DirectoryContentDto(false, new List<DirectoryContentItemDto>());
            }
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

        public async Task<DownloadedFileWithContentDto> DownloadFileAsync(string path, long start, int length)
        {
            try
            {
                _ = path ?? throw new ArgumentNullException(nameof(path));

                var targetDirectory = Path.GetDirectoryName(path);

                if (!IsDirectoryBelongsToMountpoints(targetDirectory))
                {
                    // Non-exported directory
                    return new DownloadedFileWithContentDto(false, 0, 0, new List<byte>());
                }

                if (!File.Exists(path))
                {
                    return new DownloadedFileWithContentDto(false, 0, 0, new List<byte>());
                }

                var toRead = Math.Min(length, MaxReadBlockSize);

                var buffer = new byte[toRead];
                int bytesRead;

                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var newPosition = stream.Seek((long)start, SeekOrigin.Begin);
                    if (newPosition != (long)start)
                    {
                        return new DownloadedFileWithContentDto(false, 0, 0, new List<byte>());
                    }

                    // We can't guarantee that length of bytes will be read
                    bytesRead = stream.Read(buffer, 0, toRead);
                }

                if (bytesRead > toRead)
                {
                    throw new InvalidOperationException("Got more that requested!");
                }

                if (bytesRead < toRead)
                {
                    return new DownloadedFileWithContentDto(true, start, bytesRead, buffer.ToList().GetRange(0, bytesRead));
                }

                return new DownloadedFileWithContentDto(true, start, bytesRead, buffer.ToList());
            }
            catch (Exception)
            {
                return new DownloadedFileWithContentDto(false, 0, 0, new List<byte>());
            }
        }

        public async Task<CreateFileResultDto> CreateNewFileAsync(string newFilePath)
        {
            try
            {
                _ = newFilePath ?? throw new ArgumentNullException(nameof(newFilePath));

                var targetDirectory = Path.GetDirectoryName(newFilePath);

                if (!IsDirectoryBelongsToMountpoints(targetDirectory))
                {
                    // Non-exported directory
                    return new CreateFileResultDto(false);
                }

                if (File.Exists(newFilePath))
                {
                    // File already exist
                    return new CreateFileResultDto(false);
                }

                using (var stream = File.Create(newFilePath))
                {

                }

                return new CreateFileResultDto(true);
            }
            catch (Exception)
            {
                return new CreateFileResultDto(false);
            }
        }

        private bool IsDirectoryBelongsToMountpoints(string path)
        {
            // Is directory belongs to one of mountpoints?
            var isChildDirectory = false;
            foreach (var mountpoint in _mountpoints)
            {
                if (IsDirectoryInsideAnotherOrTheSame(mountpoint.ServerSidePath, path))
                {
                    isChildDirectory = true;
                    break;
                }
            }

            return isChildDirectory;
        }

        public async Task<UpdateFileResultDto> UpdateFileAsync(string path, long start, IReadOnlyCollection<byte> data)
        {
            try
            {
                _ = path ?? throw new ArgumentNullException(nameof(path));

                var targetDirectory = Path.GetDirectoryName(path);

                if (!IsDirectoryBelongsToMountpoints(targetDirectory))
                {
                    // Non-exported directory
                    return new UpdateFileResultDto(false, 0);
                }

                if (!File.Exists(path))
                {
                    // File not exist
                    return new UpdateFileResultDto(false, 0);
                }

                using (var stream = new FileStream(path, FileMode.Open))
                {
                    var newPosition = stream.Seek((long)start, SeekOrigin.Begin);

                    if (newPosition != (long)start)
                    {
                        // Position is out of file
                        return new UpdateFileResultDto(false, 0);
                    }

                    stream.Write(data.ToArray(), 0, data.Count);
                }

                return new UpdateFileResultDto(true, data.Count());
            }
            catch (Exception)
            {
                return new UpdateFileResultDto(false, 0);
            }
        }

        public async Task<DeleteFileResultDto> DeleteFileAsync(string path)
        {
            try
            {
                _ = path ?? throw new ArgumentNullException(nameof(path));

                var targetDirectory = Path.GetDirectoryName(path);

                if (!IsDirectoryBelongsToMountpoints(targetDirectory))
                {
                    // Non-exported directory
                    return new DeleteFileResultDto(false);
                }

                if (!File.Exists(path))
                {
                    // File not exist
                    return new DeleteFileResultDto(false);
                }

                File.Delete(path);

                return new DeleteFileResultDto(true);
            }
            catch (Exception)
            {
                return new DeleteFileResultDto(false);
            }
        }

        public async Task<DeleteDirectoryResultDto> DeleteDirectoryAsync(string path)
        {
            try
            {
                _ = path ?? throw new ArgumentNullException(nameof(path));

                var targetDirectory = Path.GetDirectoryName(path);

                if (!IsDirectoryBelongsToMountpoints(targetDirectory))
                {
                    // Non-exported directory
                    return new DeleteDirectoryResultDto(false);
                }

                if (_mountpoints
                    .Where(mp => mp.ServerSidePath == path)
                    .Any())
                {
                    // Mountpoints are not-removable
                    return new DeleteDirectoryResultDto(false);
                }

                if (!Directory.Exists(path))
                {
                    // Directory doesn't exist
                    return new DeleteDirectoryResultDto(false);
                }

                Directory.Delete(path);

                return new DeleteDirectoryResultDto(true);
            }
            catch (Exception)
            {
                return new DeleteDirectoryResultDto(false);
            }
        }

        public async Task<CreateDirectoryResultDto> CreateDirectoryAsync(string path)
        {
            try
            {
                _ = path ?? throw new ArgumentNullException(nameof(path));

                var targetDirectory = Path.GetDirectoryName(path);

                if (!IsDirectoryBelongsToMountpoints(targetDirectory))
                {
                    // Non-exported directory
                    return new CreateDirectoryResultDto(false);
                }

                if (Directory.Exists(path))
                {
                    // Directory already exist
                    return new CreateDirectoryResultDto(false);
                }

                Directory.CreateDirectory(path);

                return new CreateDirectoryResultDto(true);
            }
            catch (Exception)
            {
                return new CreateDirectoryResultDto(false);
            }
        }

        public void AddMountpoint(MountpointDto mountpoint)
        {
            var isDuplicate = _mountpoints
                .Where(mp => mp.Name.Equals(mountpoint.Name) ||  mp.ServerSidePath.Equals(mountpoint.ServerSidePath))
                .Any();

            if (isDuplicate)
            {
                throw new ArgumentException("Duplicated mountpoint!", nameof(mountpoint));
            }

            _mountpoints.Add(mountpoint);
        }

        public void RemoveMountpoint(string path)
        {
            _mountpoints = _mountpoints
                .Where(mp => mp.ServerSidePath != path)
                .ToList();
        }

        public IReadOnlyCollection<MountpointDto> ListMountpoints()
        {
            return _mountpoints;
        }

        public void ClearAllMountpoints()
        {
            _mountpoints.Clear();
        }

        public async Task<GetFileInfoResultDto> GetFileInfoAsync(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var targetDirectory = Path.GetDirectoryName(path);

            if (!IsDirectoryBelongsToMountpoints(targetDirectory))
            {
                // Non-exported directory
                return new GetFileInfoResultDto(false, path, "", 0);
            }

            if (!File.Exists(path))
            {
                return new GetFileInfoResultDto(false, path, "", 0);
            }

            var info = new FileInfo(path);

            return new GetFileInfoResultDto(true, path, info.Name, info.Length);
        }

        public async Task<TruncateFileResultDto> TruncateFileAsync(string path, ulong newSize)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var targetDirectory = Path.GetDirectoryName(path);

            if (!IsDirectoryBelongsToMountpoints(targetDirectory))
            {
                // Non-exported directory
                return new TruncateFileResultDto(false);
            }

            if (!File.Exists(path))
            {
                return new TruncateFileResultDto(false);
            }

            var info = new FileInfo(path);

            using (var stream = new FileStream(path, FileMode.Open))
            {
                if ((long)newSize < info.Length)
                {
                    stream.SetLength((long)newSize);
                }
                else if ((long)newSize > info.Length)
                {
                    var toWrite = newSize - (ulong)info.Length;

                    stream.Seek(0, SeekOrigin.End);

                    // TODO: Dirty, optimize me
                    var buffer = new byte[toWrite];
                    for (ulong i = 0; i < toWrite; i++)
                    {
                        buffer[i] = 0x00;
                    }
                    stream.Write(buffer);
                }
            }

            return new TruncateFileResultDto(true);
        }
    }
}
