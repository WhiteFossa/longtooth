using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    public class GetMountpointsResponse : ResponseHeader
    {
        /// <summary>
        /// List of mountpoints
        /// </summary>
        [JsonPropertyName("Mountpoints")]
        public List<MountpointDto> Mountpoints { get; set; }

        public GetMountpointsResponse(List<MountpointDto> mountpoints) : base(CommandType.GetMountpoints)
        {
            Mountpoints = mountpoints ?? throw new ArgumentNullException(nameof(mountpoints));
        }

        public static GetMountpointsResponse Parse(string header)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<GetMountpointsResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new GetMountpointsRunResult(Command, Mountpoints);
        }
    }
}
