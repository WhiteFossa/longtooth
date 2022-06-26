using System;

namespace longtooth.Common.Abstractions.DTOs.ClientService
{
    /// <summary>
    /// Information about file
    /// </summary>
    public class FileMetadataDto
    {
        /// <summary>
        /// Returns true if file exist
        /// </summary>
        public bool IsExist { get; private set; }

        /// <summary>
        /// Filename
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Full path to file
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// File size
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Last access time
        /// </summary>
        public DateTime Atime { get; private set; }

        /// <summary>
        /// Last metadata change time
        /// </summary>
        public DateTime Ctime { get; private set; }

        /// <summary>
        /// Last content change time
        /// </summary>
        public DateTime Mtime { get; private set; }

        public FileMetadataDto(bool isExist,
            string name,
            string path,
            long size,
            DateTime atime,
            DateTime ctime,
            DateTime mtime)
        {
            IsExist = isExist;
            Name = name;
            Path = path;
            Size = size;
            Atime = atime;
            Ctime = ctime;
            Mtime = mtime;
        }
    }
}
