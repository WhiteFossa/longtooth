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
        IReadOnlyCollection<byte> GenerateMessage(IReadOnlyCollection<byte> message);

        /// <summary>
        /// Searches buffer for the first message
        /// </summary>
        FirstMessageDto ExtractFirstMessage(IReadOnlyCollection<byte> buffer);
    }
}
