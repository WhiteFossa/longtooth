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
    public class CreateFileCommand : CommandHeader
    {
        /// <summary>
        /// Create this file
        /// </summary>
        [JsonPropertyName("NewFilePath")]
        public string NewFilePath { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public CreateFileCommand(string newFilePath) : base(CommandType.CreateFile)
        {
            NewFilePath = newFilePath;
        }

        public CreateFileCommand(string newFilePath,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.CreateFile)
        {
            NewFilePath = newFilePath;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, IReadOnlyCollection<byte> payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<CreateFileCommand>(header);

            var createResult = await _filesManager.CreateNewFileAsync(parsedHeader.NewFilePath);

            var response = _responseToClientHeaderGenerator.GenerateCreateFileResponse(createResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
