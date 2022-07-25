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

        private OnNewDataReadDelegate _readCallback;
        private Socket _socket;
        private IPEndPoint _endpoint;
        private Thread _serverThread;

        /// <summary>
        /// True if we need to stop server
        /// </summary>
        private bool _needToStopServer;

        /// <summary>
        /// True, if server is running
        /// </summary>
        private bool _isRunning;

        public async Task StartAsync(OnNewDataReadDelegate readCallback)
        {
            _readCallback = readCallback ?? throw new ArgumentNullException(nameof(readCallback));

            if (_isRunning)
            {
                throw new InvalidOperationException("Server already running!");
            }

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
                _socket.BeginAccept(new AsyncCallback(AcceptCallback), _socket);

                _isRunning = true;
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

            if (_needToStopServer)
            {
                // Server was stopped while we waited for connection
                KillServer();
                return;
            }

            var connectionSocket = socket.EndAccept(result);
            connectionSocket.ReceiveBufferSize = Constants.MaxPacketSize;
            connectionSocket.SendBufferSize = Constants.MaxPacketSize;

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
            try
            {
                var connectionState = result.AsyncState as ConnectionState;
                int bytesRead = connectionState.ClientSocket.EndReceive(result);

                if (bytesRead > 0)
                {
                    var callbackBuffer = new byte[bytesRead];
                    Array.Copy(connectionState.ReadBuffer, 0, callbackBuffer, 0, bytesRead);
                    var responseToClient = await _readCallback(callbackBuffer);

                    // Sending answer if needed
                    if (responseToClient.NeedToSendResponse)
                    {
                        connectionState.WriteAmount = responseToClient.Response.Length;
                        connectionState.WriteBufferOffset = 0;

                        Array.Copy(responseToClient.Response, connectionState.WriteBuffer, connectionState.WriteAmount);

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
                        KillServer();
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
            catch(SocketException)
            {
                // Client abruptly closed connection, we can do nothing with it
            }
            catch (ObjectDisposedException)
            {
                // Swallowing it, we can do nothing
            }
        }

        /// <summary>
        /// Called after sending data to cient
        /// </summary>
        private void SendCallback(IAsyncResult result)
        {
            try
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
            catch (SocketException)
            {
                // Client abruptly closed connection, we can do nothing with it
            }
            catch (ObjectDisposedException)
            {
                // Swallowing it, we can do nothing
            }
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Server isn't running");
            }

            _needToStopServer = true;
        }

        private void ShutdownClientConnection(Socket clientSocket)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private void KillServer()
        {
            _ = _socket ?? throw new InvalidOperationException("Server isn't running, main socket is null!");

            if (!_isRunning)
            {
                throw new InvalidOperationException("Server isn't running, _isRunning flag is not set!");
            }

            _socket.Close();
            _isRunning = false;
            _needToStopServer = false;
        }

        public IReadOnlyCollection<IPAddress> GetLocalIps()
        {
            var hostname = Dns.GetHostName();
            return Dns.GetHostAddresses(hostname);
        }
    }
}
