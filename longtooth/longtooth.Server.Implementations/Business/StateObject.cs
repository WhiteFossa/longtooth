using longtooth.Common.Abstractions;
using System.Net.Sockets;

namespace longtooth.Server.Implementations.Business
{
    /// <summary>
    /// State object for reading client data asynchronously
    /// </summary>
    public class StateObject
    {
        /// <summary>
        /// Read data buffer size
        /// </summary>
        public const int BufferSize = Constants.MaxPacketSize;

        /// <summary>
        /// Read data buffer
        /// </summary>
        public byte[] Buffer = new byte[BufferSize];

        /// <summary>
        /// Server socket
        /// </summary>
        public Socket WorkSocket;
    }
}
