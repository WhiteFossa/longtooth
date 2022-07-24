using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Commands
{
    public class SetTimestampsCommand : CommandHeader
    {
        /// <summary>
        /// Set timestamps for this file / directory
        /// </summary>
        [JsonPropertyName("Path")]
        public string Path { get; private set; }

        /// <summary>
        /// Access time
        /// </summary>
        [JsonPropertyName("Atime")]
        public DateTime Atime { get; private set; }

        /// <summary>
        /// Metadata modification time
        /// </summary>
        [JsonPropertyName("Ctime")]
        public DateTime Ctime { get; private set; }

        /// <summary>
        /// Modification time
        /// </summary>
        [JsonPropertyName("Mtime")]
        public DateTime Mtime { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public SetTimestampsCommand(string path,
            DateTime atime,
            DateTime ctime,
            DateTime mtime) : base(CommandType.SetTimestamps)
        {
            Path = path;
            Atime = atime;
            Ctime = ctime;
            Mtime = mtime;
        }

        public SetTimestampsCommand(string path,
            DateTime atime,
            DateTime ctime,
            DateTime mtime,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.SetTimestamps)
        {
            Path = path;
            Atime = atime;
            Ctime = ctime;
            Mtime = mtime;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, byte[] payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<SetTimestampsCommand>(header);

            var setTimestampsResult = await _filesManager.SetTimestampsAsync(parsedHeader.Path,
                parsedHeader.Atime,
                parsedHeader.Ctime,
                parsedHeader.Mtime);

            var response = _responseToClientHeaderGenerator.GenerateSetTimestampsResponse(setTimestampsResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
