﻿using longtooth.Common.Abstractions.Enums;
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
    public class DeleteFileCommand : CommandHeader
    {
        /// <summary>
        /// Delete this file
        /// </summary>
        [JsonPropertyName("Path")]
        public string Path { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;


        [JsonConstructor]
        public DeleteFileCommand(string path) : base(CommandType.DeleteFile)
        {
            Path = path;
        }

        public DeleteFileCommand(string path,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.DeleteFile)
        {
            Path = path;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, byte[] payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<DeleteFileCommand>(header);

            var deleteResult = await _filesManager.DeleteFileAsync(parsedHeader.Path);

            var response = _responseToClientHeaderGenerator.GenerateDeleteFileResponse(deleteResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
