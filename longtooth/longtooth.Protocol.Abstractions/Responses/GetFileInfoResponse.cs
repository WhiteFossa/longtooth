using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    public class GetFileInfoResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("GetFileInfoResult")]
        public GetFileInfoResultDto GetFileInfoResult { get; set; }

        public GetFileInfoResponse(GetFileInfoResultDto getFileInfoResult) : base(CommandType.GetFileInfo)
        {
            GetFileInfoResult = getFileInfoResult;
        }

        public static GetFileInfoResponse Parse(string header, IReadOnlyCollection<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<GetFileInfoResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new GetFileInfoRunResult(GetFileInfoResult);
        }
    }
}
