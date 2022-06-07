﻿using longtooth.Common.Abstractions;
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
        private const int ListenQueueSize = 10;

        // Thread signal
        private ManualResetEvent _allDone = new ManualResetEvent(false);

        private OnNewDataReadDelegate _readCallback;
        private Socket _socket;
        private IPEndPoint _endpoint;
        private Thread _serverThread;

        private bool _needToStopServer;

        /// <summary>
        /// Receive buffer
        /// </summary>
        public byte[] _buffer = new byte[Constants.MaxPacketSize];

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

            _allDone.Set();

            connectionSocket.BeginReceive(
                _buffer,
                0,
                _buffer.Length,
                0,
                new AsyncCallback(ReadCallback),
                connectionSocket);
        }

        public void ReadCallback(IAsyncResult result)
        {
            var connectionSocket = result.AsyncState as Socket;
            int bytesRead = connectionSocket.EndReceive(result);

            if (bytesRead > 0)
            {
                var responseToClient = _readCallback(new List<byte>(_buffer).GetRange(0, bytesRead));

                // Sending answer if needed
                if (responseToClient.NeedToSendResponse)
                {
                    connectionSocket.BeginSend(
                    responseToClient.Response.ToArray(),
                    0,
                    responseToClient.Response.Count,
                    0,
                    new AsyncCallback(SendCallback),
                    connectionSocket);
                }

                if (responseToClient.NeedToClose)
                {
                    connectionSocket.Close();
                    return;
                }

                if (_needToStopServer)
                {
                    _socket.Close();
                    _needToStopServer = false;
                    return;
                }

                // Continuing to listen
                connectionSocket.BeginReceive(
                    _buffer,
                    0,
                    _buffer.Length,
                    0,
                    new AsyncCallback(ReadCallback),
                    connectionSocket);
            }
        }

        /// <summary>
        /// Called after sending data to cient
        /// </summary>
        private void SendCallback(IAsyncResult result)
        {
            var connectionSocket = result.AsyncState as Socket;
            connectionSocket.EndSend(result);
        }

        public void Stop()
        {
            _ = _serverThread ?? throw new InvalidOperationException("Server isn't running");

            _needToStopServer = true;
        }
    }
}
