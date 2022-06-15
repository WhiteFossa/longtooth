using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class CommandToServerHeaderGenerator : ICommandToServerHeaderGenerator
    {
        private byte[] EncodeCommand(object command, byte[] data)
        {
            var header = JsonSerializer.Serialize(command);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, data);

            return message.ToDataPacket();
        }

        public byte[] GeneratePingCommand()
        {
            var pingCommand = new PingCommand(null, null);

            return EncodeCommand(pingCommand, null);
        }

        public byte[] GenerateExitCommand()
        {
            var exitCommand = new ExitCommand(null, null);

            return EncodeCommand(exitCommand, null);
        }

        public byte[] GenerateGetMountpointsCommand()
        {
            var getMountpointsCommand = new GetMountpointsCommand(null, null, null);

            return EncodeCommand(getMountpointsCommand, null);
        }

        public byte[] GenerateGetDirectoryContentCommand(string serverSidePath)
        {
            var getDirectoryContentCommand = new GetDirectoryContentCommand(serverSidePath,
                null,
                null,
                null);

            return EncodeCommand(getDirectoryContentCommand, null);
        }

        public byte[] GenerateDownloadCommand(string path, ulong startPosition, uint length)
        {
            var downloadFileCommand = new DownloadCommand(path,
                startPosition,
                length,
                null,
                null,
                null);

            return EncodeCommand(downloadFileCommand, null);
        }

        public byte[] CreateFileCommand(string path)
        {
            var createFileCommand = new CreateFileCommand(path,
                null,
                null,
                null);

            return EncodeCommand(createFileCommand, null);
        }

        public byte[] UpdateFileCommand(string path, ulong startPosition, byte[] dataToWrite)
        {
            var updateFileCommand = new UpdateFileCommand(path,
                startPosition,
                dataToWrite.ToList(),
                null,
                null,
                null);

            return EncodeCommand(updateFileCommand, updateFileCommand.Content.ToArray());
        }
    }
}
