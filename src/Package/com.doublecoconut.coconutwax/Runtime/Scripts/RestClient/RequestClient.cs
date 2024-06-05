using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using UnityEngine;
using UnityEngine.Networking;

namespace RestClient
{
    /// <summary>
    /// Defines methods for sending requests and disposing resources.
    /// </summary>
    public interface IRequestClientSender : IDisposable
    {
        /// <summary>
        /// Sends a request and returns the response as a byte array.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response as a byte array.</returns>
        Task<byte[]> Send(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a request and returns the response as a Texture2D.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response as a Texture2D.</returns>
        Task<Texture2D> SendAsTexture(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Provides a fluent API for building and sending web requests. It implements the IRequestClientSender interface.
    /// This class allows you to set up a web request with various options such as URL, method, headers, and handlers.
    /// Once the request is built, it can be sent synchronously or asynchronously, and the response can be retrieved as a byte array or a Texture2D.
    /// </summary>
    public sealed class RequestClient : IRequestClientSender
    {
        /// <summary>
        /// Default headers for all requests.
        /// </summary>
        private static readonly Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>();

        /// <summary>
        /// Headers for the current request.
        /// </summary>
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        /// <summary>
        /// The URL of the request.
        /// </summary>
        private Uri _uri;

        /// <summary>
        /// The HTTP method of the request.
        /// </summary>
        private string _method;

        /// <summary>
        /// Indicates whether to ignore default headers.
        /// </summary>
        private bool _ignoreDefaultHeaders;

        /// <summary>
        /// The upload handler for the request.
        /// </summary>
        private UploadHandler _uploadHandler;

        /// <summary>
        /// The download handler for the request.
        /// </summary>
        private DownloadHandler _downloadHandler;

        /// <summary>
        /// The UnityWebRequest object for the request.
        /// </summary>
        private UnityWebRequest _request;

        /// <summary>
        /// The CancellationTokenRegistration object for the request.
        /// </summary>
        private CancellationTokenRegistration _cancellationTokenRegistration;


        /// <summary>
        /// Private constructor to prevent direct instantiation.
        /// </summary>
        private RequestClient()
        {
        }

        /// <summary>
        /// Sets a default header for all requests.
        /// </summary>
        public static void SetDefaultHeader(string key, string value)
        {
            _defaultHeaders[key] = value;
        }


        /// <summary>
        /// Creates a new RequestClient instance.
        /// </summary>
        public static RequestClient Create()
        {
            return new RequestClient();
        }

        /// <summary>
        /// Sets whether to ignore default headers.
        /// </summary>
        public RequestClient SetIgnoreDefaultHeaders(bool state)
        {
            _ignoreDefaultHeaders = state;
            return this;
        }

        /// <summary>
        /// Sets the URL and HTTP method of the request.
        /// </summary>
        public RequestClient SetUrl(string url, string method)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(method))
            {
                throw new Exception("Invalid url or method");
            }

            _uri = new Uri(url);
            _method = method;
            return this;
        }

        /// <summary>
        /// Sets a header for the request.
        /// </summary>
        public RequestClient SetHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                throw new Exception("Invalid header");
            }

            _headers[key] = value;
            return this;
        }

        /// <summary>
        /// Sets the upload handler for the request.
        /// </summary>
        public RequestClient SetUploadHandler(UploadHandler uploadHandler)
        {
            _uploadHandler = uploadHandler;
            return this;
        }

        /// <summary>
        /// Sets the download handler for the request.
        /// </summary>
        public RequestClient SetDownloadHandler(DownloadHandler downloadHandler)
        {
            _downloadHandler = downloadHandler;
            return this;
        }

