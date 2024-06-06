using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;

namespace Server.FileSystem
{
    /// <summary>
    /// Handles file operations using the file system.
    /// </summary>
    public class FileSystemFileHandler : IFileHandler
    {
        /// <summary>
        /// Checks if a file exists at the given file path.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the file exists.</returns>
        public Task<bool> FileExistsAsync(string filePath, SynchronizationContext mainThreadContext)
        {
            Log($"[FileSystemFileHandler] FileExistsAsync: {filePath}", LogType.Verbose);
            return Task.FromResult(File.Exists(filePath));
        }

        /// <summary>
        /// Reads a file at the given file path.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the contents of the file as a byte array.</returns>
        public async Task<byte[]> ReadFileAsync(string filePath, SynchronizationContext mainThreadContext)
        {
            Log($"[FileSystemFileHandler] ReadFileAsync: {filePath}", LogType.Verbose);
            return await Task.Run(() => File.ReadAllBytes(filePath));
        }
    }
}