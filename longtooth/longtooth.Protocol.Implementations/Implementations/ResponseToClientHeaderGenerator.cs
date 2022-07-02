using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.Responses;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using longtooth.Common.Abstractions.DTOs.Responses;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class ResponseToClientHeaderGenerator : IResponseToClientHeaderGenerator
    {
        private readonly IMessagesProcessor _messagesProcessor;

        public ResponseToClientHeaderGenerator(IMessagesProcessor messagesProcessor)
        {
            _messagesProcessor = messagesProcessor;
        }

        private IReadOnlyCollection<byte> EncodeResponse(object response, IReadOnlyCollection<byte> binaryData)
        {
            var header = JsonSerializer.Serialize(response);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, binaryData);

            return message.ToDataPacket();
        }

        public IReadOnlyCollection<byte> GeneratePingResponse()
        {
            var pingResponse = new PingResponse();

            return EncodeResponse(pingResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateExitResponse()
        {
            var exitResponse = new ExitResponse();

            return EncodeResponse(exitResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateGetMountpointsResponse(IReadOnlyCollection<MountpointDto> mountpoints)
        {
            var getMountpointsResponse = new GetMountpointsResponse(mountpoints);

            return EncodeResponse(getMountpointsResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateGetDirectoryContentResponse(DirectoryContentDto directoryContent)
        {
            var getDirectoryContentResponse = new GetDirectoryContentResponse(directoryContent);

            return EncodeResponse(getDirectoryContentResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateDownloadFileResponse(DownloadedFileWithContentDto file)
        {
            var metadata = new DownloadedFileDto(file.IsSuccessful, file.StartPosition, file.Length);

            var downloadFileResponse = new DownloadFileResponse(metadata, file.Content);

            return EncodeResponse(downloadFileResponse, downloadFileResponse.FileContent);
        }

        public IReadOnlyCollection<byte> GenerateCreateFileResponse(CreateFileResultDto fileCreationResult)
        {
            var createFileResponse = new CreateFileResponse(fileCreationResult);

            return EncodeResponse(createFileResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateUpdateFileResponse(UpdateFileResultDto updateFileResult)
        {
            var updateFileResponse = new UpdateFileResponse(updateFileResult);

            return EncodeResponse(updateFileResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateDeleteFileResponse(DeleteFileResultDto deleteFileResult)
        {
            var deleteFileResponse = new DeleteFileResponse(deleteFileResult);

            return EncodeResponse(deleteFileResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateDeleteDirectoryResponse(DeleteDirectoryResultDto deleteDirectoryResult)
        {
            var deleteDirectoryResponse = new DeleteDirectoryResponse(deleteDirectoryResult);

            return EncodeResponse(deleteDirectoryResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateCreateDirectoryResponse(CreateDirectoryResultDto createDirectoryResult)
        {
            var createDirectoryResponse = new CreateDirectoryResponse(createDirectoryResult);

            return EncodeResponse(createDirectoryResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateGetFileInfoResponse(GetFileInfoResultDto fileInfoResultDto)
        {
            var createDirectoryResponse = new GetFileInfoResponse(fileInfoResultDto);

            return EncodeResponse(createDirectoryResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateTruncateFileResponse(TruncateFileResultDto truncateFileResultDto)
        {
            var truncateFileResponse = new TruncateFileResponse(truncateFileResultDto);

            return EncodeResponse(truncateFileResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateSetTimestampsResponse(SetTimestampsResultDto setTimestampsResultDto)
        {
            var setTimestampsResponse = new SetTimestampsResponse(setTimestampsResultDto);

            return EncodeResponse(setTimestampsResponse, null);
        }

        public IReadOnlyCollection<byte> GenerateMoveResponse(MoveResultDto moveResultDto)
        {
            var moveResponse = new MoveResponse(moveResultDto);

            return EncodeResponse(moveResponse, null);
        }
    }
}
