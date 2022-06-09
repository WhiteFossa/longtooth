using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.FilesManager.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class CommandToServerHeaderGenerator : ICommandToServerHeaderGenerator
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        public CommandToServerHeaderGenerator(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        private byte[] EncodeCommand(object command)
        {
            var header = JsonSerializer.Serialize(command);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, null);

            return message.ToDataPacket();
        }

        public byte[] GeneratePingCommand()
        {
            var pingCommand = new PingCommand(_messagesProcessor, _responseToClientHeaderGenerator);

            return EncodeCommand(pingCommand);
        }

        public byte[] GenerateExitCommand()
        {
            var exitCommand = new ExitCommand(_messagesProcessor, _responseToClientHeaderGenerator);

            return EncodeCommand(exitCommand);
        }

        public byte[] GenerateGetMountpointsCommand(List<MountpointDto> mountpoints)
        {
            var getMountpointsCommand = new GetMountpointsCommand(_messagesProcessor, _responseToClientHeaderGenerator, _filesManager);

            return EncodeCommand(getMountpointsCommand);
        }
    }
}
