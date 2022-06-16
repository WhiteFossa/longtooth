using longtooth.Common.Abstractions.DTOs;
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
            var externalStorageDir = Android.OS.Environment.ExternalStorageDirectory;

            _mountpoints = new List<MountpointDto>();
            _mountpoints.Add(new MountpointDto("Downloads", FilesHelper.NormalizePath(@$"{ externalStorageDir }/Download")));
            _mountpoints.Add(new MountpointDto("DCIM", FilesHelper.NormalizePath(@$"{ externalStorageDir }/DCIM")));
        }

        public async Task<List<MountpointDto>> GetMountpointsAsync()
        {
            return _mountpoints;
        }

        public async Task<DirectoryContentDto> GetDirectoryContentAsync(string serverSidePath)
        {
            _ = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));

            var normalizedPath = FilesHelper.NormalizePath(serverSidePath);

            if (!IsDirectoryBelongsToMountpoint(normalizedPath))
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

        public async Task<DownloadedFileWithContentDto> DownloadFileAsync(string path, ulong start, uint length)
        {
            if (!File.Exists(path))
            {
                return new DownloadedFileWithContentDto(false, start, length, new List<byte>());
            }

            var buffer = new byte[length];

            using (var stream = new FileStream(path, FileMode.Open))
            {
                var newPosition = stream.Seek((long)start, SeekOrigin.Begin);
                if (newPosition != (long)start)
                {
                    return new DownloadedFileWithContentDto(false, start, length, new List<byte>());
                }

                var readAmount = stream.Read(buffer, 0, (int)length);
                if (readAmount != (int)length)
                {
                    return new DownloadedFileWithContentDto(false, start, length, new List<byte>());
                }
            }

            return new DownloadedFileWithContentDto(true, start, length, buffer.ToList());
        }

        public async Task<CreateFileResultDto> CreateNewFileAsync(string newFilePath)
        {
            _ = newFilePath ?? throw new ArgumentNullException(nameof(newFilePath));

            var targetDirectory = Path.GetDirectoryName(newFilePath);

            if (!IsDirectoryBelongsToMountpoint(targetDirectory))
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

        private bool IsDirectoryBelongsToMountpoint(string path)
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

        public async Task<UpdateFileResultDto> UpdateFileAsync(string path, ulong start, List<byte> data)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var targetDirectory = Path.GetDirectoryName(path);

            if (!IsDirectoryBelongsToMountpoint(targetDirectory))
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

            return new UpdateFileResultDto(true, (uint)data.Count());
        }

        public async Task<DeleteFileResultDto> DeleteFileAsync(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var targetDirectory = Path.GetDirectoryName(path);

            if (!IsDirectoryBelongsToMountpoint(targetDirectory))
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

        public async Task<DeleteDirectoryResultDto> DeleteDirectoryAsync(string path)
        {
            _ = path ?? throw new ArgumentNullException(nameof(path));

            var targetDirectory = Path.GetDirectoryName(path);

            if (!IsDirectoryBelongsToMountpoint(targetDirectory))
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
    }
}