using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class CommandToServerHeaderGenerator : ICommandToServerHeaderGenerator
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;

        public CommandToServerHeaderGenerator(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
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
    }
}
