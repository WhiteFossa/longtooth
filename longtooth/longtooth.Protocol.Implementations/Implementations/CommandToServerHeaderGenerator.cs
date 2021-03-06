using System;
using longtooth.Protocol.Abstractions.Commands;
using longtooth.Protocol.Abstractions.DataStructures;
using longtooth.Protocol.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace longtooth.Protocol.Implementations.Implementations
{
    public class CommandToServerHeaderGenerator : ICommandToServerHeaderGenerator
    {
        private IReadOnlyCollection<byte> EncodeCommand(object command, byte[] data)
        {
            var header = JsonSerializer.Serialize(command);
            var serializedHeader = Encoding.UTF8.GetBytes(header);

            var message = new Message(serializedHeader, data);

            return message.ToDataPacket();
        }

        public IReadOnlyCollection<byte> GeneratePingCommand()
        {
            var pingCommand = new PingCommand(null, null);

            return EncodeCommand(pingCommand, null);
        }

        public IReadOnlyCollection<byte> GenerateExitCommand()
        {
            var exitCommand = new ExitCommand(null, null);

            return EncodeCommand(exitCommand, null);
        }

        public IReadOnlyCollection<byte> GenerateGetMountpointsCommand()
        {
            var getMountpointsCommand = new GetMountpointsCommand(null, null, null);

            return EncodeCommand(getMountpointsCommand, null);
        }

        public IReadOnlyCollection<byte> GenerateGetDirectoryContentCommand(string serverSidePath)
        {
            var getDirectoryContentCommand = new GetDirectoryContentCommand(serverSidePath,
                null,
                null,
                null);

            return EncodeCommand(getDirectoryContentCommand, null);
        }

        public IReadOnlyCollection<byte> GenerateDownloadCommand(string path, long startPosition, int length)
        {
            var downloadFileCommand = new DownloadCommand(path,
                startPosition,
                length,
                null,
                null,
                null);

            return EncodeCommand(downloadFileCommand, null);
        }

        public IReadOnlyCollection<byte> CreateFileCommand(string path)
        {
            var createFileCommand = new CreateFileCommand(path,
                null,
                null,
                null);

            return EncodeCommand(createFileCommand, null);
        }

        public IReadOnlyCollection<byte> UpdateFileCommand(string path, long startPosition, byte[] dataToWrite)
        {
            var updateFileCommand = new UpdateFileCommand(path,
                startPosition,
                dataToWrite.ToList(),
                null,
                null,
                null);

            return EncodeCommand(updateFileCommand, updateFileCommand.Content.ToArray());
        }

        public IReadOnlyCollection<byte> DeleteFileCommand(string path)
        {
            var deleteFileCommand = new DeleteFileCommand(path,
                null,
                null,
                null);

            return EncodeCommand(deleteFileCommand, null);
        }

        public IReadOnlyCollection<byte> DeleteDirectoryCommand(string path)
        {
            var deleteDirectoryCommand = new DeleteDirectoryCommand(path,
                null,
                null,
                null);

            return EncodeCommand(deleteDirectoryCommand, null);
        }

        public IReadOnlyCollection<byte> CreateDirectoryCommand(string path)
        {
            var createDirectoryCommand = new CreateDirectoryCommand(path,
                null,
                null,
                null);

            return EncodeCommand(createDirectoryCommand, null);
        }

        public IReadOnlyCollection<byte> GetFileInfoCommand(string path)
        {
            var getFileInfoCommand = new GetFileInfoCommand(path,
                null,
                null,
                null);

            return EncodeCommand(getFileInfoCommand, null);
        }

        public IReadOnlyCollection<byte> TruncateFileCommand(string path, ulong newSize)
        {
            var truncateFileCommand = new TruncateFileCommand(path,
                newSize,
                null,
                null,
                null);

            return EncodeCommand(truncateFileCommand, null);
        }

        public IReadOnlyCollection<byte> SetTimestampsCommand(string path, DateTime atime, DateTime ctime, DateTime mtime)
        {
            var setTimestampsCommand = new SetTimestampsCommand(path,
                atime,
                ctime,
                mtime,
                null,
                null,
                null);

            return EncodeCommand(setTimestampsCommand, null);
        }

        public IReadOnlyCollection<byte> MoveCommand(string from, string to, bool isOverwrite)
        {
            var moveCommand = new MoveCommand(from,
                to,
                isOverwrite,
                null,
                null,
                null);

            return EncodeCommand(moveCommand, null);
        }

        public IReadOnlyCollection<byte> GetDiskSpaceCommand(string path)
        {
            var getDiskSpaceCommand = new GetDiskSpaceCommand(path,
                null,
                null,
                null);

            return EncodeCommand(getDiskSpaceCommand, null);
        }
    }
}
