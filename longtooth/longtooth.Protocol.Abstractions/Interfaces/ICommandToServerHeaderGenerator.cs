﻿namespace longtooth.Protocol.Abstractions.Interfaces
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
        /// <returns></returns>
        byte[] GenerateExitCommand();
    }
}
