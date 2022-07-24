using longtooth.Common.Abstractions;
using longtooth.Common.Abstractions.DTOs.MessagesProtocol;
using longtooth.Common.Abstractions.Interfaces.DataCompressor;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using longtooth.Common.Implementations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace longtooth.Common.Implementations.MessagesProtocol
{
    public class MessagesProtocol : IMessagesProtocol
    {
        /// <summary>
        /// All messages must start with this GUID
        /// </summary>
        private static readonly Guid MessageBeginSignature = new Guid("c733cfea-6bd8-47d0-863a-b189bca260a5");

        /// <summary>
        /// Byte representation of MessageBeginSignature
        /// </summary>
        private readonly IReadOnlyCollection<byte> MessageBeginSignatureArray;

        private readonly IDataCompressor _dataCompressor;

        public MessagesProtocol(IDataCompressor dataCompressor)
        {
            MessageBeginSignatureArray = new List<byte>(MessageBeginSignature.ToByteArray());

            _dataCompressor = dataCompressor;
        }

        public byte[] GenerateMessage(byte[] message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));

            var compressedMessage = _dataCompressor.Compress(message);

            var resultSize = compressedMessage.Length + MessageBeginSignatureArray.Count + sizeof(int);

            if (resultSize > Constants.MaxPacketSize)
            {
                throw new ArgumentOutOfRangeException(nameof(message));
            }

            var result = new byte[resultSize];
            MessageBeginSignatureArray.ToArray().CopyTo(result, 0);
            BitConverter.GetBytes(compressedMessage.Length).CopyTo(result, MessageBeginSignatureArray.Count);
            compressedMessage.CopyTo(result, MessageBeginSignatureArray.Count + sizeof(int));

            return result;
        }

        public FirstMessageDto ExtractFirstMessage(byte[] buffer)
        {
            _ = buffer ?? throw new ArgumentNullException(nameof(buffer));

            int messageStartIndex = buffer.FindFirstSubarray(MessageBeginSignatureArray);

            if (messageStartIndex == -1)
            {
                return new FirstMessageDto(null, buffer);
            }

            int headerSize = MessageBeginSignatureArray.Count + sizeof(int);

            // Do we have size for message size?
            if (messageStartIndex + headerSize > buffer.Length)
            {
                return new FirstMessageDto(null, buffer);
            }

            int length = BitConverter.ToInt32(buffer.ToArray(), messageStartIndex + MessageBeginSignatureArray.Count);

            // Do we have full message?
            if (messageStartIndex + headerSize + length > buffer.Length)
            {
                return new FirstMessageDto(null, buffer);
            }

            int startIndex = messageStartIndex + headerSize;

            var compressedMessage = new byte[length];
            Array.Copy(buffer, startIndex, compressedMessage, 0, length);
            var message = _dataCompressor.Decompress(compressedMessage);

            // Removing message from buffer
            var remainingLength = startIndex + length;
            var newBufferLength = messageStartIndex + buffer.Length - remainingLength;
            var newBuffer = new byte[newBufferLength];

            Array.Copy(buffer, newBuffer, messageStartIndex);
            Array.Copy(buffer, remainingLength, newBuffer, messageStartIndex, buffer.Length - remainingLength);

            return new FirstMessageDto(message, newBuffer);
        }
    }
}
