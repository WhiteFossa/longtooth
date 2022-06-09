using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.Responses;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class ResponseToClientHeaderGenerator : IResponseToClientHeaderGenerator
    {
        private readonly IMessagesProcessor _messagesProcessor;

        public ResponseToClientHeaderGenerator(IMessagesProcessor messagesProcessor)
        {
            _messagesProcessor = messagesProcessor;
        }

        private byte[] EncodeResponse(object response, byte[] binaryData)
        {
            var header = JsonSerializer.Serialize(response);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, binaryData);

            return message.ToDataPacket();
        }

        public byte[] GeneratePingResponse()
        {
            var pingResponse = new PingResponse();

            return EncodeResponse(pingResponse, null);
        }

        public byte[] GenerateExitResponse()
        {
            var exitResponse = new ExitResponse();

            return EncodeResponse(exitResponse, null);
        }
    }
}
