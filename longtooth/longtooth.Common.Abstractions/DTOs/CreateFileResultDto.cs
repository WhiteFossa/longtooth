using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    /// <summary>
    /// Information about file creation
    /// </summary>
    public class CreateFileResultDto
    {
        /// <summary>
        /// Was file created successfuly?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public CreateFileResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
