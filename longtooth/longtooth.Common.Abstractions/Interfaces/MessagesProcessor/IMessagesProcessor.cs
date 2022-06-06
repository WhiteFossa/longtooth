﻿using System;
using System.Collections.Generic;
using System.Text;

namespace longtooth.Common.Abstractions.Interfaces.MessagesProcessor
{
    /// <summary>
    /// Will be called when new message is decoded
    /// </summary>
    public delegate void OnNewMessageDelegate(List<byte> decodedMessage);

    /// <summary>
    /// Interface to process low-level messages
    /// </summary>
    public interface IMessagesProcessor
    {
        /// <summary>
        /// Set-up handler for new messages (for use with OnNewMessageArrive).
        /// In case of OnNewMessageArriveServer there is no need to set-up this delegate
        /// </summary>
        void SetupOnNewMessageDelegate(OnNewMessageDelegate handler);

        /// <summary>
        /// Prepare message to be sent. Feed the result of this call to socket
        /// </summary>
        List<byte> PrepareMessageToSend(List<byte> messageToSend);

        /// <summary>
        /// Call this when new message comes from socket (for client, calls hanlder when full message is received)
        /// </summary>
        void OnNewMessageArrive(List<byte> newMessage);

        /// <summary>
        /// Like OnNewMessageArrive(), but don't call handler. Instead returns message when it received or null if
        /// message isn't completely arrived yet
        /// </summary>
        List<byte> OnNewMessageArriveServer(List<byte> newMessage);
    }
}
