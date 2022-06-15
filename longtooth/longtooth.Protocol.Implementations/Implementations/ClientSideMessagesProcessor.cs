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

            if ((headerSize + 2 * sizeof(Int32)) > message.Count)
            {
                throw new ArgumentException("Response is too short!", nameof(message));
            }

            var binaryHeader = message.GetRange(4, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader.ToArray());

            var header = JsonSerializer.Deserialize<ResponseHeader>(stringHeader); // Initially to generic header

            var payloadSize = BitConverter.ToInt32(message.GetRange(4 + headerSize, 4).ToArray());
            var payload = message.GetRange(8 + headerSize, message.Count - 8 - headerSize);

            ResponseHeader response = null;
            switch (header.Command)
            {
                case CommandType.Ping:
                    return PingResponse.Parse(stringHeader, payload);

                case CommandType.Exit:
                    return ExitResponse.Parse(stringHeader, payload);

                case CommandType.GetMountpoints:
                    return GetMountpointsResponse.Parse(stringHeader, payload);

                case CommandType.GetDirectoryContent:
                    return GetDirectoryContentResponse.Parse(stringHeader, payload);

                case CommandType.DownloadFile:
                    return DownloadFileResponse.Parse(stringHeader, payload);

                case CommandType.CreateFile:
                    return CreateFileResponse.Parse(stringHeader, payload);

                default:
                    throw new InvalidOperationException($"Response to unknown command. Type: {header.Command}");
            }
        }
    }
}
