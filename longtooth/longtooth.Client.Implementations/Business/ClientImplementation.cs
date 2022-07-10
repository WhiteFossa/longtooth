using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static longtooth.Client.Abstractions.Interfaces.IClient;

namespace longtooth.Client.Implementations.Business
{
    public class ClientImplementation : IClient
    {
        /// <summary>
        /// Connection socket
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// This delegate will be called when data from server received
        /// </summary>
        private OnResponseDelegate _responseHandler;

        /// <summary>
        /// Thread for reading messages in an infinite loop
        /// </summary>
        private Thread _readerThread;

        /// <summary>
        /// Buffer for getting inbound messages
        /// </summary>
        private byte[] _readBuffer = new byte[Constants.MaxPacketSize];

        /// <summary>
        /// If true, then next operation will cause disconnection
        /// </summary>
        private bool _isDisconnectRequired;

        public async Task ConnectAsync(ConnectionDto connectionParams)
        {
            if (_socket != null)
            {
                throw new InvalidOperationException("Can't connect because we are already connected!");
            }

            _ = connectionParams ?? throw new ArgumentNullException(nameof(connectionParams));

            _isDisconnectRequired = false;

            var endpoint = new IPEndPoint(connectionParams.Address, (int)connectionParams.Port);

            _socket = new Socket(connectionParams.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveBufferSize = Constants.MaxPacketSize;
            _socket.SendBufferSize = Constants.MaxPacketSize;

            _socket.Connect(endpoint);

            // Starting reader thread
            _readerThread = new Thread(new ThreadStart(ReaderThreadRun));
            _readerThread.IsBackground = true;
            _readerThread.Start();
        }

        public async Task DisconnectAsync()
        {
            _readerThread.Interrupt();

            _socket = null;
        }

        public async Task DisconnectGracefullyAsync()
        {
            _isDisconnectRequired = true;
        }

        public async Task SendAsync(byte[] message)
        {
            var totalSent = 0;

            while(totalSent < message.Length)
            {
                var bytesSent = _socket.Send(message, totalSent, message.Length - totalSent, SocketFlags.None);

                totalSent += bytesSent;
            }

            if (_isDisconnectRequired)
            {
                DoDisconnect();
                return;
            }
        }

        public void SetupResponseCallback(OnResponseDelegate responseCallback)
        {
            _responseHandler = responseCallback ?? throw new ArgumentNullException(nameof(responseCallback));
        }

        /// <summary>
        /// Entry point for reader thread
        /// </summary>
        private void ReaderThreadRun()
        {
            while (true)
            {
                try
                {
                    var bytesRead = _socket.Receive(_readBuffer);

                    if (_isDisconnectRequired)
                    {
                        DoDisconnect();
                        return;
                    }

                    if (_socket == null || !_socket.Connected || bytesRead == 0)
                    {
                        // Thread was killed during the disconnection
                        DoDisconnect();
                        return;
                    }

                    var result = new byte[bytesRead];
                    Array.Copy(_readBuffer, result, bytesRead);

                    _ = _responseHandler ?? throw new InvalidOperationException("Call SetupResponseCallback() first!");
                    _responseHandler(new List<byte>(result));
                }
                catch (SocketException)
                {
                    DoDisconnect();

                    return;
                }
            }
        }

        private void DoDisconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            _socket = null;
        }
    }
}
