using System;
using System.Collections.Generic;
using System.Text;

namespace longtooth.Common.Abstractions.Interfaces.DataCompressor
{
    /// <summary>
    /// Interface to compress / decompress messages
    /// </summary>
    public interface IDataCompressor
    {
        /// <summary>
        /// Compress data and add compression header
        /// </summary>
        byte[] Compress(byte[] dataToCompress);

        /// <summary>
        /// Decompress data
        /// </summary>
        IReadOnlyCollection<byte> Decompress(IReadOnlyCollection<byte> dataToDecompress);
    }
}
