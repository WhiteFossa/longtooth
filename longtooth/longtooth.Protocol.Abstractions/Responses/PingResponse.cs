using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    /// <summary>
    /// Response to ping command
    /// </summary>
    public class PingResponse : ResponseHeader
    {
        public static PingResponse Parse(string header, byte[] payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<PingResponse>(header);
        }

        public PingResponse() : base(CommandType.Ping)
        {

        }

        public override async Task<ResponseRunResult> RunAsync()
        {
            return new ResponseRunResult(Command);
        }
    }
}
