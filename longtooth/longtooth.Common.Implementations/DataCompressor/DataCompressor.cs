﻿using longtooth.Common.Abstractions.Interfaces.DataCompressor;
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

        public byte[] Compress(byte[] dataToCompress)
        {
            var header = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<CompressionHeader>(new CompressionHeader(Algorithm)));

            var headerLength = header.Length;
            var headerLengthAsArray = BitConverter.GetBytes(headerLength);

            var compressedPayload = CompressPayload(dataToCompress);

            var result = new byte[sizeof(int) + headerLength + compressedPayload.Length];
            headerLengthAsArray.CopyTo(result, 0); // Header length
            header.CopyTo(result, sizeof(int)); // Header
            compressedPayload.CopyTo(result, sizeof(int) + headerLength); // Payload

            return result;
        }

        private byte[] CompressPayload(byte[] payloadToCompress)
        {
            using (var inputStream = new MemoryStream(payloadToCompress, true))
            using (var compressedStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(compressor);
                compressor.Close();

                return compressedStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] dataToDecompress)
        {
            // Header length
            var headerLengthBytes = new byte[sizeof(int)];
            Array.Copy(dataToDecompress, 0, headerLengthBytes, 0, sizeof(int));
            var headerLength = BitConverter.ToInt32(headerLengthBytes);

            var headerLengthPlusHeaderLength = sizeof(int) + headerLength;

            if (headerLengthPlusHeaderLength > dataToDecompress.Length)
            {
                throw new ArgumentException("To short data array for decompression!", nameof(dataToDecompress));
            }

            // Header
            var headerBytes = new byte[headerLength];
            Array.Copy(dataToDecompress, sizeof(int), headerBytes, 0, headerLength);
            var header = JsonSerializer.Deserialize<CompressionHeader>(Encoding.UTF8.GetString(headerBytes));

            if (header == null || header.Algorithm != Algorithm)
            {
                throw new ArgumentException("Damaged header or unknown compression algorithm!", nameof(dataToDecompress));
            }

            // Payload
            var payloadLength = dataToDecompress.Count() - headerLengthPlusHeaderLength;
            var payloadBytes = new byte[payloadLength];
            Array.Copy(dataToDecompress, headerLengthPlusHeaderLength, payloadBytes, 0, payloadLength);
            return DecompressPayload(payloadBytes);
        }

        private byte[] DecompressPayload(byte[] payloadToDecompress)
        {
            byte[] decompressedPayload = null;
            using (var output = new MemoryStream())
            using (var compressStream = new MemoryStream(payloadToDecompress))
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
