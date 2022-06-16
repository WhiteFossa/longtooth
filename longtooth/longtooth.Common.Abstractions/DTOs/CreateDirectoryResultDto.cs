using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class CreateDirectoryResultDto
    {
        /// <summary>
        /// Was directory created successfully?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public CreateDirectoryResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
