using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Commands
{
    /// <summary>
    /// Command to ping server
    /// </summary>
    public class PingCommand : CommandHeader
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;

        public PingCommand(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator) : base(CommandType.Ping)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
        }

        public ResponseDto Parse(string header, IReadOnlyCollection<byte> payload)
        {
            // Do work here
            var response = _responseToClientHeaderGenerator.GeneratePingResponse();

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
