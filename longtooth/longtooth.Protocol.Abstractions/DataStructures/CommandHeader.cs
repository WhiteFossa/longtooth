using longtooth.Common.Abstractions.Enums;
using System.Text.Json.Serialization;

namespace longtooth.Protocol.Abstractions.DataStructures
{
    /// <summary>
    /// Command header. This commands are sent to server
    /// </summary>
    public class CommandHeader
    {
        /// <summary>
        /// Command from client to server
        /// </summary>
        [JsonPropertyName("Command")]
        public CommandType Command { get; private set; }

        public CommandHeader(CommandType command)
        {
            Command = command;
        }
    }
}
