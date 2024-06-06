using System;

namespace Logger
{
    /// <summary>
    /// Specifies the type of log messages.
    /// </summary>
    [Flags]
    public enum LogType
    {
        /// <summary>
        /// No logging.
        /// </summary>
        None = 0,
    
        /// <summary>
        /// Regular log messages.
        /// </summary>
        Log = 1,
    
        /// <summary>
        /// Warning messages.
        /// </summary>
        Warning = 2,
    
        /// <summary>
        /// Error messages.
        /// </summary>
        Error = 4,
    
        /// <summary>
        /// Verbose messages, including all log types.
        /// </summary>
        Verbose = Log | Warning | Error
    }
}