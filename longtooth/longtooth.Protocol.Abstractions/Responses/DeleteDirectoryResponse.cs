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
    public class DeleteDirectoryResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("DeleteDirectoryResult")]
        public DeleteDirectoryResultDto DeleteDirectoryResult { get; set; }

        public DeleteDirectoryResponse(DeleteDirectoryResultDto deleteDirectoryResult) : base(CommandType.DeleteDirectory)
        {
            DeleteDirectoryResult = deleteDirectoryResult;
        }

        public static DeleteDirectoryResponse Parse(string header, List<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<DeleteDirectoryResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new DeleteDirectoryRunResult(DeleteDirectoryResult);
        }
    }
}
