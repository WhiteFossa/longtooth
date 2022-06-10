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
    }
}
