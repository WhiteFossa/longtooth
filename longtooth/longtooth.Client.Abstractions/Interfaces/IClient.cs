using longtooth.Client.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace longtooth.Client.Abstractions.Interfaces
{
    public interface IClient
    {
        /// <summary>
        /// Delegate, called when server responses to a message
        /// </summary>
        public delegate void OnResponseDelegate(IReadOnlyCollection<byte> response);

        /// <summary>
        /// Call this before any other operations
        /// </summary>
        void SetupResponseCallback(OnResponseDelegate responseCallback);

        /// <summary>
        /// Connect to server
        /// </summary>
        Task ConnectAsync(ConnectionDto connectionParams);

        /// <summary>
        /// Sends message to a server
        /// </summary>
        Task SendAsync(IReadOnlyCollection<byte> message);

        /// <summary>
        /// Disconnect from server (hard, forced)
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Disconnect from server (graceful)
        /// </summary>
        /// <returns></returns>
        Task DisconnectGracefullyAsync();
    }
}
