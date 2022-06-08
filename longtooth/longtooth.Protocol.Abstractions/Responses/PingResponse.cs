using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Enums;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    /// <summary>
    /// Response to ping command
    /// </summary>
    public class PingResponse : ResponseHeader
    {
        public static PingResponse Parse(string header)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<PingResponse>(header);
        }

        public PingResponse() : base(CommandType.Ping)
        {

        }

        public override async Task RunAsync(ILogger logger)
        {
            await logger.LogInfoAsync("Pong!");
        }
    }
}
