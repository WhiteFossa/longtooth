﻿using longtooth.Common.Abstractions.DTOs;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to generate responses to client
    /// </summary>
    public interface IResponseToClientHeaderGenerator
    {
        /// <summary>
        /// Generates response to ping command
        /// </summary>
        byte[] GeneratePingResponse();

        /// <summary>
        /// Generates response to exit command
        /// </summary>
        /// <returns></returns>
        byte[] GenerateExitResponse();

        /// <summary>
        /// Generates response with the list of server's mountpoints
        /// </summary>
        byte[] GenerateGetMountpointsResponse(List<MountpointDto> mountpoints);
    }
}
