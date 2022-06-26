using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;

namespace longtooth.Protocol.Abstractions.Commands
{
    public class TruncateFileCommand : CommandHeader
    {
        /// <summary>
        /// Truncate this file
        /// </summary>
        [JsonPropertyName("Path")]
        public string Path { get; private set; }

        /// <summary>
        /// New file size
        /// </summary>
        [JsonPropertyName("NewSize")]
        public ulong NewSize { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public TruncateFileCommand(string path, ulong newSize) : base(CommandType.TruncateFile)
        {
            Path = path;
            NewSize = newSize;
        }

        public TruncateFileCommand(string path,
            ulong newSize,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.GetFileInfo)
        {
            Path = path;
            NewSize = newSize;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, IReadOnlyCollection<byte> payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<TruncateFileCommand>(header);

            var truncateFileResult = await _filesManager.TruncateFileAsync(parsedHeader.Path, parsedHeader.NewSize);

            var response = _responseToClientHeaderGenerator.GenerateTruncateFileResponse(truncateFileResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
