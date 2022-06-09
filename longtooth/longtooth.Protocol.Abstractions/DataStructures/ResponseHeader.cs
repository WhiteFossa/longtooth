using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace longtooth.Protocol.Abstractions.DataStructures
{
    /// <summary>
    /// Common header for all responses from server
    /// </summary>
    public class ResponseHeader
    {
        /// <summary>
        /// Response to this command
        /// </summary>
        [JsonPropertyName("Command")]
        public CommandType Command { get; set; }

        public ResponseHeader(CommandType command)
        {
            Command = command;
        }

        /// <summary>
        /// Implement me in children
        /// </summary>
        public virtual async Task<ResponseRunResult> RunAsync()
        {
            throw new InvalidOperationException("Don't call me, call my children!");
        }
    }
}
