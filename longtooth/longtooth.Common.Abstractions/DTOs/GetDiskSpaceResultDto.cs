using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class GetDiskSpaceResultDto
    {
        [JsonPropertyName("IsSuccessful")]
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Disk size
        /// </summary>
        [JsonPropertyName("DiskSize")]
        public long DiskSize { get; private set; }

        /// <summary>
        /// Free disk space
        /// </summary>
        [JsonPropertyName("FreeSpace")]
        public long FreeSpace { get; private set; }

        public GetDiskSpaceResultDto(bool isSuccessful, long diskSize, long freeSpace)
        {
            IsSuccessful = isSuccessful;
            DiskSize = diskSize;
            FreeSpace = freeSpace;
        }
    }
}
