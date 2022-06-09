using longtooth.Common.Abstractions.Enums;
using System;
using System.Collections.Generic;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    /// <summary>
    /// Response to Get Mountpoints command
    /// </summary>
    public class GetMountpointsRunResult : ResponseRunResult
    {
        public List<MountpointDto> Mountpoints { get; private set; }

        public GetMountpointsRunResult(CommandType command, List<MountpointDto> mountpoints) : base(command)
        {
            Mountpoints = mountpoints ?? throw new ArgumentNullException(nameof(mountpoints));
        }
    }
}
