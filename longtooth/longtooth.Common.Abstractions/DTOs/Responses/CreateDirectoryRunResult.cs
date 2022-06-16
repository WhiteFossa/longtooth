using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class CreateDirectoryRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("CreateDirectoryResult")]
        public CreateDirectoryResultDto CreateDirectoryResult { get; private set; }

        public CreateDirectoryRunResult(CreateDirectoryResultDto createDirectoryResult) : base(CommandType.CreateDirectory)
        {
            CreateDirectoryResult = createDirectoryResult;
        }
    }
}
