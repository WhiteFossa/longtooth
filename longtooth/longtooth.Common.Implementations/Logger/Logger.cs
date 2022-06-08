using longtooth.Common.Abstractions.Interfaces.Logger;
using System;
using System.Threading.Tasks;

namespace longtooth.Common.Implementations.Logger
{
    public class Logger : ILogger
    {
        private Action<string> _logFunc = str => Console.WriteLine(str);

        public async Task LogInfoAsync(string message)
        {
            await WriteMessage(message, "[{0}] Info: {1}");
        }

        public async Task LogWarningAsync(string message)
        {
            await WriteMessage(message, "[{0}] Warning: {1}");
        }

        public async Task LogErrorAsync(string message)
        {
            await WriteMessage(message, "[{0}] Error: {1}");
        }

        public void SetLoggingFunction(Action<string> logFunc)
        {
            _logFunc = logFunc ?? throw new ArgumentNullException(nameof(logFunc));
        }

        /// <summary>
        /// Writes message to log, using given template
        /// </summary>
        private async Task WriteMessage(string message, string template)
        {
            _logFunc(string.Format(template, DateTime.Now.ToLongTimeString(), message));
        }
    }
}
