using longtooth.Common.Abstractions.Enums;

namespace longtooth.Common.Abstractions.DTOs.Responses
{
    /// <summary>
    /// Response run result
    /// </summary>
    public class ResponseRunResult
    {
        /// <summary>
        /// Response to this command was processed
        /// </summary>
        public CommandType ResponseToCommand { get; private set; }

        public ResponseRunResult(CommandType responseToCommand)
        {
            ResponseToCommand = responseToCommand;
        }
    }
}
