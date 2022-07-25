using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class ServerSideMessagesProcessor : IServerSideMessagesProcessor
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        public ServerSideMessagesProcessor(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseMessageAsync(byte[] message)
        {
            var headerSizeBytes = new byte[sizeof(int)];
            Array.Copy(message, 0, headerSizeBytes, 0, sizeof(int));
            var headerSize = BitConverter.ToInt32(headerSizeBytes);

            if ((headerSize + 2 * sizeof(int)) > message.Length)
            {
                throw new ArgumentException("Message is too short!", nameof(message));
            }

            var binaryHeader = new byte[headerSize];
            Array.Copy(message, sizeof(int), binaryHeader, 0, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader);

            var header = JsonSerializer.Deserialize<CommandHeader>(stringHeader); // Initially to generic header

            var payloadSizeBytes = new byte[sizeof(int)];
            Array.Copy(message, sizeof(int) + headerSize, payloadSizeBytes, 0, sizeof(int));
            var payloadSize = BitConverter.ToInt32(payloadSizeBytes);

            var payload = new byte[payloadSize];
            Array.Copy(message, 2 * sizeof(int) + headerSize, payload, 0, message.Length - 2 * sizeof(int) - headerSize);

            ResponseDto result;
            switch (header.Command)
            {
                case CommandType.Ping:
                    result = new PingCommand(_messagesProcessor, _responseToClientHeaderGenerator).Parse(stringHeader, payload);

                    break;

                case CommandType.Exit:
                    result = new ExitCommand(_messagesProcessor, _responseToClientHeaderGenerator).Parse(stringHeader, payload);

                    break;

                case CommandType.GetMountpoints:
                    result = await new GetMountpointsCommand(_messagesProcessor, _responseToClientHeaderGenerator, _filesManager)
                        .ParseAsync(stringHeader, payload);

                    break;

                case CommandType.GetDirectoryContent:
                    result = await new GetDirectoryContentCommand(string.Empty,
                        _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);

                    break;

                case CommandType.DownloadFile:
                    result = await new DownloadCommand(string.Empty,
                        0,
                        0,
                        _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);

                    break;

                case CommandType.CreateFile:
                    result = await new CreateFileCommand(string.Empty,
                        _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);

                    break;

                case CommandType.UpdateFile:
                    result = await new UpdateFileCommand(String.Empty,
                        0,
                        null,
                        _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);

                    break;

                case CommandType.DeleteFile:
                    result = await new DeleteFileCommand(string.Empty,
                         _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.DeleteDirectory:
                    result = await new DeleteDirectoryCommand(string.Empty,
                         _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.CreateDirectory:
                    result = await new CreateDirectoryCommand(string.Empty,
                         _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.GetFileInfo:
                    result = await new GetFileInfoCommand(string.Empty,
                         _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.TruncateFile:
                    result = await new TruncateFileCommand(string.Empty,
                            0,
                            _messagesProcessor,
                            _responseToClientHeaderGenerator,
                            _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.SetTimestamps:
                    result = await new SetTimestampsCommand(string.Empty,
                            DateTime.UnixEpoch,
                            DateTime.UnixEpoch,
                            DateTime.UnixEpoch,
                            _messagesProcessor,
                            _responseToClientHeaderGenerator,
                            _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.Move:
                    result = await new MoveCommand(string.Empty,
                        string.Empty,
                        false,
                        _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                case CommandType.GetDiskSpace:
                    result = await new GetDiskSpaceCommand(string.Empty,
                        _messagesProcessor,
                        _responseToClientHeaderGenerator,
                        _filesManager)
                        .ParseAsync(stringHeader, payload);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown command type: { header.Command }");
            }

            return result;
        }
    }
}
