using longtooth.Common.Abstractions;
using longtooth.Server.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
        private const int ListenQueueSize = 10;

        // Thread signal
        private ManualResetEvent _allDone = new ManualResetEvent(false);

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
            var connectionSocket = socket.EndAccept(result);
            connectionSocket.ReceiveBufferSize = Constants.MaxPacketSize;
            connectionSocket.SendBufferSize = Constants.MaxPacketSize;

            _allDone.Set();

            // Waiting for data from client
            var connectionState = new ConnectionState(connectionSocket);
            connectionSocket.BeginReceive(
                connectionState.ReadBuffer,
                0,
                connectionState.ReadBuffer.Length,
                0,
                new AsyncCallback(ReadCallbackAsync),
                connectionState);

            // And starting to listen again
            socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

        }

        public async void ReadCallbackAsync(IAsyncResult result)
        {
            var connectionState = result.AsyncState as ConnectionState;
            int bytesRead = connectionState.ClientSocket.EndReceive(result);

            if (bytesRead > 0)
            {
                var responseToClient = await _readCallback(new List<byte>(connectionState.ReadBuffer).GetRange(0, bytesRead));

                // Sending answer if needed
                if (responseToClient.NeedToSendResponse)
                {
                    connectionState.WriteAmount = responseToClient.Response.Count;
                    connectionState.WriteBufferOffset = 0;

                    Array.Copy(responseToClient.Response.ToArray(), connectionState.WriteBuffer, connectionState.WriteAmount);

                    connectionState.ClientSocket.BeginSend(
                        connectionState.WriteBuffer,
                        0,
                        connectionState.WriteAmount,
                        0,
                        new AsyncCallback(SendCallback),
                        connectionState);
                }

                connectionState.IsShutdownRequired = responseToClient.NeedToClose;

                if (responseToClient.NeedToClose)
                {
                    connectionState.ClientSocket.Shutdown(SocketShutdown.Both);
                    connectionState.ClientSocket.Close();
                    return;
                }

                if (_needToStopServer)
                {
                    _socket.Close();
                    _needToStopServer = false;
                    return;
                }

                // Continuing to listen
                connectionState.ClientSocket.BeginReceive(
                    connectionState.ReadBuffer,
                    0,
                    connectionState.ReadBuffer.Length,
                    0,
                    new AsyncCallback(ReadCallbackAsync),
                    connectionState);
            }
        }

        /// <summary>
        /// Called after sending data to cient
        /// </summary>
        private void SendCallback(IAsyncResult result)
        {
            var connectionState = result.AsyncState as ConnectionState;

            var sentAmount = connectionState.ClientSocket.EndSend(result);

            connectionState.WriteBufferOffset += sentAmount;

            if (connectionState.WriteBufferOffset < connectionState.WriteAmount)
            {
                // Continuing transmission, we was unable to send data via one call
                connectionState.ClientSocket.BeginSend(
                        connectionState.WriteBuffer,
                        connectionState.WriteBufferOffset,
                        connectionState.WriteAmount,
                        0,
                        new AsyncCallback(SendCallback),
                        connectionState);
            }
            else
            {
                // Current transmission completed
                if (connectionState.IsShutdownRequired)
                {
                    ShutdownClientConnection(connectionState.ClientSocket);
                }
            }
        }

        public void Stop()
        {
            _ = _serverThread ?? throw new InvalidOperationException("Server isn't running");

            _needToStopServer = true;
        }

        private void ShutdownClientConnection(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}
