using System.Threading.Tasks;

namespace longtooth.Server.Abstractions.Interfaces
{
    /// <summary>
    /// Longtooth server
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Start server
        /// </summary>
        Task StartAsync();

    }
}
