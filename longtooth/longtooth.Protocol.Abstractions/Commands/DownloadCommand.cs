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
    /// <summary>
    /// Command to download part of file
    /// </summary>
    public class DownloadCommand : CommandHeader
    {
        /// <summary>
        /// File to download
        /// </summary>
        [JsonPropertyName("FilePath")]
        public string FilePath { get; private set; }

        /// <summary>
        /// Position in file to start reading
        /// </summary>
        [JsonPropertyName("StartPosition")]
        public ulong StartPosition { get; private set; }

        /// <summary>
        /// Read this amount of bytes
        /// </summary>
        [JsonPropertyName("ReadLength")]
        public uint ReadLength { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public DownloadCommand(string filePath, ulong startPosition, uint readLength) : base(CommandType.DownloadFile)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            StartPosition = startPosition;
            ReadLength = readLength;
        }

        public DownloadCommand(string filePath,
            ulong startPosition,
            uint readLength,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.DownloadFile)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath)); ;
            StartPosition = startPosition;
            ReadLength = readLength;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, IReadOnlyCollection<byte> payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<DownloadCommand>(header);

            var fileData = await _filesManager.DownloadFileAsync(parsedHeader.FilePath, parsedHeader.StartPosition, parsedHeader.ReadLength);

            var response = _responseToClientHeaderGenerator.GenerateDownloadFileResponse(fileData);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }

    }
}
