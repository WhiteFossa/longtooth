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

        public byte[] GeneratePingResponse()
        {
            var pingResponse = new PingResponse();

            var header = JsonSerializer.Serialize(pingResponse);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, null);

            return message.ToDataPacket();
        }
    }
}
