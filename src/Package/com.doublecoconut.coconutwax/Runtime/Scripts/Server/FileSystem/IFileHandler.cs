using System.Threading;
using System.Threading.Tasks;

namespace Server.FileSystem
{
    /// <summary>
    /// Defines methods for handling file operations asynchronously.
    /// </summary>
    public interface IFileHandler
    {
        /// <summary>
        /// Checks if a file exists at the given file path.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the file exists.</returns>
        Task<bool> FileExistsAsync(string filePath, SynchronizationContext mainThreadContext);

        /// <summary>
        /// Reads a file at the given file path.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the contents of the file as a byte array.</returns>
        Task<byte[]> ReadFileAsync(string filePath, SynchronizationContext mainThreadContext);
    }
}