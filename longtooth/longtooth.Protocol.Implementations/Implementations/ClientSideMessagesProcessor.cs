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
        public ResponseHeader ParseMessage(IReadOnlyCollection<byte> message)
        {
            var messageAsList = new List<byte>(message); // Could we do it in a more efficient way?

            var headerSize = BitConverter.ToInt32(messageAsList.GetRange(0, sizeof(Int32)).ToArray(), 0);

            if ((headerSize + 2 * sizeof(Int32)) > messageAsList.Count)
            {
                throw new ArgumentException("Response is too short!", nameof(message));
            }

            var binaryHeader = messageAsList.GetRange(4, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader.ToArray());

            var header = JsonSerializer.Deserialize<ResponseHeader>(stringHeader); // Initially to generic header

            var payloadSize = BitConverter.ToInt32(messageAsList.GetRange(4 + headerSize, 4).ToArray());
            var payload = messageAsList.GetRange(8 + headerSize, message.Count - 8 - headerSize).ToArray(); // TODO: Fixme, I'm slow

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

                case CommandType.UpdateFile:
                    return UpdateFileResponse.Parse(stringHeader, payload);

                case CommandType.DeleteFile:
                    return DeleteFileResponse.Parse(stringHeader, payload);

                case CommandType.DeleteDirectory:
                    return DeleteDirectoryResponse.Parse(stringHeader, payload);

                case CommandType.CreateDirectory:
                    return CreateDirectoryResponse.Parse(stringHeader, payload);

                case CommandType.GetFileInfo:
                    return GetFileInfoResponse.Parse(stringHeader, payload);

                case CommandType.TruncateFile:
                    return TruncateFileResponse.Parse(stringHeader, payload);

                case CommandType.SetTimestamps:
                    return SetTimestampsResponse.Parse(stringHeader, payload);

                case CommandType.Move:
                    return MoveResponse.Parse(stringHeader, payload);

                case CommandType.GetDiskSpace:
                    return GetDiskSpaceResponse.Parse(stringHeader, payload);

                default:
                    throw new InvalidOperationException($"Response to unknown command. Type: {header.Command}");
            }
        }
    }
}
