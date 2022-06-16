namespace longtooth.Abstractions.DTOs
{
    /// <summary>
    /// DTO with server IP
    /// </summary>
    public class ServerIpDto
    {
        /// <summary>
        /// Server IP
        /// </summary>
        public string Ip { get; private set; }

        public ServerIpDto(string ip)
        {
            Ip = ip;
        }
    }
}
