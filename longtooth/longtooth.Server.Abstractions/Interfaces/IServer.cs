using longtooth.Server.Abstractions.DTOs;
using System.Threading.Tasks;

namespace longtooth.Server.Abstractions.Interfaces
{
    /// <summary>
    /// Delegate, called when new data comes from client
    /// </summary>
    public delegate ResponseDto OnNewDataReadDelegate(ReadDataDto data);

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
