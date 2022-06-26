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
        IReadOnlyCollection<byte> GeneratePingResponse();

        /// <summary>
        /// Generates response to exit command
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<byte> GenerateExitResponse();

        /// <summary>
        /// Generates response with the list of server's mountpoints
        /// </summary>
        IReadOnlyCollection<byte> GenerateGetMountpointsResponse(IReadOnlyCollection<MountpointDto> mountpoints);

        /// <summary>
        /// Generates directory content response
        /// </summary>
        IReadOnlyCollection<byte> GenerateGetDirectoryContentResponse(DirectoryContentDto directoryContent);

        /// <summary>
        /// Generates response for downloaded file
        /// </summary>
        IReadOnlyCollection<byte> GenerateDownloadFileResponse(DownloadedFileWithContentDto file);

        /// <summary>
        /// Generate response for file creation
        /// </summary>
        IReadOnlyCollection<byte> GenerateCreateFileResponse(CreateFileResultDto fileCreationResult);

        /// <summary>
        /// Generate response for file update
        /// </summary>
        IReadOnlyCollection<byte> GenerateUpdateFileResponse(UpdateFileResultDto updateFileResult);

        /// <summary>
        /// Generate response for file delete
        /// </summary>
        IReadOnlyCollection<byte> GenerateDeleteFileResponse(DeleteFileResultDto deleteFileResult);

        /// <summary>
        /// Generate response for directory deletion
        /// </summary>
        IReadOnlyCollection<byte> GenerateDeleteDirectoryResponse(DeleteDirectoryResultDto deleteDirectoryResult);

        /// <summary>
        /// Generate response for directory creation
        /// </summary>
        IReadOnlyCollection<byte> GenerateCreateDirectoryResponse(CreateDirectoryResultDto createDirectoryResult);

        /// <summary>
        /// Generate response for file information
        /// </summary>
        IReadOnlyCollection<byte> GenerateGetFileInfoResponse(GetFileInfoResultDto fileInfoResultDto);

        /// <summary>
        /// Generate response for file truncation / grow
        /// </summary>
        IReadOnlyCollection<byte> GenerateTruncateFileResponse(TruncateFileResultDto truncateFileResultDto);
    }
}
