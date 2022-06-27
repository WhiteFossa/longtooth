using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class SetTimestampsResultDto
    {
        /// <summary>
        /// Is successful?
        /// </summary>
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        public SetTimestampsResultDto(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }
}
