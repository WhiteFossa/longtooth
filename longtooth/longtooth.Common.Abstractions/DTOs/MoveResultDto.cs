using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class MoveResultDto
    {
        /// <summary>
        /// Is successful?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public MoveResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
