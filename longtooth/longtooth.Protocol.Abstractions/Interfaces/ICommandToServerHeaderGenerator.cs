using System;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to generate commands to server
    /// </summary>
    public interface ICommandToServerHeaderGenerator
    {
        /// <summary>
        /// Generates ping command
        /// </summary>
        IReadOnlyCollection<byte> GeneratePingCommand();

        /// <summary>
        /// Gracefully close the connection
        /// </summary>
        IReadOnlyCollection<byte> GenerateExitCommand();

        /// <summary>
        /// Get mountpoints list from server
        /// </summary>
        IReadOnlyCollection<byte> GenerateGetMountpointsCommand();

        /// <summary>
        /// Get server directory content
        /// </summary>
        IReadOnlyCollection<byte> GenerateGetDirectoryContentCommand(string serverSidePath);

        /// <summary>
        /// Generate "download a file" command
        /// </summary>
        IReadOnlyCollection<byte> GenerateDownloadCommand(string path, long startPosition, int length);

        /// <summary>
        /// Generate "create a file" command
        /// </summary>
        IReadOnlyCollection<byte> CreateFileCommand(string path);

        /// <summary>
        /// Generate "update a file" command
        /// </summary>
        IReadOnlyCollection<byte> UpdateFileCommand(string path, long startPosition, byte[] dataToWrite);

        /// <summary>
        /// Generate "delete file" command
        /// </summary>
        IReadOnlyCollection<byte> DeleteFileCommand(string path);

        /// <summary>
        /// Generate "delete directory" command
        /// </summary>
        IReadOnlyCollection<byte> DeleteDirectoryCommand(string path);

        /// <summary>
        /// Generate "create directory" command
        /// </summary>
        IReadOnlyCollection<byte> CreateDirectoryCommand(string path);

        /// <summary>
        /// Generate "get file info" command
        /// </summary>
        IReadOnlyCollection<byte> GetFileInfoCommand(string path);

        /// <summary>
        /// Generate "truncate file" command
        /// </summary>
        IReadOnlyCollection<byte> TruncateFileCommand(string path, ulong newSize);

        /// <summary>
        /// Generate "set timestamps" command
        /// </summary>
        IReadOnlyCollection<byte> SetTimestampsCommand(string path, DateTime atime, DateTime ctime, DateTime mtime);

        /// <summary>
        /// Generates "move file or directory" command.
        /// </summary>
        IReadOnlyCollection<byte> MoveCommand(string from, string to, bool isOverwrite);

        /// <summary>
        /// Generates "get disk space by path" command.
        /// </summary>
        IReadOnlyCollection<byte> GetDiskSpaceCommand(string path);
    }
}
