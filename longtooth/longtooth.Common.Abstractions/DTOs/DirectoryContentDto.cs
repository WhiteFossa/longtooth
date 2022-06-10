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
        /// Directory content
        /// </summary>
        [JsonPropertyName("Items")]
        public List<DirectoryContentItemDto> Items { get; private set; }

        public DirectoryContentDto(bool isSuccessful, List<DirectoryContentItemDto> items)
        {
            IsSuccessful = isSuccessful;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}
