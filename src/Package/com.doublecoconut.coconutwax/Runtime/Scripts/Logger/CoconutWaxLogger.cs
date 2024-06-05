using System;
using UnityEngine;

namespace Logger
{
    /// <summary>
    /// Provides methods for logging messages with different log types.
    /// </summary>
    public static class CoconutWaxLogger
    {
        /// <summary>
        /// The prefix for each log message.
        /// </summary>
        private const string Prefix = "[CoconutWax]";

        /// <summary>
        /// The current log type settings.
        /// </summary>
        private static LogType _currentLogType = LogType.Verbose; // Default log type to Verbose to log all messages

        /// <summary>
        /// Sets the current log type.
        /// </summary>
        /// <param name="logType">The log type to be set.</param>
        public static void SetLogType(LogType logType)
        {
            _currentLogType = logType;
        }


        /// <summary>
        /// Logs a message if the specified log type is enabled.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logType">The type of log message.</param>
        public static void Log(string message, LogType logType)
        {
            if ((_currentLogType & logType) == 0) return;
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log($"{Prefix} [LOG]: {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{Prefix} [WARNING]: {message}");
                    break;
                case LogType.Error:
                    Debug.LogError($"{Prefix} [ERROR]: {message}");
                    break;
                case LogType.Verbose:
                    Debug.Log($"{Prefix} [VERBOSE]: {message}");
                    break;
                case LogType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}