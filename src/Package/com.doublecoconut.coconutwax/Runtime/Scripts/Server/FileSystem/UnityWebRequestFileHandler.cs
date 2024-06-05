using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;

namespace Server.FileSystem
{
    /// <summary>
    /// Handles file operations using Unity's WebRequest.
    /// </summary>
    public class UnityWebRequestFileHandler : IFileHandler
    {
        /// <summary>
        /// Checks if a file exists at the given file path using a web request.
        /// </summary>
        /// <param name="filePath">The URL of the file.</param>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the file exists.</returns>
        public Task<bool> FileExistsAsync(string filePath, SynchronizationContext mainThreadContext)
        {
            var tcs = new TaskCompletionSource<bool>();

            mainThreadContext.Post(_ =>
            {
                Log($"[UnityWebRequestFileHandler] FileExistsAsync: {filePath}", LogType.Verbose);
                UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
                var operation = webRequest.SendWebRequest();
                Log($"[UnityWebRequestFileHandler] Send Web Request", LogType.Verbose);

                operation.completed += __ =>
                {
                    Log(
                        $"[UnityWebRequestFileHandler] Send Web Request Completed: {filePath}, Result: {webRequest.result}",
                        LogType.Verbose);
                    if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        webRequest.result == UnityWebRequest.Result.ProtocolError)
                    {
                        tcs.SetResult(false);
                    }
                    else
                    {
                        tcs.SetResult(true);
                    }

                    webRequest.Dispose();
                };
            }, null);

            return tcs.Task;
        }

        /// <summary>
        /// Reads a file at the given file path using a web request.
        /// </summary>
        /// <param name="filePath">The URL of the file.</param>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the contents of the file as a byte array.</returns>
        public Task<byte[]> ReadFileAsync(string filePath, SynchronizationContext mainThreadContext)
        {
            var tcs = new TaskCompletionSource<byte[]>();

            mainThreadContext.Post(_ =>
            {
                Log($"[UnityWebRequestFileHandler] ReadFileAsync: {filePath}", LogType.Verbose);

                UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
                var operation = webRequest.SendWebRequest();
                Log("[UnityWebRequestFileHandler] Send Web Request", LogType.Verbose);

                operation.completed += __ =>
                {
                    Log(
                        $"[UnityWebRequestFileHandler] Send Web Request Completed: {filePath}, Result: {webRequest.result}",
                        LogType.Verbose);

                    if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        webRequest.result == UnityWebRequest.Result.ProtocolError)
                    {
                        tcs.SetException(new Exception(webRequest.error));
                    }
                    else
                    {
                        tcs.SetResult(webRequest.downloadHandler.data);
                    }

                    webRequest.Dispose();
                };
            }, null);

            return tcs.Task;
        }
    }
}