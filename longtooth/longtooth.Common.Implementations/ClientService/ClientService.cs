using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Implementations.Helpers;
using longtooth.Protocol.Abstractions.Interfaces;

namespace longtooth.Common.Implementations.ClientService
{
    public class ClientService : IClientService
    {
        private readonly IClientSideMessagesProcessor _clientSideMessagesProcessor;
        private readonly ICommandToServerHeaderGenerator _commandGenerator;
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IClient _client;

        /// <summary>
        /// To wait for callbacks
        /// </summary>
        private AutoResetEvent _stopWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Last mountpoints enumeration result
        /// </summary>
        private IReadOnlyCollection<MountpointDto> _mountpoints;

        /// <summary>
        /// Last get directory content call result
        /// </summary>
        private GetDirectoryContentRunResult _directoryContent;

        /// <summary>
        /// Result of last download command
        /// </summary>
        private DownloadFileRunResult _downloadFileRunResult;

        /// <summary>
        /// Result of last "get file info" call
        /// </summary>
        private GetFileInfoRunResult _getFileInfoRunResult;

        /// <summary>
        /// Result of last "create directory" call
        /// </summary>
        private CreateDirectoryRunResult _createDirectoryRunResult;

        /// <summary>
        /// Result of last "delete directory" call
        /// </summary>
        private DeleteDirectoryRunResult _deleteDirectoryRunResult;

        /// <summary>
        /// Result of last "create file" call
        /// </summary>
        private CreateFileRunResult _createFileRunResult;

        /// <summary>
        /// Result of last "delete file" call
        /// </summary>
        private DeleteFileRunResult _deleteFileRunResult;

        /// <summary>
        /// Result of last "update file" call
        /// </summary>
        private UpdateFileRunResult _updateFileRunResult;

        /// <summary>
        /// Result of last "truncate file" call
        /// </summary>
        private TruncateFileRunResult _truncateFileRunResult;

        public ClientService(IClientSideMessagesProcessor clienSideMessagesProcessor,
            ICommandToServerHeaderGenerator commandGenerator,
            IMessagesProcessor messagesProcessor,
            IClient client
            )
        {
            _clientSideMessagesProcessor = clienSideMessagesProcessor;
            _commandGenerator = commandGenerator;
            _messagesProcessor = messagesProcessor;
            _client = client;
        }

        public async Task OnNewMessageAsync(IReadOnlyCollection<byte> decodedMessage)
        {
            var response = _clientSideMessagesProcessor.ParseMessage(decodedMessage);
            var runResult = await response.RunAsync();

            switch (runResult.ResponseToCommand)
            {
                case CommandType.GetMountpoints:
                    _mountpoints = (runResult as GetMountpointsRunResult).Mountpoints;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.GetDirectoryContent:
                    _directoryContent = runResult as GetDirectoryContentRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.DownloadFile:
                    _downloadFileRunResult = runResult as DownloadFileRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.GetFileInfo:
                    _getFileInfoRunResult = runResult as GetFileInfoRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.CreateDirectory:
                    _createDirectoryRunResult = runResult as CreateDirectoryRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.DeleteDirectory:
                    _deleteDirectoryRunResult = runResult as DeleteDirectoryRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.CreateFile:
                    _createFileRunResult = runResult as CreateFileRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.DeleteFile:
                    _deleteFileRunResult = runResult as DeleteFileRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.UpdateFile:
                    _updateFileRunResult = runResult as UpdateFileRunResult;
                    _stopWaitHandle.Set();
                    break;

                case CommandType.TruncateFile:
                    _truncateFileRunResult = runResult as TruncateFileRunResult;
                    _stopWaitHandle.Set();
                    break;

                default:
                    throw new InvalidOperationException("Unknown command type in response!");
            }
        }

        private async Task PrepareAndSendCommand(IReadOnlyCollection<byte> commandMessage)
        {
            var encodedMessage = _messagesProcessor.PrepareMessageToSend(new List<byte>(commandMessage));
            await _client.SendAsync(encodedMessage);
        }

        private string MountpointToLocalPath(string path)
        {
            // TODO: Make me better
            return path.Replace(@"/storage/emulated/0", @"");
        }

        private string ServerSidePathToLocalPath(string path)
        {
            // TODO: Make me better
            return path.Replace(@"/storage/emulated/0", @"/");
        }

        private string LocalPathToServerSidePath(string path)
        {
            // TODO: Make me better
            return @$"/storage/emulated/0{ path }";
        }

