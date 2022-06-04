using System.Net.Sockets;

namespace longtooth.Server.Implementations.Business
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Size of receive buffer
        public const int BufferSize = 1024;

        // Receive buffer
        public byte[] Buffer = new byte[BufferSize];

        // Client socket.
        public Socket WorkSocket;
    }
}
