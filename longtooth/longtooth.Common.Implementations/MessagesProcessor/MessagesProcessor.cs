using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using System;

namespace longtooth.Common.Implementations.MessagesProcessor
{
    public class MessagesProcessor : IMessagesProcessor
    {
        private readonly IMessagesProtocol _messagesProtocol;

        private OnNewMessageDelegate _onNewMessage;

        /// <summary>
        /// Here we accumulate data from socket till we get full message
        /// </summary>
        private byte[] _accumulator = new byte[0];

        public MessagesProcessor(IMessagesProtocol messagesProtocol)
        {
            _messagesProtocol = messagesProtocol;
        }

        public void SetupOnNewMessageDelegate(OnNewMessageDelegate handler)
        {
            _onNewMessage = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public byte[] PrepareMessageToSend(byte[] messageToSend)
        {
            return _messagesProtocol.GenerateMessage(messageToSend);
        }

        public void OnNewMessageArrive(byte[] newMessage)
        {
            _ = _onNewMessage ?? throw new InvalidOperationException("Messages processor isn't set up.");
            _ = newMessage ?? throw new ArgumentNullException(nameof(newMessage));

            var newAccumulator = new byte[_accumulator.Length + newMessage.Length];
            _accumulator.CopyTo(newAccumulator, 0);
            newMessage.CopyTo(newAccumulator, _accumulator.Length);
            _accumulator = newAccumulator;

            while(true)
            {
                var decodedMessage = _messagesProtocol.ExtractFirstMessage(_accumulator);
                _accumulator = decodedMessage.NewBuffer;

                if (decodedMessage.Message == null)
                {
                    return; // We need more data
                }

                _onNewMessage(decodedMessage.Message);
            };
        }

        public byte[] OnNewMessageArriveServer(byte[] newMessage)
        {
            _ = newMessage ?? throw new ArgumentNullException(nameof(newMessage));

            var newAccumulator = new byte[_accumulator.Length + newMessage.Length];
            _accumulator.CopyTo(newAccumulator, 0);
            newMessage.CopyTo(newAccumulator, _accumulator.Length);
            _accumulator = newAccumulator;

            while (true)
            {
                var decodedMessage = _messagesProtocol.ExtractFirstMessage(_accumulator);
                _accumulator = decodedMessage.NewBuffer;

                if (decodedMessage.Message == null)
                {
                    return null; // We need more data
                }

                return decodedMessage.Message;
            };
        }
    }
}
