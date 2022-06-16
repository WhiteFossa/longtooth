using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class DeleteDirectoryResultDto
    {
        /// <summary>
        /// Was directory deleted successfully?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public DeleteDirectoryResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
