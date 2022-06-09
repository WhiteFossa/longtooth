using longtooth.Server.Abstractions.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace longtooth.Server.Abstractions.Interfaces
{
    /// <summary>
    /// Delegate, called when new data comes from client
    /// </summary>
    public delegate Task<ResponseDto> OnNewDataReadDelegate(List<byte> data);

    /// <summary>
    /// Longtooth server
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Start server
        /// </summary>
        Task StartAsync(OnNewDataReadDelegate readCallback);

        /// <summary>
        /// Stops server
        /// </summary>
        void Stop();

    }
}
