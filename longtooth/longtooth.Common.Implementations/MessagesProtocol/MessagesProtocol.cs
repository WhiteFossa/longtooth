using longtooth.Common.Abstractions;
using longtooth.Common.Abstractions.DTOs.MessagesProtocol;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using longtooth.Common.Implementations.Extensions;
using System;
using System.Collections.Generic;

namespace longtooth.Common.Implementations.MessagesProtocol
{
    public class MessagesProtocol : IMessagesProtocol
    {
        /// <summary>
        /// All messages must start with this GUID
        /// </summary>
        private static readonly Guid MessageBeginSignature = new Guid("c733cfea-6bd8-47d0-863a-b189bca260a5");

        /// <summary>
        /// All messages must end with this GUID
        /// </summary>
        private static readonly Guid MessageEndSignature = new Guid("3ae63b06-866e-43c2-9357-be92374e1050");

        /// <summary>
        /// Byte representation of MessageBeginSignature
        /// </summary>
        private readonly IReadOnlyCollection<byte> MessageBeginSignatureArray;

        /// <summary>
        /// Byte representation of MessageEndSignature
        /// </summary>
        private readonly IReadOnlyCollection<byte> MessageEndSignatureArray;

        public MessagesProtocol()
        {
            MessageBeginSignatureArray = new List<byte>(MessageBeginSignature.ToByteArray());
            MessageEndSignatureArray = new List<byte>(MessageEndSignature.ToByteArray());
        }

        public IReadOnlyCollection<byte> GenerateMessage(IReadOnlyCollection<byte> message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));

            if (message.Count + MessageBeginSignatureArray.Count + MessageEndSignatureArray.Count > Constants.MaxPacketSize)
            {
                throw new ArgumentOutOfRangeException(nameof(message));
            }

            var result = new List<byte>();

            result.AddRange(MessageBeginSignatureArray);
            result.AddRange(message);
            result.AddRange(MessageEndSignatureArray);

            return result;
        }

        public FirstMessageDto ExtractFirstMessage(IReadOnlyCollection<byte> buffer)
        {
            _ = buffer ?? throw new ArgumentNullException(nameof(buffer));

            // We must have both message begin and message end signatures, end must be later than begin
            var messageStartIndex = buffer.FindFirstSubarray(MessageBeginSignatureArray);

            if (messageStartIndex == -1)
            {
                return new FirstMessageDto(null, buffer);
            }

            var messageEndIndex = buffer.FindFirstSubarray(MessageEndSignatureArray);

            if (messageEndIndex == -1 || messageEndIndex <= messageStartIndex)
            {
                return new FirstMessageDto(null, buffer);
            }

            var startIndex = messageStartIndex + MessageBeginSignatureArray.Count;
            var length = messageEndIndex - messageStartIndex - MessageBeginSignatureArray.Count;

            var bufferAsList = new List<byte>(buffer);
            var result = bufferAsList.GetRange(startIndex, length);

            // Removing message from buffer
            var remainingLength = messageEndIndex + MessageEndSignatureArray.Count;
            var newBuffer = bufferAsList.GetRange(0, messageStartIndex);
            newBuffer.AddRange(bufferAsList.GetRange(remainingLength, buffer.Count - remainingLength));

            return new FirstMessageDto(result, newBuffer);
        }


    }
}
