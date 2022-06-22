using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using longtooth.Common.Abstractions.DTOs.ClientService;
using longtooth.Common.Abstractions.Interfaces.ClientService;

namespace longtooth.Common.Implementations.ClientService
{
    public class ClientService : IClientService
    {
        public async Task<FilesystemItemDto> GetDirectoryContentAsync(string path)
        {
            if (path.Equals(@"/"))
            {
                var subcontent = new List<FilesystemItemDto>();
                subcontent.Add(new FilesystemItemDto(true, true, @"/Dir 1", "Dir 1", 0, new List<FilesystemItemDto>()));
                subcontent.Add(new FilesystemItemDto(true, true, @"/Dir 2", "Dir 2", 0, new List<FilesystemItemDto>()));
                subcontent.Add(new FilesystemItemDto(true, false, @"/testfile.txt", "testfile.txt", 1337, new List<FilesystemItemDto>()));

                return new FilesystemItemDto(true, true, @"/", @"/", 0, subcontent);
            }
            else if (path.Equals(@"/testfile.txt"))
            {
                var subcontent = new List<FilesystemItemDto>();
                return new FilesystemItemDto(true, false, @"/testfile.txt", @"testfile.txt", 1337, subcontent);
            }
            else if (path.Equals(@"/Dir 1"))
            {
                var subcontent = new List<FilesystemItemDto>();
                subcontent.Add(new FilesystemItemDto(true, true, @"/Dir 1/Dir 3", "Dir 3", 0, new List<FilesystemItemDto>()));
                subcontent.Add(new FilesystemItemDto(true, false, @"/Dir 1/testfile1.txt", "testfile1.txt", 1337, new List<FilesystemItemDto>()));

                return new FilesystemItemDto(true, true, @"/Dir 1", @"Dir 1", 0, subcontent);
            }
            else if (path.Equals(@"/Dir 1/Dir 3"))
            {
                var subcontent = new List<FilesystemItemDto>();
                return new FilesystemItemDto(true, true, @"/Dir 1/Dir 3", @"Dir 3", 0, subcontent);
            }
            else if (path.Equals(@"/Dir 1/testfile1.txt"))
            {
                var subcontent = new List<FilesystemItemDto>();
                return new FilesystemItemDto(true, false, @"/Dir 1/testfile1.txt", @"testfile1.txt", 1337, subcontent);
            }
            else if (path.Equals(@"/Dir 2"))
            {
                var subcontent = new List<FilesystemItemDto>();
                subcontent.Add(new FilesystemItemDto(true, true, @"/Dir 2/Dir 4", "Dir 4", 0, new List<FilesystemItemDto>()));
                subcontent.Add(new FilesystemItemDto(true, false, @"/Dir 2/testfile2.txt", "testfile2.txt", 1337, new List<FilesystemItemDto>()));

                return new FilesystemItemDto(true, true, @"/Dir 2", @"Dir 2", 0, subcontent);
            }
            else if (path.Equals(@"/Dir 2/Dir 4"))
            {
                var subcontent = new List<FilesystemItemDto>();
                return new FilesystemItemDto(true, true, @"/Dir 2/Dir 4", @"Dir 4", 0, subcontent);
            }
            else if (path.Equals(@"/Dir 2/testfile2.txt"))
            {
                var subcontent = new List<FilesystemItemDto>();
                return new FilesystemItemDto(true, false, @"/Dir 2/testfile2.txt", @"testfile2.txt", 1337, subcontent);
            }

            // Incorrect directory
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
