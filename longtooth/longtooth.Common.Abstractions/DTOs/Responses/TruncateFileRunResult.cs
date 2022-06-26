using System;
using System.Text.Json.Serialization;
using longtooth.Common.Abstractions.Enums;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class TruncateFileRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("TruncateFileResult")]
        public TruncateFileResultDto TruncateFileResult { get; private set; }

        public TruncateFileRunResult(TruncateFileResultDto truncateFileResult) : base(CommandType.TruncateFile)
        {
            TruncateFileResult = truncateFileResult ?? throw new ArgumentNullException(nameof(truncateFileResult));
        }
    }
}
