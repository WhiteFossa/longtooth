using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class DeleteDirectoryRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("DeleteDirectoryResult")]
        public DeleteDirectoryResultDto DeleteDirectoryResult { get; private set; }

        public DeleteDirectoryRunResult(DeleteDirectoryResultDto deleteDirectoryResult) : base(CommandType.DeleteDirectory)
        {
            DeleteDirectoryResult = deleteDirectoryResult;
        }
    }
}
