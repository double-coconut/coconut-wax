using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.FileSystem;
using UnityEngine;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;

namespace Server
{
    /// <summary>
    /// Represents a local HTTP server.
    /// </summary>
    public class LocalServer : IDisposable
    {
        /// <summary>
        /// The listener for HTTP requests.
        /// </summary>
        private readonly HttpListener _listener;

        /// <summary>
        /// The main thread context.
        /// </summary>
        private readonly SynchronizationContext _mainThreadContext;

        /// <summary>
        /// The handler for file operations.
        /// </summary>
        private readonly IFileHandler _fileHandler;

        /// <summary>
        /// The thread running the server.
        /// </summary>
        private Thread _serverThread;

        /// <summary>
        /// Indicates whether the server is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// The base directory of the server.
        /// </summary>
        private readonly string _baseDirectory;

        /// <summary>
        /// The URL of the server.
        /// </summary>
        public readonly string Url;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalServer"/> class.
        /// </summary>
        /// <param name="mainThreadContext">The main thread context.</param>
        /// <param name="host">The host of the server.</param>
        /// <param name="port">The port of the server.</param>
        public LocalServer(SynchronizationContext mainThreadContext, string host = "http://+", uint port = 8080)
        {
            _mainThreadContext = mainThreadContext ?? throw new ArgumentNullException(nameof(mainThreadContext));
            _baseDirectory = Path.Combine(Application.streamingAssetsPath, "CoconutWaxWeb");
            _listener = new HttpListener();
            Url = $"{host}:{port}/";
            _listener.Prefixes.Add(Url);
            _fileHandler = FileHandlerFactory.CreateFileHandler();
            Log($"[Local Server] Preparing: Url: {Url}, File handler: {_fileHandler.GetType()}", LogType.Log);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            if (IsRunning)
            {
                Log("[Local Server] Server is already running.", LogType.Warning);
                return;
            }

            IsRunning = true;
            _serverThread = new Thread(async () => await Run());
            _serverThread.Start();
            Log("[Local Server] Started!", LogType.Log);
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                Log("[Local Server] Server is not running.", LogType.Warning);
                return;
            }

            IsRunning = false;
            _listener.Stop();
            _serverThread.Join();
            Log("[Local Server] Stopped!", LogType.Log);
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        private async Task Run()
        {
            _listener.Start();
            Log($"[Local Server] Thread Running!: Thread ID: {Thread.CurrentThread.ManagedThreadId}",
                LogType.Log);

            while (IsRunning)
            {
                try
                {
                    Log("[Local Server] Waiting for request...", LogType.Log);
                    var context = await _listener.GetContextAsync();
                    _ = ProcessRequestAsync(context);
                }
                catch (HttpListenerException) when (!IsRunning)
                {
                    // Expected exception when stopping the server
                }
                catch (Exception ex)
                {
                    Log($"[Local Server] {ex.Message}", LogType.Error);
                }
            }

            _listener.Close();
            Log($"[Local Server] Running Thread stopped!: Thread ID: {Thread.CurrentThread.ManagedThreadId}",
                LogType.Log);
        }

        /// <summary>
        /// Processes an incoming request.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Log($"[Local Server] Got request...: Path: {request.Url.AbsolutePath}, Method: {request.HttpMethod}",
                LogType.Log);

            if (request.HttpMethod.ToLower() != "get")
            {
                Log($"[Local Server] Method not allowed: {request.HttpMethod}", LogType.Warning);
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.OutputStream.Close();
                return;
            }

            string filePath = GetFilePath(request.Url.AbsolutePath);
            Log($"[Local Server] File: {filePath}", LogType.Log);

            var fileExists = await _fileHandler.FileExistsAsync(filePath, _mainThreadContext);
            Log($"[Local Server] File Exists: {fileExists}", LogType.Log);
            if (fileExists)
            {
                try
                {
                    string contentType = GetContentType(filePath);
                    Log($"[Local Server] Request Content Type: {contentType}", LogType.Log);
                    byte[] buffer = await _fileHandler.ReadFileAsync(filePath, _mainThreadContext);
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = contentType;
                    Log($"[Local Server] Writing response...: Buffer Length: {buffer.Length}", LogType.Log);
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch (OperationCanceledException)
                {
                    // Handle the cancellation exception if needed
                    Log($"[Local Server] Request canceled: {request.Url.AbsolutePath}", LogType.Log);
                }
            }
            else
            {
                Log($"[Local Server] File not found: {filePath}, Url: {request.Url.AbsolutePath}",
                    LogType.Error);
                response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("404 - Not Found");
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }

            response.OutputStream.Close();
            Log($"[Local Server] Response sent: {request.Url.AbsolutePath}, Status Code: {response.StatusCode}",
                LogType.Log);
        }

        /// <summary>
        /// Gets the file path for a given URL path.
        /// </summary>
        /// <param name="urlPath">The URL path.</param>
        /// <returns>The file path.</returns>
        private string GetFilePath(string urlPath)
        {
            if (urlPath == "/")
            {
                return $"{_baseDirectory}/index.html";
            }

            string relativePath = urlPath.TrimStart('/');
            return $"{_baseDirectory}/{relativePath}";
        }

        /// <summary>
        /// Gets the content type for a given file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The content type.</returns>
        private string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }

        /// <summary>
        /// Disposes the server, freeing up any resources it is holding.
        /// </summary>
        public void Dispose()
        {
            if (IsRunning)
            {
                Stop();
            }

            _listener?.Close();
        }
    }
}