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
    public class CreateFileResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("CreateFileResult")]
        public CreateFileResultDto CreateFileResult { get; set; }

        public CreateFileResponse(CreateFileResultDto createFileResult) : base(CommandType.CreateFile)
        {
            CreateFileResult = createFileResult;
        }

        public static CreateFileResponse Parse(string header, byte[] payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<CreateFileResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new CreateFileRunResult(CreateFileResult);
        }
    }
}
