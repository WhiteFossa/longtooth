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
    public class DeleteFileResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("DeleteFileResult")]
        public DeleteFileResultDto DeleteFileResult { get; set; }

        public DeleteFileResponse(DeleteFileResultDto deleteFileResult) : base(CommandType.DeleteFile)
        {
            DeleteFileResult = deleteFileResult;
        }

        public static DeleteFileResponse Parse(string header, List<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<DeleteFileResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new DeleteFileRunResult(DeleteFileResult);
        }
    }
}
