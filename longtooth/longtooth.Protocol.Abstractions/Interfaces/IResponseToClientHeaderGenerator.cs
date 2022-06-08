namespace longtooth.Protocol.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to generate responses to client
    /// </summary>
    public interface IResponseToClientHeaderGenerator
    {
        /// <summary>
        /// Generates response to ping command
        /// </summary>
        byte[] GeneratePingResponse();
    }
}
