using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using longtooth.Client.Abstractions.Interfaces;
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
                _socket);
        }

        private void SendCallback(IAsyncResult result)
        {
            var socket = result.AsyncState as Socket;

            socket.EndSend(result);
            _sendDone.Set();

            // Waiting for response
            var state = new StateObject();
            state.WorkSocket = socket;

            _socket.BeginReceive(
                state.Buffer,
                0,
                StateObject.BufferSize,
                0,
                new AsyncCallback(ReceiveCallback),
                state);
        }

        /// <summary>
        /// Called when data received
        /// </summary>
        private void ReceiveCallback(IAsyncResult result)
        {
            var state = result.AsyncState as StateObject;
            var socket = state.WorkSocket;

            var readCount = socket.EndReceive(result);

            _receiveDone.Set();

            _ = _responseHandler ?? throw new ArgumentNullException(nameof(result));
            _responseHandler(new List<byte>(state.Buffer).GetRange(0, readCount));
        }
    }
}
