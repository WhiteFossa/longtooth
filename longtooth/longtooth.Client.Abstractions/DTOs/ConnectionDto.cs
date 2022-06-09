using System;
using System.Net;

namespace longtooth.Client.Abstractions.DTOs
{
    /// <summary>
    /// Connection parameters
    /// </summary>
    public class ConnectionDto
    {
        /// <summary>
        /// Server address
        /// </summary>
        public IPAddress Address { get; private set; }

        /// <summary>
        /// Server port
        /// </summary>
        public uint Port { get; private set; }

        public ConnectionDto(IPAddress address, uint port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
        }
    }
}
