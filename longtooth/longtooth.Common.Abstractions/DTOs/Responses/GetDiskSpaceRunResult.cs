using longtooth.Common.Abstractions.Enums;
using System;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class GetDiskSpaceRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("GetDiskSpaceResult")]
        public GetDiskSpaceResultDto GetDiskSpaceResult { get; private set; }

        public GetDiskSpaceRunResult(GetDiskSpaceResultDto getDiskSpaceResult) : base(CommandType.GetDiskSpace)
        {
            GetDiskSpaceResult = getDiskSpaceResult ?? throw new ArgumentNullException(nameof(getDiskSpaceResult));
        }
    }
}
