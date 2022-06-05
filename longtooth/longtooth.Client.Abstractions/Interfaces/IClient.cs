using longtooth.Client.Abstractions.DTOs;

namespace longtooth.Client.Abstractions.Interfaces
{
    public interface IClient
    {
        /// <summary>
        /// Delegate, called when server responses to a message
        /// </summary>
        public delegate void OnResponseDelegate(List<byte> response);

        /// <summary>
        /// Connect to server
        /// </summary>
        Task ConnectAsync(ConnectionDto connectionParams);

        /// <summary>
        /// Sends message to a server
        /// </summary>
        Task SendAsync(List<byte> message, OnResponseDelegate responseCallback);

        /// <summary>
        /// Disconnect from server
        /// </summary>
        Task DisconnectAsync();
    }
}
