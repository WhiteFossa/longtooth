using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Commands
{
    public class MoveCommand : CommandHeader
    {
        /// <summary>
        /// Move from this path
        /// </summary>
        [JsonPropertyName("From")]
        public string From { get; private set; }

        /// <summary>
        /// Move to this path
        /// </summary>
        [JsonPropertyName("To")]
        public string To { get; private set; }

        /// <summary>
        /// If true, then overwrite destination if exist
        /// </summary>
        [JsonPropertyName("IsOverwrite")]
        public bool IsOverwrite { get; private set; }

        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        [JsonConstructor]
        public MoveCommand(string from,
            string to,
            bool isOverwrite) : base(CommandType.Move)
        {
            From = from;
            To = to;
            IsOverwrite = isOverwrite;
        }

        public MoveCommand(string from,
            string to,
            bool isOverwrite,
            IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.Move)
        {
            From = from;
            To = to;
            IsOverwrite = isOverwrite;

            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, byte[] payload)
        {
            // Do work here
            var parsedHeader = JsonSerializer.Deserialize<MoveCommand>(header);

            var moveResult = await _filesManager.MoveAsync(parsedHeader.From, parsedHeader.To, parsedHeader.IsOverwrite);

            var response = _responseToClientHeaderGenerator.GenerateMoveResponse(moveResult);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
