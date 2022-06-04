using longtooth.Server.Abstractions.DTOs;
using longtooth.Server.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
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

        private OnNewDataReadDelegate _readCallback;
        private Socket _socket;
        private IPEndPoint _endpoint;
        private Thread _serverThread;

        private bool _needToStopServer;

        public async Task StartAsync(OnNewDataReadDelegate readCallback)
        {
            _readCallback = readCallback ?? throw new ArgumentNullException(nameof(readCallback));

            var listenIp = IPAddress.Any;
            _endpoint = new IPEndPoint(listenIp, ListenPort);

            _socket = new Socket(listenIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _needToStopServer = false;

            _serverThread = new Thread(new ThreadStart(ServerThreadRun));
            _serverThread.Start();
        }

        /// <summary>
        /// Server thread entry point
        /// </summary>
        private void ServerThreadRun()
        {
            try
            {
                _socket.Bind(_endpoint);
                _socket.Listen(ListenQueueSize);

                _allDone.Reset();

                _socket.BeginAccept(new AsyncCallback(AcceptCallback), _socket);

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

        public void ReadCallback(IAsyncResult result)
        {
            var state = result.AsyncState as StateObject;
            var socket = state.WorkSocket;

            int bytesRead = socket.EndReceive(result);

            if (bytesRead > 0)
            {
                var receivedData = new ReadDataDto(
                    bytesRead,
                    new List<byte>(state.Buffer).GetRange(0, bytesRead));

                _readCallback(receivedData);

                if (_needToStopServer)
                {
                    socket.Close();
                    _needToStopServer = false;
                    return;
                }

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

        public void Stop()
        {
            _ = _serverThread ?? throw new InvalidOperationException("Server isn't running");

            _needToStopServer = true;
        }
    }
}
