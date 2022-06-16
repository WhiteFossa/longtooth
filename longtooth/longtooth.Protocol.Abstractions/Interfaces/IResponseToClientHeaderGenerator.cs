using longtooth.Common.Abstractions.DTOs;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to generate responses to client
    /// </summary>
    public interface IResponseToClientHeaderGenerator
    {
        /// <summary>
        /// Generates response to ping command
        /// </summary>
        byte[] GeneratePingResponse();

        /// <summary>
        /// Generates response to exit command
        /// </summary>
        /// <returns></returns>
        byte[] GenerateExitResponse();

        /// <summary>
        /// Generates response with the list of server's mountpoints
        /// </summary>
        byte[] GenerateGetMountpointsResponse(List<MountpointDto> mountpoints);

        /// <summary>
        /// Generates directory content response
        /// </summary>
        byte[] GenerateGetDirectoryContentResponse(DirectoryContentDto directoryContent);

        /// <summary>
        /// Generates response for downloaded file
        /// </summary>
        byte[] GenerateDownloadFileResponse(DownloadedFileWithContentDto file);

        /// <summary>
        /// Generate response for file creation
        /// </summary>
        byte[] GenerateCreateFileResponse(CreateFileResultDto fileCreationResult);

        /// <summary>
        /// Generate response for file update
        /// </summary>
        byte[] GenerateUpdateFileResponse(UpdateFileResultDto updateFileResult);

        /// <summary>
        /// Generate response for file delete
        /// </summary>
        byte[] GenerateDeleteFileResponse(DeleteFileResultDto deleteFileResult);

        /// <summary>
        /// Generate response for directory deletion
        /// </summary>
        byte[] GenerateDeleteDirectoryResponse(DeleteDirectoryResultDto deleteDirectoryResult);

        /// <summary>
        /// Generate response for directory creation
        /// </summary>
        byte[] GenerateCreateDirectoryResponse(CreateDirectoryResultDto createDirectoryResult);
    }
}
