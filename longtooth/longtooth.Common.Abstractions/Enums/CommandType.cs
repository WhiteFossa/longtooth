namespace longtooth.Common.Abstractions.Enums
{
    /// <summary>
    /// Possible commands from client to server
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Gracefully close connection
        /// </summary>
        Exit = 0,

        /// <summary>
        /// Check connection
        /// </summary>
        Ping = 1,

        /// <summary>
        /// Ask phone for mountpoints
        /// </summary>
        GetMountpoints = 2,

        /// <summary>
        /// Get server directory content
        /// </summary>
        GetDirectoryContent = 3,

        /// <summary>
        /// Download part of file
        /// </summary>
        DownloadFile = 4,

        /// <summary>
        /// Create new file
        /// </summary>
        CreateFile = 5,

        /// <summary>
        /// Update file content
        /// </summary>
        UpdateFile = 6,

        /// <summary>
        /// Delete file
        /// </summary>
        DeleteFile = 7,

        /// <summary>
        /// Delete directory
        /// </summary>
        DeleteDirectory = 8,
    }
}
