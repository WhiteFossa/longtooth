using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Enums;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class ServerSideMessagesProcessor : IServerSideMessagesProcessor
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;

        public ServerSideMessagesProcessor(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
        }

        public ResponseDto ParseMessage(List<byte> message)
        {
            var headerSize = BitConverter.ToInt32(message.GetRange(0, sizeof(Int32)).ToArray(), 0);

            if ((headerSize + 2 * sizeof(Int32)) < message.Count)
            {
                throw new ArgumentException("Message is too short!", nameof(message));
            }

            var binaryHeader = message.GetRange(4, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader.ToArray());

            var header = JsonSerializer.Deserialize<CommandHeader>(stringHeader); // Initially to generic header

            ResponseDto result;
            switch (header.Command)
            {
                case CommandType.Ping:
                    result = new PingCommand(_messagesProcessor, _responseToClientHeaderGenerator).Parse(stringHeader);

                    break;

                case CommandType.Exit:
                    result = new ExitCommand(_messagesProcessor, _responseToClientHeaderGenerator).Parse(stringHeader);

                    break;

                default:
                    throw new InvalidOperationException($"Unknown command type: { header.Command }");
            }

            return result;
        }
    }
}
