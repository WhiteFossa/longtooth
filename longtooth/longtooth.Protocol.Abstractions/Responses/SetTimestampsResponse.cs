using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;

namespace longtooth.Protocol.Abstractions.Responses
{
    public class SetTimestampsResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("SetTimestampsResult")]
        public SetTimestampsResultDto SetTimestampsResult { get; set; }

        public SetTimestampsResponse(SetTimestampsResultDto setTimestampsResult) : base(CommandType.SetTimestamps)
        {
            SetTimestampsResult = setTimestampsResult ?? throw new ArgumentNullException(nameof(setTimestampsResult));
        }

        public static SetTimestampsResponse Parse(string header, byte[] payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<SetTimestampsResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new SetTimestampsRunResult(SetTimestampsResult);
        }
    }
}
