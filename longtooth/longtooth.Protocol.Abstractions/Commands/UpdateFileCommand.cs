using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Commands
{
    public class UpdateFileCommand : CommandHeader
    {
        /// <summary>
        /// File to update
        /// </summary>
        [JsonPropertyName("FilePath")]
        public string FilePath { get; private set; }

        /// <summary>
        /// Position in file to start where update will start
        /// </summary>
        [JsonPropertyName("StartPosition")]
        public long StartPosition { get; private set; }

        /// <summary>
        /// Update content
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<byte> Content { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public UpdateFileCommand(string filePath, long startPosition) : base(CommandType.UpdateFile)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            StartPosition = startPosition;
            Content = null;
        }

        public UpdateFileCommand(string filePath,
            long startPosition,
            IReadOnlyCollection<byte> content,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.UpdateFile)
        {
            FilePath = filePath;
            StartPosition = startPosition;
            Content = content;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, IReadOnlyCollection<byte> payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<UpdateFileCommand>(header);

            var updateResult = await _filesManager.UpdateFileAsync(parsedHeader.FilePath, parsedHeader.StartPosition, payload);

            var response = _responseToClientHeaderGenerator.GenerateUpdateFileResponse(updateResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
