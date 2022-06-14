using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    /// <summary>
    /// Data, read from file
    /// </summary>
    public class DownloadedFileDto
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
        public ulong StartPosition { get; private set; }

        /// <summary>
        /// Content length
        /// </summary>
        [JsonPropertyName("Length")]
        public uint Length { get; private set; }


        public DownloadedFileDto(bool isSuccessful,
            ulong startPosition,
            uint length)
        {
            IsSuccessful = isSuccessful;
            StartPosition = startPosition;
            Length = length;
        }
    }
}
