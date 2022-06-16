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

        public async Task<ResponseDto> ParseMessageAsync(List<byte> message)
        {
            var headerSize = BitConverter.ToInt32(message.GetRange(0, sizeof(Int32)).ToArray(), 0);

            if ((headerSize + 2 * sizeof(Int32)) > message.Count)
            {
                throw new ArgumentException("Message is too short!", nameof(message));
            }

            var binaryHeader = message.GetRange(4, headerSize);
            var stringHeader = Encoding.UTF8.GetString(binaryHeader.ToArray());

            var header = JsonSerializer.Deserialize<CommandHeader>(stringHeader); // Initially to generic header

            var payloadSize = BitConverter.ToInt32(message.GetRange(4 + headerSize, 4).ToArray());
            var payload = message.GetRange(8 + headerSize, message.Count - 8 - headerSize);

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

                default:
                    throw new InvalidOperationException($"Unknown command type: { header.Command }");
            }

            return result;
        }
    }
}
