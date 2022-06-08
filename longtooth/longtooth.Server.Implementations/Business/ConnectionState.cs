using longtooth.Common.Abstractions;
using System;
using System.Net.Sockets;

namespace longtooth.Server.Implementations.Business
{
    /// <summary>
    /// Class to store client connection
    /// </summary>
    public class ConnectionState
    {
        /// <summary>
        /// Socket for this connection
        /// </summary>
        public Socket ClientSocket { get; private set; }

        /// <summary>
        /// Buffer for this connection
        /// </summary>
        public byte[] Buffer { get; private set; } = new byte[Constants.MaxPacketSize];

        public ConnectionState(Socket clientSocket)
        {
            ClientSocket = clientSocket ?? throw new ArgumentNullException(nameof(clientSocket));
            Buffer = new byte[Constants.MaxPacketSize];
        }
    }
}
