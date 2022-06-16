using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class DeleteFileResultDto
    {
        /// <summary>
        /// Was file deleted successfully?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public DeleteFileResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
