using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.FilesManager.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Commands
{
    public class GetDirectoryContentCommand : CommandHeader
    {
        /// <summary>
        /// What directory do we want to get?
        /// </summary>
        [JsonPropertyName("ServerSidePath")]
        public string ServerSidePath { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public GetDirectoryContentCommand(string serverSidePath) : base(CommandType.GetDirectoryContent)
        {
            ServerSidePath = serverSidePath ?? throw new ArgumentNullException(nameof(serverSidePath));
        }

        public GetDirectoryContentCommand(string serverSidePath,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.GetDirectoryContent)
        {
            ServerSidePath = serverSidePath;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<GetDirectoryContentCommand>(header);

            var directoryContent = await _filesManager.GetDirectoryContentAsync(parsedHeader.ServerSidePath);

            var response = _responseToClientHeaderGenerator.GenerateGetDirectoryContentResponse(directoryContent);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(new List<byte>(response));

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
