using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    /// <summary>
    /// Information about server's directory
    /// </summary>
    public class DirectoryContentDto
    {
        /// <summary>
        /// Is directory listing successful?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

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

        /// <summary>
        /// Directory content
        /// </summary>
        [JsonPropertyName("Items")]
        public IReadOnlyCollection<DirectoryContentItemDto> Items { get; private set; }

        public DirectoryContentDto(bool isSuccessful,
            DateTime atime,
            DateTime ctime,
            DateTime mtime,
            IReadOnlyCollection<DirectoryContentItemDto> items)
        {
            IsSuccessful = isSuccessful;
            Atime = atime;
            Ctime = ctime;
            Mtime = mtime;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}
