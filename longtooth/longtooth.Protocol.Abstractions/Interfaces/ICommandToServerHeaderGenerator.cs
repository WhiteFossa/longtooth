using longtooth.Common.Abstractions.DTOs;
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
    }
}
