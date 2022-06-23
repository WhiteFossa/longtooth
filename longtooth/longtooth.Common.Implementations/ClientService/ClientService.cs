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
            // TODO: Implement "GetFileInfo" command
            var parentDirectory = FilesHelper.MoveUp(path);
            await PrepareAndSendCommand(
                _commandGenerator.GenerateGetDirectoryContentCommand(LocalPathToServerSidePath(parentDirectory)));
            _stopWaitHandle.WaitOne();

            if (_directoryContent.DirectoryContent.IsSuccessful)
            {
                var filename = FilesHelper.GetFileOrDirectoryName(path);

                var file = _directoryContent
                    .DirectoryContent
                    .Items
                    .Where(i => !i.IsDirectory)
                    .FirstOrDefault(i => i.Name.Equals(filename));

                if (file != null)
                {
                    return new FilesystemItemDto(true,
                        false,
                        path,
                        file.Name,
                        file.Size,
                        new List<FilesystemItemDto>());
                }
            }

            // Non-existent item
            return new FilesystemItemDto(false, false, @"", @"", 0, new List<FilesystemItemDto>());
        }

        public async Task<FileMetadata> GetFileMetadata(string path)
        {
            if (path.Equals(@"/testfile.txt"))
            {
                return new FileMetadata(true, "testfile.txt", @"/testfile.txt");
            }
            else if (path.Equals(@"/Dir 1/testfile1.txt"))
            {
                return new FileMetadata(true, "testfile1.txt", @"/Dir 1/testfile1.txt");
            }
            else if (path.Equals(@"/Dir 2/testfile2.txt"))
            {
                return new FileMetadata(true, "testfile2.txt", @"/Dir 2/testfile2.txt");
            }

            // Not found
            return new FileMetadata(false, String.Empty, String.Empty);
        }

        public async Task<FileContent> GetFileContent(string path, long offset, long maxLength)
        {
            byte[] content = null;

            if (path.Equals(@"/testfile.txt"))
            {
                content = Encoding.UTF8.GetBytes("This is testfile.txt sample content");
            }
            else if (path.Equals(@"/Dir 1/testfile1.txt"))
            {
                content = Encoding.UTF8.GetBytes("This is testfile1.txt sample content");
            }
            else if (path.Equals(@"/Dir 2/testfile2.txt"))
            {
                content = Encoding.UTF8.GetBytes("This is testfile2.txt sample content");
            }
            else
            {
                // Not found
                return new FileContent(false, false, new List<byte>());
            }

            var isInRange = offset < content.Length;

            var canRead = Math.Min(maxLength, content.Length - offset);

            List<byte> result;

            if (!isInRange)
            {
                result = new List<byte>();
            }
            else
            {
                result = content.ToList()
                    .GetRange((int)offset, (int)canRead);
            }

            return new FileContent(true, isInRange, result);
        }
    }
}
