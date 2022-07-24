using System;
using System.Collections.Generic;
using System.Linq;

namespace longtooth.Protocol.Abstractions.DataStructures
{
    /// <summary>
    /// Message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Header size
        /// </summary>
        public int HeaderSize { get;private set; }

        /// <summary>
        /// Header itself
        /// </summary>
        public byte[] Header { get; private set; }

        /// <summary>
        /// Binary data size
        /// </summary>
        public int BinaryDataSize { get; private set; }

        /// <summary>
        /// Binary data
        /// </summary>
        public byte[] BinaryData { get; private set; }

        public Message(byte[] header, byte[] binaryData)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            HeaderSize = Header.Length;

            if (binaryData == null)
            {
                BinaryDataSize = 0;
                BinaryData = null;
            }
            else
            {
                BinaryDataSize = binaryData.Length;
                BinaryData = binaryData;
            }
        }

        public byte[] ToDataPacket()
        {
            var resultSize = 2 * sizeof(int) + HeaderSize + BinaryDataSize;
            var result = new byte[resultSize];

            BitConverter.GetBytes(HeaderSize).CopyTo(result, 0); // Header size

            Array.Copy(Header, 0, result, sizeof(int), Header.Length); // Header

            BitConverter.GetBytes(BinaryDataSize).CopyTo(result, sizeof(int) + HeaderSize); // Binary data size

            if (BinaryDataSize != 0)
            {
                Array.Copy(BinaryData, 0, result, 2 * sizeof(int) + HeaderSize, BinaryData.Length); // Binary data
            }

            return result;
        }
    }
}
