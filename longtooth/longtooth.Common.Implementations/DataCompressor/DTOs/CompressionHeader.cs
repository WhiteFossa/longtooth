using longtooth.Common.Implementations.DataCompressor.Enums;

namespace longtooth.Common.Implementations.DataCompressor.DTOs
{
    /// <summary>
    /// Any compressed message starts with it
    /// </summary>
    public class CompressionHeader
    {
        /// <summary>
        /// Algorithm, used for compression
        /// </summary>
        public CompressionAlgorithm Algorithm { get; private set; }

        public CompressionHeader(CompressionAlgorithm algorithm)
        {
            Algorithm = algorithm;
        }
    }
}