        /// <summary>
        /// Builds the request.
        /// </summary>
        public IRequestClientSender BuildRequest()
        {
            if (_request != null)
            {
                throw new Exception("Request already built");
            }

            _request = new UnityWebRequest(_uri, _method);
            if (_method == UnityWebRequest.kHttpVerbPOST || _method == UnityWebRequest.kHttpVerbPUT)
            {
                if (_uploadHandler == null)
                {
                    throw new Exception("Upload handler not set");
                }

                _request.uploadHandler = _uploadHandler;
            }

            _request.downloadHandler = _downloadHandler ??= new DownloadHandlerBuffer();

            if (!_ignoreDefaultHeaders)
            {
                foreach (var header in _defaultHeaders)
                {
                    _request.SetRequestHeader(header.Key, header.Value);
                }
            }

            foreach (var header in _headers)
            {
                _request.SetRequestHeader(header.Key, header.Value);
            }

            return this;
        }

        /// <summary>
        /// Sends the request and returns the response as a byte array.
        /// </summary>
        public Task<byte[]> Send(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();

            CoconutWaxLogger.Log($"[{_request.method}], {_request.url}", Logger.LogType.Log);

            UnityWebRequestAsyncOperation operation = _request.SendWebRequest();
            _cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                if (operation != null)
                {
                    operation.completed -= OperationOnCompleted;
                    operation.webRequest?.Abort();
                }

                tcs.SetCanceled();
                ClearRequest();
            });
            operation.completed += OperationOnCompleted;

            void OperationOnCompleted(AsyncOperation _)
            {
                operation.completed -= OperationOnCompleted;

                if (_request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError ||
                    _request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    CoconutWaxLogger.Log($"[{_request.method}], {_request.url}: {_request.error}",
                        Logger.LogType.Error);
                    tcs.SetException(new Exception(_request.error));
                    ClearRequest();
                    return;
                }

                if (_request.result == UnityWebRequest.Result.InProgress)
                {
                    return;
                }

                CoconutWaxLogger.Log($"[{_request.method}], {_request.url}: {_request.downloadHandler.text}",
                    Logger.LogType.Log);
                tcs.SetResult(_request.downloadHandler.data);
                ClearRequest();
            }

            return tcs.Task;
        }

        /// <summary>
        /// Sends the request and returns the response as a Texture2D.
        /// </summary>
        public Task<Texture2D> SendAsTexture(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();

            CoconutWaxLogger.Log($"[{_request.method}], {_request.url}", Logger.LogType.Log);

            UnityWebRequestAsyncOperation operation = _request.SendWebRequest();
            _cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                if (operation != null)
                {
                    operation.completed -= OperationOnCompleted;
                    operation.webRequest?.Abort();
                }

                tcs.SetCanceled();
                ClearRequest();
            });
            operation.completed += OperationOnCompleted;

            void OperationOnCompleted(AsyncOperation _)
            {
                operation.completed -= OperationOnCompleted;

                if (_request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError ||
                    _request.result == UnityWebRequest.Result.DataProcessingError)
                {
                    CoconutWaxLogger.Log($"[{_request.method}], {_request.url}: {_request.error}",
                        Logger.LogType.Error);
                    tcs.SetException(new Exception(_request.error));
                    ClearRequest();
                    return;
                }

                if (_request.result == UnityWebRequest.Result.InProgress)
                {
                    return;
                }

                CoconutWaxLogger.Log($"[{_request.method}], {_request.url}: [TEXTURE2D]", Logger.LogType.Log);
                tcs.SetResult(((DownloadHandlerTexture)_request.downloadHandler).texture);
                ClearRequest();
            }

            return tcs.Task;
        }

        /// <summary>
        /// Clears the request.
        /// </summary>
        private void ClearRequest()
        {
            _cancellationTokenRegistration.Dispose();
            _request?.Dispose();
            _request = null;
        }

        /// <summary>
        /// Disposes the request and its handlers.
        /// </summary>
        public void Dispose()
        {
            _uploadHandler?.Dispose();
            _downloadHandler?.Dispose();
            ClearRequest();
        }
    }
}