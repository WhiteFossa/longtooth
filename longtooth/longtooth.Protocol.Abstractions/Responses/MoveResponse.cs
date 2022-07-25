using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    public class MoveResponse : ResponseHeader
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonPropertyName("MoveResult")]
        public MoveResultDto MoveResult { get; set; }

        public MoveResponse(MoveResultDto moveResult) : base(CommandType.Move)
        {
            MoveResult = moveResult ?? throw new ArgumentNullException(nameof(moveResult));
        }

        public static MoveResponse Parse(string header, byte[] payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<MoveResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new MoveRunResult(MoveResult);
        }
    }
}
