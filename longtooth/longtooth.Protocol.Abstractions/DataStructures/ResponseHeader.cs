using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Protocol.Abstractions.Enums;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.DataStructures
{
    /// <summary>
    /// Common header for all responses from server
    /// </summary>
    public class ResponseHeader
    {
        /// <summary>
        /// Response to this command
        /// </summary>
        [JsonPropertyName("Command")]
        public CommandType Command { get; set; }

        public ResponseHeader(CommandType command)
        {
            Command = command;
        }

        /// <summary>
        /// Implement me in children
        /// </summary>
        public virtual async Task RunAsync(ILogger logger, IClient client)
        {

        }
    }
}
