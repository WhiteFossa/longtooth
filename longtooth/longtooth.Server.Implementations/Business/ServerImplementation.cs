using longtooth.Server.Abstractions.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace longtooth.Server.Implementations.Business
{
    public class ServerImplementation : IServer
    {
        /// <summary>
        /// Port where we are listening
        /// </summary>
        private const int ListenPort = 5934;

        /// <summary>
        /// Listen queue size (number of concurrent connections)
        /// </summary>
        private const int ListenQueueSize = 100;

        // Thread signal
        public static ManualResetEvent _allDone = new ManualResetEvent(false);

        public async Task StartAsync()
        {
            var listenIp = IPAddress.Any;
            var endPoint = new IPEndPoint(listenIp, ListenPort);

            var socket = new Socket(listenIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(endPoint);
                socket.Listen(ListenQueueSize);

                _allDone.Reset();

                socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                _allDone.WaitOne();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                throw new InvalidOperationException("Socket operation error", ex);
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            var socket = result.AsyncState as Socket;
            var handler = socket.EndAccept(result);

            _allDone.Set();

            var state = new StateObject();
            state.WorkSocket = handler;

            handler.BeginReceive(
                state.Buffer,
                0,
                StateObject.BufferSize,
                0,
                new AsyncCallback(ReadCallback),
                state);
        }

        public static void ReadCallback(IAsyncResult result)
        {
            var state = result.AsyncState as StateObject;
            var socket = state.WorkSocket;

            int bytesRead = socket.EndReceive(result);

            if (bytesRead > 0)
            {
                var message = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);

                Debug.WriteLine(message);

                // Continuing to listen
                socket.BeginReceive(
                    state.Buffer,
                    0,
                    StateObject.BufferSize,
                    0,
                    new AsyncCallback(ReadCallback),
                    state);
            }
        }
    }
}
