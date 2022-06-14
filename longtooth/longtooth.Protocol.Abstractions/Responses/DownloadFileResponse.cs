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
    public class DownloadFileResponse : ResponseHeader
    {
        /// <summary>
        /// Downloaded file metadata
        /// </summary>
        [JsonPropertyName("File")]
        public DownloadedFileDto File { get; private set; }

        /// <summary>
        /// Partial file content
        /// </summary>
        public List<byte> FileContent { get; private set; }

        public DownloadFileResponse(DownloadedFileDto file, List<byte> fileContent) : base(CommandType.DownloadFile)
        {
            File = file;
            FileContent = fileContent;

            if (File == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (FileContent == null)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }
        }

        public static DownloadFileResponse Parse(string header)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));

            return JsonSerializer.Deserialize<DownloadFileResponse>(header);
        }
        public async override Task<ResponseRunResult> RunAsync()
        {
            return new DownloadFileRunResult(File);
        }

    }
}
