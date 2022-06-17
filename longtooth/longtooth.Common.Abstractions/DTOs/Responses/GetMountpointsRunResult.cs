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
        public IReadOnlyCollection<MountpointDto> Mountpoints { get; private set; }

        public GetMountpointsRunResult(IReadOnlyCollection<MountpointDto> mountpoints) : base(CommandType.GetMountpoints)
        {
            Mountpoints = mountpoints ?? throw new ArgumentNullException(nameof(mountpoints));
        }
    }
}
