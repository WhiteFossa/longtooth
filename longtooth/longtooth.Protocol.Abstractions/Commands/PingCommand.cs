using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Enums;

namespace longtooth.Protocol.Abstractions.Commands
{
    /// <summary>
    /// Command to ping server
    /// </summary>
    public class PingCommand : CommandHeader
    {
        public PingCommand() : base(CommandType.Ping)
        {
        }
    }
}
