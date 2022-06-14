using longtooth.Common.Abstractions.Enums;
using System;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class DownloadFileRunResult : ResponseRunResult
    {
        /// <summary>
        /// File metadata
        /// </summary>
        public DownloadedFileDto File { get; private set; }

        public DownloadFileRunResult(DownloadedFileDto file) : base(CommandType.DownloadFile)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
        }
    }
}
