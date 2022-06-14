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
    /// Response to graceful exit command
    /// </summary>
    public class ExitResponse : ResponseHeader
    {
        public static ExitResponse Parse(string header, List<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<ExitResponse>(header);
        }

        public ExitResponse() : base(CommandType.Exit)
        {

        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new ResponseRunResult(Command);
        }
    }
}
