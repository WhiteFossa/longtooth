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
        byte[] GeneratePingCommand();

        /// <summary>
        /// Gracefully close the connection
        /// </summary>
        byte[] GenerateExitCommand();

        /// <summary>
        /// Get mountpoints list from server
        /// </summary>
        byte[] GenerateGetMountpointsCommand();

        /// <summary>
        /// Get server directory content
        /// </summary>
        byte[] GenerateGetDirectoryContentCommand(string serverSidePath);

        /// <summary>
        /// Generate "download a file" command
        /// </summary>
        byte[] GenerateDownloadCommand(string path, ulong startPosition, uint length);

        /// <summary>
        /// Generate "create a file" command
        /// </summary>
        byte[] CreateFileCommand(string path);

        /// <summary>
        /// Generate "update a file" command
        /// </summary>
        byte[] UpdateFileCommand(string path, ulong startPosition, byte[] dataToWrite);

        /// <summary>
        /// Generate "delete file" command
        /// </summary>
        byte[] DeleteFileCommand(string path);

        /// <summary>
        /// Generate "delete directory" command
        /// </summary>
        byte[] DeleteDirectoryCommand(string path);

        /// <summary>
        /// Generate "create directory" command
        /// </summary>
        byte[] CreateDirectoryCommand(string path);
    }
}
