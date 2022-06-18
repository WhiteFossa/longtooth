using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    /// <summary>
    /// As DownloadedFileDto, but with content
    /// </summary>
    public class DownloadedFileWithContentDto
    {
        /// <summary>
        /// Did read succesfully?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Download start position
        /// </summary>
        [JsonPropertyName("StartPosition")]
        public long StartPosition { get; private set; }

        /// <summary>
        /// Content length
        /// </summary>
        [JsonPropertyName("Length")]
        public int Length { get; private set; }

        /// <summary>
        /// Content
        /// </summary>
        [JsonPropertyName("Content")]
        public IReadOnlyCollection<byte> Content { get; private set; }

        public DownloadedFileWithContentDto(bool isSuccessful,
            long startPosition,
            int length,
            IReadOnlyCollection<byte> content)
        {
            IsSuccessful = isSuccessful;
            StartPosition = startPosition;
            Length = length;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
