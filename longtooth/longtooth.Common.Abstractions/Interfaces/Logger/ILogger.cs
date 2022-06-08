using System;
using System.Threading.Tasks;

namespace longtooth.Common.Abstractions.Interfaces.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Set function, writing to log. If not set, then Console.WriteLine will be used
        /// </summary>
        void SetLoggingFunction(Action<string> logFunc);

        /// <summary>
        /// Log info-level event
        /// </summary>
        Task LogInfoAsync(string message);

        /// <summary>
        /// Log warning-level event
        /// </summary>
        Task LogWarningAsync(string message);

        /// <summary>
        /// Log error-level event
        /// </summary>
        Task LogErrorAsync(string message);
    }
}
