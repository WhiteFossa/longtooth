using longtooth.Common.Abstractions.DTOs.MessagesProtocol;
using System.Collections.Generic;

namespace longtooth.Common.Abstractions.Interfaces.MessagesProtocol
{
    /// <summary>
    /// Interface to generate/decode low-level messages
    /// </summary>
    public interface IMessagesProtocol
    {
        /// <summary>
        /// Encode message. Encoded message is ready to be pushed to socket
        /// </summary>
        List<byte> GenerateMessage(List<byte> message);

        /// <summary>
        /// Searches buffer for the first message
        /// </summary>
        FirstMessageDto ExtractFirstMessage(List<byte> buffer);
    }
}
