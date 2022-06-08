﻿namespace longtooth.Protocol.Abstractions.Enums
{
    /// <summary>
    /// Possible commands from client to server
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Gracefully close connection
        /// </summary>
        Close = 0,

        /// <summary>
        /// Check connection
        /// </summary>
        Ping = 1
    }
}