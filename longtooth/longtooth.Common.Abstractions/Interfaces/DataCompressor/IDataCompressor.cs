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
        IReadOnlyCollection<byte> Compress(IReadOnlyCollection<byte> dataToCompress);

        /// <summary>
        /// Decompress data
        /// </summary>
        IReadOnlyCollection<byte> Decompress(IReadOnlyCollection<byte> dataToDecompress);
    }
}
