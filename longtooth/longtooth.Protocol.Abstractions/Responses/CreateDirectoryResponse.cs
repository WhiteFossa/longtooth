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
    public class CreateDirectoryResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("CreateDirectoryResult")]
        public CreateDirectoryResultDto CreateDirectoryResult { get; set; }

        public CreateDirectoryResponse(CreateDirectoryResultDto createDirectoryResult) : base(CommandType.CreateDirectory)
        {
            CreateDirectoryResult = createDirectoryResult;
        }

        public static CreateDirectoryResponse Parse(string header, IReadOnlyCollection<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<CreateDirectoryResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new CreateDirectoryRunResult(CreateDirectoryResult);
        }
    }
}
