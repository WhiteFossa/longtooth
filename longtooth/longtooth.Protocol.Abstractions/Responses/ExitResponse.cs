using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Enums;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    /// <summary>
    /// Response to graceful exit command
    /// </summary>
    public class ExitResponse : ResponseHeader
    {
        public static ExitResponse Parse(string header)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<ExitResponse>(header);
        }

        public ExitResponse() : base(CommandType.Exit)
        {

        }

        public async override Task RunAsync(ILogger logger, IClient client)
        {
            await logger.LogInfoAsync("Exited");

            await client.DisconnectGracefullyAsync();
        }
    }
}
