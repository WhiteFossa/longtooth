using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using System;
using System.Collections.Generic;

namespace longtooth.Common.Implementations.MessagesProcessor
{
    public class MessagesProcessor : IMessagesProcessor
    {
        private readonly IMessagesProtocol _messagesProtocol;

        private OnNewMessageDelegate _onNewMessage;

        /// <summary>
        /// Here we accumulate data from socket till we get full message
        /// </summary>
        private List<byte> _accumulator = new List<byte>();

        public MessagesProcessor(IMessagesProtocol messagesProtocol)
        {
            _messagesProtocol = messagesProtocol;
        }

        public void SetupOnNewMessageDelegate(OnNewMessageDelegate handler)
        {
            _onNewMessage = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public List<byte> PrepareMessageToSend(List<byte> messageToSend)
        {
            return _messagesProtocol.GenerateMessage(messageToSend);
        }

        public void OnNewMessageArrive(List<byte> newMessage)
        {
            _ = _onNewMessage ?? throw new InvalidOperationException("Messages processor isn't set up.");
            _ = newMessage ?? throw new ArgumentNullException(nameof(newMessage));

            _accumulator.AddRange(newMessage);

            while(true)
            {
                var decodedMessage = _messagesProtocol.ExtractFirstMessage(ref _accumulator);

                if (decodedMessage == null)
                {
                    return; // We need more data
                }

                _onNewMessage(decodedMessage);
            };
        }

        public List<byte> OnNewMessageArriveServer(List<byte> newMessage)
        {
            _ = newMessage ?? throw new ArgumentNullException(nameof(newMessage));

            _accumulator.AddRange(newMessage);

            while (true)
            {
                var decodedMessage = _messagesProtocol.ExtractFirstMessage(ref _accumulator);

                if (decodedMessage == null)
                {
                    return null; // We need more data
                }

                return decodedMessage;
            };
        }
    }
}
