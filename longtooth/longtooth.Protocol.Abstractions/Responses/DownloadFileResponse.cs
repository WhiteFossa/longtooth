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
        [JsonIgnore]
        public IReadOnlyCollection<byte> FileContent { get; private set; }

        public DownloadFileResponse(DownloadedFileDto file, IReadOnlyCollection<byte> fileContent) : base(CommandType.DownloadFile)
        {
            File = file;
            FileContent = fileContent;

            if (File == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
        }

        public static DownloadFileResponse Parse(string header, IReadOnlyCollection<byte> payload)
        {
            _ = header ?? throw new ArgumentNullException(nameof(header));
            _ = payload ?? throw new ArgumentNullException(nameof(payload));

            var fileInfo = JsonSerializer.Deserialize<DownloadFileResponse>(header);
            return new DownloadFileResponse(fileInfo.File, payload);
        }
        public async override Task<ResponseRunResult> RunAsync()
        {
            var fileWithContent = new DownloadedFileWithContentDto(File.IsSuccessful, File.StartPosition, File.Length, FileContent);

            return new DownloadFileRunResult(fileWithContent);
        }

    }
}
