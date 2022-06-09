using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class ClientSideMessagesProcessor : IClientSideMessagesProcessor
    {
        public ResponseHeader ParseMessage(List<byte> message)
        {
            var headerSize = BitConverter.ToInt32(message.GetRange(0, sizeof(Int32)).ToArray(), 0);

            if ((headerSize + 2 * sizeof(Int32)) < message.Count)
            {
                throw new ArgumentException("Response is too short!", nameof(message));
            }

            var binaryHeader = message.GetRange(4, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader.ToArray());

            var header = JsonSerializer.Deserialize<ResponseHeader>(stringHeader); // Initially to generic header

            ResponseHeader response = null;
            switch (header.Command)
            {
                case CommandType.Ping:
                    return PingResponse.Parse(stringHeader);

                case CommandType.Exit:
                    return ExitResponse.Parse(stringHeader);

                case CommandType.GetMountpoints:
                    return GetMountpointsResponse.Parse(stringHeader);

                default:
                    throw new InvalidOperationException($"Response to unknown command. Type: {header.Command}");
            }
        }
    }
}
