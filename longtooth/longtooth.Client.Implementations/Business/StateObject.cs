using System.Net.Sockets;

namespace longtooth.Client.Implementations.Business
{
    /// <summary>
    /// State object for reading client data asynchronously
    /// </summary>
    public class StateObject
    {
        /// <summary>
        /// Client socket
        /// </summary>
        public Socket WorkSocket { get; set; }

        /// <summary>
        /// Receive buffer size
        /// </summary>
        public const int BufferSize = 1024;

        /// <summary>
        /// Receive buffer
        /// </summary>
        public byte[] Buffer = new byte[BufferSize];
    }
}
