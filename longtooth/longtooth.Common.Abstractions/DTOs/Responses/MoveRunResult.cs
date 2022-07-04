using longtooth.Common.Abstractions.Enums;
using System;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class MoveRunResult : ResponseRunResult
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("MoveResult")]
        public MoveResultDto MoveResult { get; private set; }

        public MoveRunResult(MoveResultDto moveResult) : base(CommandType.Move)
        {
            MoveResult = moveResult ?? throw new ArgumentNullException(nameof(moveResult));
        }
    }
}
