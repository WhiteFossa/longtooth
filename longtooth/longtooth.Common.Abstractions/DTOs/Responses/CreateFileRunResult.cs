using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class CreateFileRunResult : ResponseRunResult
    {
        /// <summary>
        /// Is successful?
        /// </summary>
        [JsonPropertyName("CreateFileResult")]
        public CreateFileResultDto CreateFileResult { get; set; }

        public CreateFileRunResult(CreateFileResultDto createFileResult) : base(CommandType.CreateFile)
        {
            CreateFileResult = createFileResult;
        }
    }
}
