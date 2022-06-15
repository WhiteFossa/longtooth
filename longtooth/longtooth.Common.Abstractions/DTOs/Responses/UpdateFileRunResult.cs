using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class UpdateFileRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("UpdateFileResult")]
        public UpdateFileResultDto UpdateFileResult { get; private set; }

        public UpdateFileRunResult(UpdateFileResultDto updateFileResult) : base(CommandType.UpdateFile)
        {
            UpdateFileResult = updateFileResult;
        }
    }
}
