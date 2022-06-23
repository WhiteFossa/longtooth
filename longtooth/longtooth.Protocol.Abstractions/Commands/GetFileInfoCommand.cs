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
    public class GetFileInfoCommand : CommandHeader
    {
        /// <summary>
        /// Get information about this file
        /// </summary>
        [JsonPropertyName("Path")]
        public string Path { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public GetFileInfoCommand(string path) : base(CommandType.GetFileInfo)
        {
            Path = path;
        }

        public GetFileInfoCommand(string path,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.GetFileInfo)
        {
            Path = path;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, IReadOnlyCollection<byte> payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<GetFileInfoCommand>(header);

            var fileInfoResult = await _filesManager.GetFileInfoAsync(parsedHeader.Path);

            var response = _responseToClientHeaderGenerator.GenerateGetFileInfoResponse(fileInfoResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
