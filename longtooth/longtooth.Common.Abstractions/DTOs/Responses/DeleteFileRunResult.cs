using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class DeleteFileRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("DeleteFileResult")]
        public DeleteFileResultDto DeleteFileResult { get; private set; }

        public DeleteFileRunResult(DeleteFileResultDto deleteFileResult) : base(CommandType.DeleteFile)
        {
            DeleteFileResult = deleteFileResult;
        }
    }
}
