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
        /// If true, then response will be sent to client, otherwise connection will be closed
        /// </summary>
        public bool NeedToSendResponse { get; private set; }

        /// <summary>
        /// Response to a client
        /// </summary>
        public List<byte> Response { get; private set; }

        public ResponseDto(bool needToSendResponse, List<byte> response)
        {
            NeedToSendResponse = needToSendResponse;
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }
    }
}
