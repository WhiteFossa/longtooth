using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class TruncateFileResultDto
    {
        /// <summary>
        /// Is successful?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public TruncateFileResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
