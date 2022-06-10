using longtooth.Common.Abstractions.Enums;
using System;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    public class GetDirectoryContentRunResult : ResponseRunResult
    {
        /// <summary>
        /// Directory content
        /// </summary>
        public DirectoryContentDto DirectoryContent { get; private set; }

        public GetDirectoryContentRunResult(DirectoryContentDto directoryContent) : base(CommandType.GetDirectoryContent)
        {
            DirectoryContent = directoryContent ?? throw new ArgumentNullException(nameof(directoryContent));
        }
    }
}
