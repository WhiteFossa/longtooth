using System;
using System.Collections.Generic;

namespace longtooth.Common.Abstractions.DTOs.ClientService
{
    public class FilesystemItemDto
    {
        /// <summary>
        /// Is exist?
        /// </summary>
        public bool IsExist { get; private set; }

        /// <summary>
        /// Is directory?
        /// </summary>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Item path
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// File size, only for files
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Sub-items
        /// </summary>
        public IReadOnlyCollection<FilesystemItemDto> Content { get; private set; }

        public FilesystemItemDto(bool isExist,
            bool isDirectory,
            string path,
            string name,
            long size,
            IReadOnlyCollection<FilesystemItemDto> content)
        {
            IsExist = isExist;
            IsDirectory = isDirectory;
            Path = path;
            Name = name;
            Size = size;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
