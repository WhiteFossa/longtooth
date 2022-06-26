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
    public class TruncateFileResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("TruncateFileResult")]
        public TruncateFileResultDto TruncateFileResult { get; set; }

        public TruncateFileResponse(TruncateFileResultDto truncateFileResult) : base(CommandType.TruncateFile)
        {
            TruncateFileResult = truncateFileResult ?? throw new ArgumentNullException(nameof(truncateFileResult));
        }

        public static TruncateFileResponse Parse(string header, IReadOnlyCollection<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<TruncateFileResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new TruncateFileRunResult(TruncateFileResult);
        }
    }
}
