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
        public ResponseHeader ParseMessage(byte[] message)
        {
            var headerSizeBytes = new byte[sizeof(int)];
            Array.Copy(message, 0, headerSizeBytes, 0, sizeof(int));
            var headerSize = BitConverter.ToInt32(headerSizeBytes);

            if ((headerSize + 2 * sizeof(int)) > message.Length)
            {
                throw new ArgumentException("Response is too short!", nameof(message));
            }

            var binaryHeader = new byte[headerSize];
            Array.Copy(message, sizeof(int), binaryHeader, 0, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader);

            var header = JsonSerializer.Deserialize<ResponseHeader>(stringHeader); // Initially to generic header

            var payloadSizeBytes = new byte[sizeof(int)];
            Array.Copy(message, headerSize + sizeof(int), payloadSizeBytes, 0, sizeof(int));
            var payloadSize = BitConverter.ToInt32(payloadSizeBytes);

            var payload = new byte[payloadSize];
            Array.Copy(message, 2 * sizeof(int) + headerSize, payload, 0, payloadSize);

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
