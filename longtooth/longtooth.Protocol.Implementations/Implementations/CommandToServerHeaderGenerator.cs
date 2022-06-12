using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class CommandToServerHeaderGenerator : ICommandToServerHeaderGenerator
    {
        private byte[] EncodeCommand(object command)
        {
            var header = JsonSerializer.Serialize(command);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, null);

            return message.ToDataPacket();
        }

        public byte[] GeneratePingCommand()
        {
            var pingCommand = new PingCommand(null, null);

            return EncodeCommand(pingCommand);
        }

        public byte[] GenerateExitCommand()
        {
            var exitCommand = new ExitCommand(null, null);

            return EncodeCommand(exitCommand);
        }

        public byte[] GenerateGetMountpointsCommand()
        {
            var getMountpointsCommand = new GetMountpointsCommand(null, null, null);

            return EncodeCommand(getMountpointsCommand);
        }

        public byte[] GenerateGetDirectoryContentCommand(string serverSidePath)
        {
            var getDirectoryContentCommand = new GetDirectoryContentCommand(serverSidePath,
                null,
                null,
                null);

            return EncodeCommand(getDirectoryContentCommand);
        }
    }
}
