using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Commands
{
    /// <summary>
    /// Command, causing server to disconnect a client gracefully
    /// </summary>
    public class ExitCommand : CommandHeader
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;

        public ExitCommand(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator) : base(CommandType.Exit)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
        }

        public ResponseDto Parse(string header, List<byte> payload)
        {
            // Do work here
            var response = _responseToClientHeaderGenerator.GenerateExitResponse();

            var responseMessage = _messagesProcessor.PrepareMessageToSend(new List<byte>(response));

            return new ResponseDto(true, true, responseMessage);
        }
    }
}
