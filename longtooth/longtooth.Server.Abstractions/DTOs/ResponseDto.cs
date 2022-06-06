using System;
using System.Collections.Generic;

namespace longtooth.Server.Abstractions.DTOs
{
    /// <summary>
    /// Response to request
    /// </summary>
    public class ResponseDto
    {
        /// <summary>
        /// If true, then response will be sent to client
        /// </summary>
        public bool NeedToSendResponse { get; private set; }

        /// <summary>
        /// If true, then server will close connection
        /// </summary>
        public bool NeedToClose { get; private set; }

        /// <summary>
        /// Response to a client
        /// </summary>
        public List<byte> Response { get; private set; }

        public ResponseDto(bool needToSendResponse, bool needToClose, List<byte> response)
        {
            NeedToSendResponse = needToSendResponse;
            NeedToClose = needToClose;
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }
    }
}
