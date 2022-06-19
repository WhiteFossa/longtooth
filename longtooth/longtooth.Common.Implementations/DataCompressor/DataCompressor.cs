using longtooth.Common.Abstractions.Interfaces.DataCompressor;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace longtooth.Common.Implementations.DataCompressor
{
    public class DataCompressor : IDataCompressor
    {
        public IReadOnlyCollection<byte> Compress(IReadOnlyCollection<byte> dataToCompress)
        {
            byte[] compressedPayload = null;

            using (var inputStream = new MemoryStream(dataToCompress.ToArray(), true))
            using (var compressedStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(compressor);
                compressor.Close();

                compressedPayload = compressedStream.ToArray();
            }

            return new List<byte>(compressedPayload);
        }

        public IReadOnlyCollection<byte> Decompress(IReadOnlyCollection<byte> dataToDecompress)
        {
            byte[] decompressedPayload = null;
            using (var output = new MemoryStream())
            using (var compressStream = new MemoryStream(dataToDecompress.ToArray()))
            using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
            {
                decompressor.CopyTo(output);

                output.Seek(0, SeekOrigin.Begin);

                decompressedPayload = output.ToArray();
            }

            return decompressedPayload;
        }
    }
}
