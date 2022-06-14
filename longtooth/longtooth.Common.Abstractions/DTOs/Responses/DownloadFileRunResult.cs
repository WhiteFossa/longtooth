using longtooth.Common.Abstractions.Enums;
using System;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class DownloadFileRunResult : ResponseRunResult
    {
        /// <summary>
        /// File metadata
        /// </summary>
        public DownloadedFileWithContentDto File { get; private set; }

        public DownloadFileRunResult(DownloadedFileWithContentDto file) : base(CommandType.DownloadFile)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }
    }
}
