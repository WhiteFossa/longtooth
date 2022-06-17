using longtooth.Protocol.Abstractions.DataStructures;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to parse server responses
    /// </summary>
    public interface IClientSideMessagesProcessor
    {
        /// <summary>
        /// Call this when new message arrive and call Run() of result
        /// </summary>
        ResponseHeader ParseMessage(IReadOnlyCollection<byte> message);
    }
}
