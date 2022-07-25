using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Commands
{
    public class GetMountpointsCommand : CommandHeader
    {
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IResponseToClientHeaderGenerator _responseToClientHeaderGenerator;
        private readonly IFilesManager _filesManager;

        public GetMountpointsCommand(IMessagesProcessor messagesProcessor,
            IResponseToClientHeaderGenerator responseToClientHeaderGenerator,
            IFilesManager filesManager) : base(CommandType.GetMountpoints)
        {
            _messagesProcessor = messagesProcessor;
            _responseToClientHeaderGenerator = responseToClientHeaderGenerator;
            _filesManager = filesManager;
        }

        public async Task<ResponseDto> ParseAsync(string header, byte[] payload)
        {
            // Do work here
            var mountpoints = await _filesManager.GetMountpointsAsync();

            var response = _responseToClientHeaderGenerator.GenerateGetMountpointsResponse(mountpoints);

            var responseMessage = _messagesProcessor.PrepareMessageToSend(response);

            return new ResponseDto(true, false, responseMessage);
        }
    }
}
