using System;

namespace longtooth.Common.Abstractions.DTOs
{
    /// <summary>
    /// One server filesystem mountpoint
    /// </summary>
    public class MountpointDto
    {
        /// <summary>
        /// Mountpoint name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Path to mountpoint on server
        /// </summary>
        public string ServerSidePath { get; private set; }

        public MountpointDto(string name, string serverSidePath)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ServerSidePath = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));
        }
    }
}
