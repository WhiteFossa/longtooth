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
                        new List<FilesystemItemDto>()))
                    .ToList();

                return new FilesystemItemDto(true, true, @"/", @"/", 0, content);
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
                        new List<FilesystemItemDto>()))
                    .ToList();

                return new FilesystemItemDto(true, true, path, path, 0, content);
            }

            // OK, it's a file, not a directory
            var metadata = await GetFileMetadata(path);
            if (metadata.IsExist)
            {
                return new FilesystemItemDto(true,
                    false,
                    path,
                    metadata.Name,
                    metadata.Size,
                    new List<FilesystemItemDto>());
            }

            // Non-existent item
            return new FilesystemItemDto(false, false, @"", @"", 0, new List<FilesystemItemDto>());
        }

        public async Task<FileMetadata> GetFileMetadata(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.GetFileInfoCommand(LocalPathToServerSidePath(path)));
            _stopWaitHandle.WaitOne();

            var fileInfo = _getFileInfoRunResult.GetFileInfoResult;

            if (!fileInfo.IsExist)
            {
                return new FileMetadata(false, String.Empty, path, 0);
            }

            return new FileMetadata(true, fileInfo.Name, path, fileInfo.Size);
        }

        public async Task<FileContent> GetFileContent(string path, long offset, long maxLength)
        {
            await PrepareAndSendCommand(
                _commandGenerator.GenerateDownloadCommand(LocalPathToServerSidePath(path), offset, (int)maxLength));
            _stopWaitHandle.WaitOne();

            if (!_downloadFileRunResult.File.IsSuccessful)
            {
                return new FileContent(false, false, new List<byte>());
            }

            return new FileContent(true, true, _downloadFileRunResult.File.Content);

        }
    }
}
