using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Protocol.Abstractions.DataStructures;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.Responses
{
    public class GetDirectoryContentResponse : ResponseHeader
    {
        /// <summary>
        /// Directory content
        /// </summary>
        [JsonPropertyName("Content")]
        public DirectoryContentDto Content { get; set; }

        public GetDirectoryContentResponse(DirectoryContentDto content) : base(CommandType.GetDirectoryContent)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public static GetDirectoryContentResponse Parse(string header, List<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<GetDirectoryContentResponse>(header);
        }

        public async override Task<ResponseRunResult> RunAsync()
        {
            return new GetDirectoryContentRunResult(Content);
        }
    }
}
