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
    public class GetDiskSpaceResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("GetDiskSpaceResult")]
        public GetDiskSpaceResultDto GetDiskSpaceResult { get; set; }

        public GetDiskSpaceResponse(GetDiskSpaceResultDto getDiskSpaceResult) : base(CommandType.GetDiskSpace)
        {
            GetDiskSpaceResult = getDiskSpaceResult ?? throw new ArgumentNullException(nameof(getDiskSpaceResult));
        }

        public static GetDiskSpaceResponse Parse(string header, byte[] payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<GetDiskSpaceResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new GetDiskSpaceRunResult(GetDiskSpaceResult);
        }
    }
}
