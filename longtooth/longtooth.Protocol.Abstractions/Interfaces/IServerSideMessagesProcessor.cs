using longtooth.Server.Abstractions.DTOs;
using System.Collections.Generic;

namespace longtooth.Protocol.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to parse client's messages (on server)
    /// </summary>
    public interface IServerSideMessagesProcessor
    {
        /// <summary>
        /// Call this when new message arrive
        /// </summary>
        ResponseDto ParseMessage(List<byte> message);
    }
}