        public async Task<FilesystemItemDto> GetDirectoryContentAsync(string path)
        {
            if (path.Equals(@"/"))
            {
                // Enumerating mountpoints
                await PrepareAndSendCommand(_commandGenerator.GenerateGetMountpointsCommand());
                _stopWaitHandle.WaitOne();

                var content = _mountpoints
                    .Select(mp => new FilesystemItemDto(true,
                        true,
                        MountpointToLocalPath(mp.ServerSidePath),
                        mp.Name,
                        0,
                        DateTime.UnixEpoch,
                        DateTime.UnixEpoch,
                        DateTime.UnixEpoch,
                        new List<FilesystemItemDto>()))
                    .ToList();

                return new FilesystemItemDto(true,
                    true,
                    @"/",
                    @"/",
                    0,
                    DateTime.UnixEpoch,
                    DateTime.UnixEpoch,
                    DateTime.UnixEpoch,
                    content);
            }

            // Ordinary directory or file, trying to get directory first
            await PrepareAndSendCommand(
                _commandGenerator.GenerateGetDirectoryContentCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            if (_directoryContent.DirectoryContent.IsSuccessful)
            {
                // Sure, this is directory
                var content = _directoryContent
                    .DirectoryContent
                    .Items
                    .Select(di => new FilesystemItemDto(true,
                        di.IsDirectory,
                        $@"{ path }/{ di.Name }",
                        di.Name,
                        di.Size,
                        di.Atime,
                        di.Ctime,
                        di.Mtime,
                        new List<FilesystemItemDto>()))
                    .ToList();

                return new FilesystemItemDto(true,
                    true,
                    path,
                    path,
                    0,
                    _directoryContent.DirectoryContent.Atime,
                    _directoryContent.DirectoryContent.Ctime,
                    _directoryContent.DirectoryContent.Mtime,
                    content);
            }

            // OK, it's a file, not a directory
            var metadata = await GetFileMetadataAsync(path);
            if (metadata.IsExist)
            {
                return new FilesystemItemDto(true,
                    false,
                    path,
                    metadata.Name,
                    metadata.Size,
                    metadata.Atime,
                    metadata.Ctime,
                    metadata.Mtime,
                    new List<FilesystemItemDto>());
            }

            // Non-existent item
            return new FilesystemItemDto(false,
                false,
                @"",
                @"",
                0,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                DateTime.UnixEpoch,
                new List<FilesystemItemDto>());
        }

        public async Task<FileMetadataDto> GetFileMetadataAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.GetFileInfoCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            var fileInfo = _getFileInfoRunResult.GetFileInfoResult;

            if (!fileInfo.IsExist)
            {
                return new FileMetadataDto(false,
                    String.Empty,
                    path,
                    0,
                    DateTime.UnixEpoch,
                    DateTime.UnixEpoch,
                    DateTime.UnixEpoch);
            }

            return new FileMetadataDto(true,
                fileInfo.Name,
                path,
                fileInfo.Size,
                fileInfo.Atime,
                fileInfo.Ctime,
                fileInfo.Mtime);
        }

        public async Task<FileContentDto> GetFileContentAsync(string path, long offset, long maxLength)
        {
            await PrepareAndSendCommand(
                _commandGenerator.GenerateDownloadCommand(LocalPathToServerSidePath(path), offset, (int)maxLength));
            _stopWaitHandle.WaitOne();

            if (!_downloadFileRunResult.File.IsSuccessful)
            {
                return new FileContentDto(false, false, new List<byte>());
            }

            return new FileContentDto(true, true, _downloadFileRunResult.File.Content);

        }

        public async Task<bool> CreateDirectoryAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.CreateDirectoryCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            return _createDirectoryRunResult.CreateDirectoryResult.IsSuccessful;
        }

        public async Task<bool> DeleteDirectoryAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.DeleteDirectoryCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            return _deleteDirectoryRunResult.DeleteDirectoryResult.IsSuccessful;
        }

        public async Task<bool> CreateFileAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.CreateFileCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            return _createFileRunResult.CreateFileResult.IsSuccessful;
        }

        public async Task<bool> DeleteFileAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.DeleteFileCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            return _deleteFileRunResult.DeleteFileResult.IsSuccessful;
        }

        public async Task<UpdateFileResultDto> UpdateFileContentAsync(string path, ulong offset, IReadOnlyCollection<byte> buffer)
        {
            await PrepareAndSendCommand(
                _commandGenerator.UpdateFileCommand(LocalPathToServerSidePath(path), (long)offset, buffer.ToArray()));
            _stopWaitHandle.WaitOne();

            return _updateFileRunResult.UpdateFileResult;
        }

        public async Task<bool> TruncateFileAsync(string path, ulong newSize)
        {
            await PrepareAndSendCommand(
                _commandGenerator.TruncateFileCommand(LocalPathToServerSidePath(path), newSize));
            _stopWaitHandle.WaitOne();

            return _truncateFileRunResult.TruncateFileResult.IsSuccessful;
        }
    }
}
