using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class UpdateFileResultDto
    {
        /// <summary>
        /// Was file created successfuly?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// How much bytes was written (for convenience)
        /// </summary>
        [JsonPropertyName("BytesWritten")]
        public uint BytesWritten { get; private set; }

        public UpdateFileResultDto(bool isSuccessful, uint bytesWritten)
        {
            IsSuccessful = isSuccessful;
            BytesWritten = bytesWritten;
        }
    }
}
