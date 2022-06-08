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
        /// Buffer for this connection (read)
        /// </summary>
        public byte[] ReadBuffer { get; private set; } = new byte[Constants.MaxPacketSize];

        /// <summary>
        /// How much data do we need to send?
        /// </summary>
        public int WriteAmount { get; set; }

        /// <summary>
        /// Position in write buffer
        /// </summary>
        public int WriteBufferOffset { get; set; }

        /// <summary>
        /// Buffer for this connection (write)
        /// </summary>
        public byte[] WriteBuffer { get; private set; } = new byte[Constants.MaxPacketSize];

        public ConnectionState(Socket clientSocket)
        {
            ClientSocket = clientSocket ?? throw new ArgumentNullException(nameof(clientSocket));
            ReadBuffer = new byte[Constants.MaxPacketSize];
        }
    }
}
