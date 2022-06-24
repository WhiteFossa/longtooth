using System;
using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class GetFileInfoRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("GetFileInfoResult")]
        public GetFileInfoResultDto GetFileInfoResult { get; private set; }

        public GetFileInfoRunResult(GetFileInfoResultDto getFileInfoResult) : base(CommandType.GetFileInfo)
        {
            GetFileInfoResult = getFileInfoResult ?? throw new ArgumentNullException(nameof(getFileInfoResult));
        }
    }
}
