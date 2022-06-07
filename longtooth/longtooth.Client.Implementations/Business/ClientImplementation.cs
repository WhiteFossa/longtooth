using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions;
using System.Net;
using System.Net.Sockets;
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
        /// Connect done event signal
        /// </summary>
        private ManualResetEvent _connectDone = new ManualResetEvent(false);

        /// <summary>
        /// Send done event signal
        /// </summary>
        private ManualResetEvent _sendDone = new ManualResetEvent(false);

        /// <summary>
        /// Receive done event signal
        /// </summary>
        private ManualResetEvent _receiveDone = new ManualResetEvent(false);

        /// <summary>
        /// Receive buffer
        /// </summary>
        public byte[] _buffer = new byte[Constants.MaxPacketSize];

        public async Task ConnectAsync(ConnectionDto connectionParams)
        {
            _ = connectionParams ?? throw new ArgumentNullException(nameof(connectionParams));

            var endpoint = new IPEndPoint(connectionParams.Address, (int)connectionParams.Port);

            _socket = new Socket(connectionParams.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _socket.BeginConnect(endpoint, new AsyncCallback(ConnectCallback), _socket);

            _connectDone.WaitOne();
        }

        /// <summary>
        /// Called when connection estailished
        /// </summary>
        private void ConnectCallback(IAsyncResult result)
        {
            var socket = result.AsyncState as Socket;

            socket.EndConnect(result);

            _connectDone.Set();
        }

        public async Task DisconnectAsync()
        {
            if (_socket == null || !_socket.Connected)
            {
                throw new InvalidOperationException("Client not connected to server");
            }

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public async Task SendAsync(List<byte> message, OnResponseDelegate responseCallback)
        {
            _responseHandler = responseCallback ?? throw new ArgumentNullException(nameof(responseCallback));

            _socket.BeginSend(
                message.ToArray(),
                0,
                message.Count,
                0,
                new AsyncCallback(SendCallback),
                null);
        }

        private void SendCallback(IAsyncResult result)
        {
            _socket.EndSend(result);
            _sendDone.Set();

            // Waiting for response
            _socket.BeginReceive(
                _buffer,
                0,
                _buffer.Length,
                0,
                new AsyncCallback(ReceiveCallback),
                null);
        }

        /// <summary>
        /// Called when data received
        /// </summary>
        private void ReceiveCallback(IAsyncResult result)
        {
            _ = _responseHandler ?? throw new ArgumentNullException(nameof(result));

            var readCount = _socket.EndReceive(result);

            _receiveDone.Set();

            // Delivering data to user
            _responseHandler(new List<byte>(_buffer).GetRange(0, readCount));

            // Continue to listen
            _socket.BeginReceive(
               _buffer,
               0,
               _buffer.Length,
               0,
               new AsyncCallback(ReceiveCallback),
               null);
        }
    }
}
