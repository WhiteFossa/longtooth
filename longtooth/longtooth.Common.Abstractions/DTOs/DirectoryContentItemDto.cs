using System;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    /// <summary>
    /// Directory content item (i.e. file or directory)
    /// </summary>
    public class DirectoryContentItemDto
    {
        /// <summary>
        /// Is it file or directory?
        /// </summary>
        [JsonPropertyName("IsDirectory")]
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// File / directory name
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name { get; private set; }

        /// <summary>
        /// File size
        /// </summary>
        [JsonPropertyName("Size")]
        public long Size { get; private set; }

        /// <summary>
        /// Last access time
        /// </summary>
        [JsonPropertyName("Atime")]
        public DateTime Atime { get; private set; }

        /// <summary>
        /// Last metadata change time
        /// </summary>
        [JsonPropertyName("Ctime")]
        public DateTime Ctime { get; private set; }

        /// <summary>
        /// Last content change time
        /// </summary>
        [JsonPropertyName("Mtime")]
        public DateTime Mtime { get; private set; }

        public DirectoryContentItemDto(bool isDirectory,
            string name,
            long size,
            DateTime atime,
            DateTime ctime,
            DateTime mtime)
        {
            IsDirectory = isDirectory;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
            Atime = atime;
            Ctime = ctime;
            Mtime = mtime;
        }
    }

}
