using longtooth.Common.Abstractions.Interfaces.DataCompressor;
using longtooth.Common.Implementations.DataCompressor.DTOs;
using longtooth.Common.Implementations.DataCompressor.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace longtooth.Common.Implementations.DataCompressor
{
    public class DataCompressor : IDataCompressor
    {
        /// <summary>
        /// Algorith to use
        /// </summary>
        private const CompressionAlgorithm Algorithm = CompressionAlgorithm.Deflate;

        public IReadOnlyCollection<byte> Compress(IReadOnlyCollection<byte> dataToCompress)
        {
            var result = new List<byte>();

            var header = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<CompressionHeader>(new CompressionHeader(Algorithm)));

            // Header length
            var headerLength = header.Length;
            result.AddRange(BitConverter.GetBytes(headerLength));

            // Header
            result.AddRange(header);

            // Payload
            result.AddRange(CompressPayload(dataToCompress));

            return result;
        }

        private IReadOnlyCollection<byte> CompressPayload(IReadOnlyCollection<byte> payloadToCompress)
        {
            byte[] compressedPayload = null;

            using (var inputStream = new MemoryStream(payloadToCompress.ToArray(), true))
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
            // Header length
            var dataToDecompressAsList = new List<byte>(dataToDecompress.ToArray()); // TODO: Can we do it without copying?

            var headerLengthAsBytesList = dataToDecompressAsList.GetRange(0, sizeof(int));
            var headerLength = BitConverter.ToInt32(headerLengthAsBytesList.ToArray());

            var headerLengthPlusHeaderLength = sizeof(int) + headerLength;

            if (headerLengthPlusHeaderLength > dataToDecompress.Count)
            {
                throw new ArgumentException("To short data array for decompression!", nameof(dataToDecompress));
            }

            // Header
            var headerAsBytesList = dataToDecompressAsList.GetRange(sizeof(int), headerLength);
            var header = JsonSerializer.Deserialize<CompressionHeader>(Encoding.UTF8.GetString(headerAsBytesList.ToArray()));

            if (header == null || header.Algorithm != Algorithm)
            {
                throw new ArgumentException("Damaged header or unknown compression algorithm!", nameof(dataToDecompress));
            }

            // Payload
            var payloadLength = dataToDecompress.Count() - headerLengthPlusHeaderLength;
            var payload = dataToDecompressAsList.GetRange(headerLengthPlusHeaderLength, payloadLength);
            return DecompressPayload(payload);
        }

        private IReadOnlyCollection<byte> DecompressPayload(IReadOnlyCollection<byte> payloadToDecompress)
        {
            byte[] decompressedPayload = null;
            using (var output = new MemoryStream())
            using (var compressStream = new MemoryStream(payloadToDecompress.ToArray()))
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
