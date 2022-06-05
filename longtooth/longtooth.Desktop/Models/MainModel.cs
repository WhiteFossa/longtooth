using System.Net;

namespace longtooth.Desktop.Models
{
    /// <summary>
    /// Main model
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// Server IP
        /// </summary>
        public IPAddress ServerIp { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        public uint ServerPort { get; set; }
    }
}
