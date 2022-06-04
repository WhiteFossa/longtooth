using System.Collections.Generic;

namespace longtooth.Server.Abstractions.DTOs
{
    /// <summary>
    /// Here we store data, read from client
    /// </summary>
    public class ReadDataDto
    {
        /// <summary>
        /// Data amount
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Actual data
        /// </summary>
        public List<byte> Data { get; private set; }

        public ReadDataDto(int size, List<byte> data)
        {
            Size = size;
            Data = data;
        }
    }
}
