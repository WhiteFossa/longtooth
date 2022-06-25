using System;
using System.Collections.Generic;

namespace longtooth.Common.Abstractions.DTOs.ClientService
{
    /// <summary>
    /// Part of file content
    /// </summary>
    public class FileContentDto
    {
        /// <summary>
        /// Is file exist
        /// </summary>
        public bool IsExist { get; private set; }

        /// <summary>
        /// If required part of file is within the file?
        /// </summary>
        public bool IsInRagne { get; private set; }

        /// <summary>
        /// File content
        /// </summary>
        public IReadOnlyCollection<byte> Content { get; private set; }

        public FileContentDto(bool isExist, bool isInRagne, IReadOnlyCollection<byte> content)
        {
            IsExist = isExist;
            IsInRagne = isInRagne;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
