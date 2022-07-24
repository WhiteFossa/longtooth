using System.Collections.Generic;

namespace longtooth.Common.Abstractions.DTOs.MessagesProtocol
{
    /// <summary>
    /// Information about the first message, extracted from buffer
    /// </summary>
    public class FirstMessageDto
    {
        /// <summary>
        /// Extracted message (can be null, if buffer contain no messages)
        /// </summary>
        public byte[] Message { get; private set; }

        /// <summary>
        /// Buffer without message (i.e. message is cut from buffer)
        /// </summary>
        public byte[] NewBuffer { get; private set; }

        public FirstMessageDto(byte[] message, byte[] newBuffer)
        {
            Message = message;
            NewBuffer = newBuffer;
        }
    }
}
