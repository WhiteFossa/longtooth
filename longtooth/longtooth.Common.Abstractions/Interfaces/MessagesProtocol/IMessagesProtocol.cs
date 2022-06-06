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
        /// Searches buffer for the first message, and if message found - removes it from buffer and
        /// returns it. If buffer doesn't contain a message - returns null
        /// </summary>
        List<byte> ExtractFirstMessage(ref List<byte> buffer);
    }
}
