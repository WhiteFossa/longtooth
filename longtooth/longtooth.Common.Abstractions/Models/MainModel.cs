using longtooth.Common.Abstractions.DTOs;
using System.Collections.Generic;
using System.Net;

namespace longtooth.Common.Abstractions.Models
{
    /// <summary>
    /// Main model
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// Server IP
        /// </summary>
        public IPAddress ServerIp { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        public uint ServerPort { get; set; }

        /// <summary>
        /// Share phone's mountpoints
        /// </summary>
        public List<MountpointDto> Mountpoints { get; set; }
    }
}
