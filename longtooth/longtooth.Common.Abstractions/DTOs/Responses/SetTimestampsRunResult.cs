using System;
using System.Text.Json.Serialization;
using longtooth.Common.Abstractions.Enums;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class SetTimestampsRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("SetTimestampsResult")]
        public SetTimestampsResultDto SetTimestampsResult { get; private set; }

        public SetTimestampsRunResult(SetTimestampsResultDto setTimestampsResult) : base(CommandType.SetTimestamps)
        {
            SetTimestampsResult = setTimestampsResult ?? throw new ArgumentNullException(nameof(setTimestampsResult));
        }
    }
}
