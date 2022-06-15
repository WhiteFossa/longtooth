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
    public class UpdateFileResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("UpdateFileResult")]
        public UpdateFileResultDto UpdateFileResult { get; set; }

        public UpdateFileResponse(UpdateFileResultDto updateFileResult) : base(CommandType.UpdateFile)
        {
            UpdateFileResult = updateFileResult;
        }

        public static UpdateFileResponse Parse(string header, List<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<UpdateFileResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new UpdateFileRunResult(UpdateFileResult);
        }
    }
}
