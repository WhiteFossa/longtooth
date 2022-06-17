using System;
using System.Collections.Generic;

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
        public IReadOnlyCollection<byte> Header { get; private set; }

        /// <summary>
        /// Binary data size
        /// </summary>
        public int BinaryDataSize { get; private set; }

        /// <summary>
        /// Binary data
        /// </summary>
        public IReadOnlyCollection<byte> BinaryData { get; private set; }

        public Message(IReadOnlyCollection<byte> header, IReadOnlyCollection<byte> binaryData)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            HeaderSize = Header.Count;

            if (binaryData == null)
            {
                BinaryDataSize = 0;
                BinaryData = null;
            }
            else
            {
                BinaryDataSize = binaryData.Count;
                BinaryData = binaryData;
            }
        }

        public IReadOnlyCollection<byte> ToDataPacket()
        {
            var result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(HeaderSize));
            result.AddRange(Header);
            result.AddRange(BitConverter.GetBytes(BinaryDataSize));

            if (BinaryDataSize != 0)
            {
                result.AddRange(BinaryData);
            }

            return result.ToArray();
        }
    }
}
