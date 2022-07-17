using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Protocol.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace longtooth.Common.Implementations.ClientService
{
    public class ClientService : IClientService
    {
        private readonly IClientSideMessagesProcessor _clientSideMessagesProcessor;
        private readonly ICommandToServerHeaderGenerator _commandGenerator;
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IClient _client;

        /// <summary>
        /// Last mountpoints enumeration result
        /// </summary>
        private IReadOnlyCollection<MountpointDto> _mountpoints;
        private AutoResetEvent _getMountpointsWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Last get directory content call result
        /// </summary>
        private GetDirectoryContentRunResult _directoryContent;
        private AutoResetEvent _getDirectoryContentWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last download command
        /// </summary>
        private DownloadFileRunResult _downloadFileRunResult;
        private AutoResetEvent _downloadFileWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "get file info" call
        /// </summary>
        private GetFileInfoRunResult _getFileInfoRunResult;
        private AutoResetEvent _getFileInfoWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "create directory" call
        /// </summary>
        private CreateDirectoryRunResult _createDirectoryRunResult;
        private AutoResetEvent _createDirectoryWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "delete directory" call
        /// </summary>
        private DeleteDirectoryRunResult _deleteDirectoryRunResult;
        private AutoResetEvent _deleteDirectoryWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "create file" call
        /// </summary>
        private CreateFileRunResult _createFileRunResult;
        private AutoResetEvent _createFileWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "delete file" call
        /// </summary>
        private DeleteFileRunResult _deleteFileRunResult;
        private AutoResetEvent _deleteFileWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "update file" call
        /// </summary>
        private UpdateFileRunResult _updateFileRunResult;
        private AutoResetEvent _updateFileWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of last "truncate file" call
        /// </summary>
        private TruncateFileRunResult _truncateFileRunResult;
        private AutoResetEvent _truncateFileWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of "set timestamp" command
        /// </summary>
        private SetTimestampsRunResult _setTimestampsRunResult;
        private AutoResetEvent _setTimestampsWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of "move file/directory" command
        /// </summary>
        private MoveRunResult _moveRunResult;
        private AutoResetEvent _moveWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// Result of "get disk space" command
        /// </summary>
        private GetDiskSpaceRunResult _getDiskSpaceRunResult;
        private AutoResetEvent _getDiskSpaceWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// To make multithreaded calls synchronous single-threaded
        /// </summary>
        private AutoResetEvent _syncWaitHandle = new AutoResetEvent(true);

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

            _syncWaitHandle.Set();

            switch (runResult.ResponseToCommand)
            {
                case CommandType.GetMountpoints:
                    _mountpoints = (runResult as GetMountpointsRunResult).Mountpoints;
                    _getMountpointsWaitHandle.Set();
                    break;

                case CommandType.GetDirectoryContent:
                    _directoryContent = runResult as GetDirectoryContentRunResult;
                    _getDirectoryContentWaitHandle.Set();
                    break;

                case CommandType.DownloadFile:
                    _downloadFileRunResult = runResult as DownloadFileRunResult;
                    _downloadFileWaitHandle.Set();
                    break;

                case CommandType.GetFileInfo:
                    _getFileInfoRunResult = runResult as GetFileInfoRunResult;
                    _getFileInfoWaitHandle.Set();
                    break;

                case CommandType.CreateDirectory:
                    _createDirectoryRunResult = runResult as CreateDirectoryRunResult;
                    _createDirectoryWaitHandle.Set();
                    break;

                case CommandType.DeleteDirectory:
                    _deleteDirectoryRunResult = runResult as DeleteDirectoryRunResult;
                    _deleteDirectoryWaitHandle.Set();
                    break;

                case CommandType.CreateFile:
                    _createFileRunResult = runResult as CreateFileRunResult;
                    _createFileWaitHandle.Set();
                    break;

                case CommandType.DeleteFile:
                    _deleteFileRunResult = runResult as DeleteFileRunResult;
                    _deleteFileWaitHandle.Set();
                    break;

                case CommandType.UpdateFile:
                    _updateFileRunResult = runResult as UpdateFileRunResult;
                    _updateFileWaitHandle.Set();
                    break;

                case CommandType.TruncateFile:
                    _truncateFileRunResult = runResult as TruncateFileRunResult;
                    _truncateFileWaitHandle.Set();
                    break;

                case CommandType.SetTimestamps:
                    _setTimestampsRunResult = runResult as SetTimestampsRunResult;
                    _setTimestampsWaitHandle.Set();
                    break;

                case CommandType.Move:
                    _moveRunResult = runResult as MoveRunResult;
                    _moveWaitHandle.Set();
                    break;

                case CommandType.GetDiskSpace:
                    _getDiskSpaceRunResult = runResult as GetDiskSpaceRunResult;
                    _getDiskSpaceWaitHandle.Set();
                    break;

                default:
                    throw new InvalidOperationException("Unknown command type in response!");
            }
        }

        private async Task PrepareAndSendCommand(byte[] commandMessage)
        {
            _syncWaitHandle.WaitOne();
            _syncWaitHandle.Reset();

            var encodedMessage = _messagesProcessor.PrepareMessageToSend(commandMessage);
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
                _getMountpointsWaitHandle.WaitOne();

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
            _getDirectoryContentWaitHandle.WaitOne();

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
            _getFileInfoWaitHandle.WaitOne();

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
            _downloadFileWaitHandle.WaitOne();

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
            _createDirectoryWaitHandle.WaitOne();

            return _createDirectoryRunResult.CreateDirectoryResult.IsSuccessful;
        }

        public async Task<bool> DeleteDirectoryAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.DeleteDirectoryCommand(LocalPathToServerSidePath(path)));
            _deleteDirectoryWaitHandle.WaitOne();

            return _deleteDirectoryRunResult.DeleteDirectoryResult.IsSuccessful;
        }

        public async Task<bool> CreateFileAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.CreateFileCommand(LocalPathToServerSidePath(path)));
            _createFileWaitHandle.WaitOne();

            return _createFileRunResult.CreateFileResult.IsSuccessful;
        }

        public async Task<bool> DeleteFileAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.DeleteFileCommand(LocalPathToServerSidePath(path)));
            _deleteFileWaitHandle.WaitOne();

            return _deleteFileRunResult.DeleteFileResult.IsSuccessful;
        }

        public async Task<UpdateFileResultDto> UpdateFileContentAsync(string path, ulong offset, IReadOnlyCollection<byte> buffer)
        {
            await PrepareAndSendCommand(
                _commandGenerator.UpdateFileCommand(LocalPathToServerSidePath(path), (long)offset, buffer.ToArray()));
            _updateFileWaitHandle.WaitOne();

            return _updateFileRunResult.UpdateFileResult;
        }

        public async Task<bool> TruncateFileAsync(string path, ulong newSize)
        {
            await PrepareAndSendCommand(
                _commandGenerator.TruncateFileCommand(LocalPathToServerSidePath(path), newSize));
            _truncateFileWaitHandle.WaitOne();

            return _truncateFileRunResult.TruncateFileResult.IsSuccessful;
        }

        public async Task<bool> SetTimestampsAsync(string path, DateTime atime, DateTime ctime, DateTime mtime)
        {
            await PrepareAndSendCommand(
                _commandGenerator.SetTimestampsCommand(LocalPathToServerSidePath(path), atime, ctime, mtime));
            _setTimestampsWaitHandle.WaitOne();

            return _setTimestampsRunResult.SetTimestampsResult.IsSuccessful;
        }

        public async Task<bool> MoveAsync(string from, string to, bool isOverwrite)
        {
            await PrepareAndSendCommand(
                _commandGenerator.MoveCommand(LocalPathToServerSidePath(from), LocalPathToServerSidePath(to), isOverwrite));
            _moveWaitHandle.WaitOne();

            return _moveRunResult.MoveResult.IsSuccessful;
        }

        public async Task<GetDiskSpaceResultDto> GetDiskSpaceAsync(string path)
        {
            await PrepareAndSendCommand(
                _commandGenerator.GetDiskSpaceCommand(LocalPathToServerSidePath(path)));
            _getDiskSpaceWaitHandle.WaitOne();

            return _getDiskSpaceRunResult.GetDiskSpaceResult;
        }
    }
}
