using System.Text.Json.Serialization;

namespace longtooth.Common.Abstractions.DTOs
{
    public class GetFileInfoResultDto
    {
        /// <summary>
        /// Is file exist?
        /// </summary>
        [JsonPropertyName("IsExist")]
        public bool IsExist { get; private set; }

        /// <summary>
        /// Server-side path to phile
        /// </summary>
        [JsonPropertyName("Path")]
        public string Path { get; private set; }

        /// <summary>
        /// Filename (without path)
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name { get; private set; }

        /// <summary>
        /// File size
        /// </summary>
        [JsonPropertyName("Size")]
        public long Size { get; private set; }
        

        public GetFileInfoResultDto(bool isExist, string path, string name, long size)
        {
            IsExist = isExist;
            Path = path;
            Name = name;
            Size = size;
        }
    }
}
