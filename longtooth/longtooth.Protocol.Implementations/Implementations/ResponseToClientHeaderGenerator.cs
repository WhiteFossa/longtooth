﻿using longtooth.Common.Abstractions.DTOs;
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

        private byte[] EncodeResponse(object response, byte[] binaryData)
        {
            var header = JsonSerializer.Serialize(response);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, binaryData);

            return message.ToDataPacket();
        }

        public byte[] GeneratePingResponse()
        {
            var pingResponse = new PingResponse();

            return EncodeResponse(pingResponse, null);
        }

        public byte[] GenerateExitResponse()
        {
            var exitResponse = new ExitResponse();

            return EncodeResponse(exitResponse, null);
        }

        public byte[] GenerateGetMountpointsResponse(IReadOnlyCollection<MountpointDto> mountpoints)
        {
            var getMountpointsResponse = new GetMountpointsResponse(mountpoints);

            return EncodeResponse(getMountpointsResponse, null);
        }

        public byte[] GenerateGetDirectoryContentResponse(DirectoryContentDto directoryContent)
        {
            var getDirectoryContentResponse = new GetDirectoryContentResponse(directoryContent);

            return EncodeResponse(getDirectoryContentResponse, null);
        }

        public byte[] GenerateDownloadFileResponse(DownloadedFileWithContentDto file)
        {
            var metadata = new DownloadedFileDto(file.IsSuccessful, file.StartPosition, file.Length);

            var downloadFileResponse = new DownloadFileResponse(metadata, file.Content);

            return EncodeResponse(downloadFileResponse, downloadFileResponse.FileContent);
        }

        public byte[] GenerateCreateFileResponse(CreateFileResultDto fileCreationResult)
        {
            var createFileResponse = new CreateFileResponse(fileCreationResult);

            return EncodeResponse(createFileResponse, null);
        }

        public byte[] GenerateUpdateFileResponse(UpdateFileResultDto updateFileResult)
        {
            var updateFileResponse = new UpdateFileResponse(updateFileResult);

            return EncodeResponse(updateFileResponse, null);
        }

        public byte[] GenerateDeleteFileResponse(DeleteFileResultDto deleteFileResult)
        {
            var deleteFileResponse = new DeleteFileResponse(deleteFileResult);

            return EncodeResponse(deleteFileResponse, null);
        }

        public byte[] GenerateDeleteDirectoryResponse(DeleteDirectoryResultDto deleteDirectoryResult)
        {
            var deleteDirectoryResponse = new DeleteDirectoryResponse(deleteDirectoryResult);

            return EncodeResponse(deleteDirectoryResponse, null);
        }

        public byte[] GenerateCreateDirectoryResponse(CreateDirectoryResultDto createDirectoryResult)
        {
            var createDirectoryResponse = new CreateDirectoryResponse(createDirectoryResult);

            return EncodeResponse(createDirectoryResponse, null);
        }

        public byte[] GenerateGetFileInfoResponse(GetFileInfoResultDto fileInfoResultDto)
        {
            var createDirectoryResponse = new GetFileInfoResponse(fileInfoResultDto);

            return EncodeResponse(createDirectoryResponse, null);
        }

        public byte[] GenerateTruncateFileResponse(TruncateFileResultDto truncateFileResultDto)
        {
            var truncateFileResponse = new TruncateFileResponse(truncateFileResultDto);

            return EncodeResponse(truncateFileResponse, null);
        }

        public byte[] GenerateSetTimestampsResponse(SetTimestampsResultDto setTimestampsResultDto)
        {
            var setTimestampsResponse = new SetTimestampsResponse(setTimestampsResultDto);

            return EncodeResponse(setTimestampsResponse, null);
        }

        public byte[] GenerateMoveResponse(MoveResultDto moveResultDto)
        {
            var moveResponse = new MoveResponse(moveResultDto);

            return EncodeResponse(moveResponse, null);
        }

        public byte[] GenerateGetDiskSpaceResponse(GetDiskSpaceResultDto getDiskSpaceResultDto)
        {
            var getDiskSpaceResponce = new GetDiskSpaceResponse(getDiskSpaceResultDto);

            return EncodeResponse(getDiskSpaceResponce, null);
        }
    }
}
