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
        /// Generates response with the list of mountpoints
        /// </summary>
        byte[] GenerateGetMountpointsCommand(List<MountpointDto> mountpoints);
    }
}
