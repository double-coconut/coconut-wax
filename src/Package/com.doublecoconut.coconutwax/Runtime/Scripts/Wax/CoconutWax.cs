using System;
using System.Threading;
using System.Threading.Tasks;
using Server;
using Wax.Exceptions;
using Wax.Payload;
#if UNIWEBVIEW
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;
#endif

namespace Wax
{
    /// <summary>
    /// The CoconutWax class is responsible for managing the local server and handling the operations.
    /// </summary>
    public sealed class CoconutWax : IDisposable
    {
        public const string DefaultWaxTokenContract = "eosio.token";
        
        private LocalServer localServer;
        private readonly uint _port;
        private string _currentUserAgent;

        /// <summary>
        /// Initializes a new instance of the CoconutWax class with the specified port.
        /// </summary>
        /// <param name="port">The port number for the local server.</param>
        /// <param name="userAgent">The user agent for UniWebView.</param>
        public CoconutWax(uint port = 2023, string userAgent = null)
        {
            _port = port;
            _currentUserAgent = userAgent;
        }


        /// <summary>
        /// Authenticates the user with the specified token contracts.
        /// </summary>
        /// <param name="tokenContracts">The token contracts for authentication.</param>
        /// <returns>The authentication payload data.</returns>
        public async Task<AuthenticationPayloadData> Authenticate(params string[] tokenContracts)
        {
            if (tokenContracts == null || tokenContracts.Length == 0)
            {
                tokenContracts = new[] { DefaultWaxTokenContract };
            }

            try
            {
                string route = $"#authenticate?tokenContracts={string.Join(",", tokenContracts)}";
                RunServer();
                AuthenticationPayloadData payloadData = await ShowWebView<AuthenticationPayloadData>(route);
                return payloadData;
            }
            finally
            {
                localServer.Stop();
            }
        }

        /// <summary>
        /// Transfers the NFT to the specified account.
        /// </summary>
        /// <param name="toAccount">The account to transfer the NFT to.</param>
        /// <param name="assetIds">The asset IDs of the NFTs to transfer.</param>
        /// <param name="memo">The memo for the transfer.</param>
        /// <returns>The transfer NFT payload data.</returns>
        public async Task<TransferNFTPayloadData> TransferNFT(string toAccount, string[] assetIds, string memo)
        {
            try
            {
                string route =
                    $"#transferNFT?toAccount={toAccount}&assetIds={string.Join(",", assetIds)}&memoContent={memo}";
                RunServer();
                TransferNFTPayloadData payloadData = await ShowWebView<TransferNFTPayloadData>(route);
                return payloadData;
            }
            finally
            {
                localServer?.Stop();
            }
        }

        /// <summary>
        /// Transfers the token to the specified account.
        /// </summary>
        /// <param name="toAccount">The account to transfer the token to.</param>
        /// <param name="amount">The amount of the token to transfer.</param>
        /// <param name="tokenContract">The token contract for the transfer.</param>
        /// <param name="symbol">The symbol of the token.</param>
        /// <param name="memo">The memo for the transfer.</param>
        /// <returns>The transfer token payload data.</returns>
        public async Task<TransferTokenPayloadData> TransferToken(string toAccount, float amount, string tokenContract,
            string symbol, string memo)
        {
            try
            {
                string route =
                    $"#transferToken?toAccount={toAccount}&amount={amount}&tokenContract={tokenContract}&symbol={symbol}&memoContent={memo}";
                RunServer();
                TransferTokenPayloadData payloadData = await ShowWebView<TransferTokenPayloadData>(route);
                return payloadData;
            }
            finally
            {
                localServer?.Stop();
            }
        }

        /// <summary>
        /// Refreshes the balance of the specified user account.
        /// </summary>
        /// <param name="tokenContract">The token contract for the balance refresh.</param>
        /// <param name="userAccount">The user account to refresh the balance.</param>
        /// <returns>The refresh balance payload data.</returns>
        public async Task<RefreshBalancePayloadData> RefreshBalance(string tokenContract, string userAccount)
        {
            try
            {
                string route = $"#refreshBalance?tokenContract={tokenContract}&userAccount={userAccount}";
                RunServer();
                RefreshBalancePayloadData payloadData = await ShowWebView<RefreshBalancePayloadData>(route, true);
                return payloadData;
            }
            finally
            {
                localServer?.Stop();
            }
        }

        /// <summary>
        /// Starts the local server.
        /// </summary>
        private void RunServer()
        {
            localServer?.Dispose();
            localServer = new LocalServer(SynchronizationContext.Current, "http://127.0.0.1", _port);
            localServer.Start();
        }

        /// <summary>
        /// Displays a web view with the specified route and returns the payload data.
        /// </summary>
        /// <typeparam name="TPayloadData">The type of the payload data.</typeparam>
        /// <param name="route">The route for the web view.</param>
        /// <param name="isSilent">Whether the web view is silent.</param>
        /// <returns>The payload data.</returns>
        private Task<TPayloadData> ShowWebView<TPayloadData>(string route, bool isSilent = false)
        {
            TaskCompletionSource<TPayloadData> tcs = new TaskCompletionSource<TPayloadData>();
#if !UNIWEBVIEW
            tcs.SetException(new Exception("Unable to locate UniWebView plugin. Please install it."));
#else
            bool webViewMessageHandled = false;

            async void WebViewMessageReceived(UniWebView webView, UniWebViewMessage message)
            {
                Log($"WebViewMessageReceived: {message.RawMessage}", LogType.Log);
                try
                {
                    TPayloadData payloadData = await MessageParser.ParseMessage<TPayloadData>(message.RawMessage);
                    webViewMessageHandled = true;
                    tcs.SetResult(payloadData);
                    if (isSilent)
                    {
                        CloseWebView(webView);
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }

                CloseWebView(webView);
            }

            void CloseWebView(UniWebView webView)
            {
                if (isSilent)
                {
                    Object.Destroy(webView.gameObject);
                    webView = null;
                    return;
                }

                webView.Hide(false, UniWebViewTransitionEdge.Bottom, completionHandler: () =>
                {
                    Object.Destroy(webView.gameObject);
                    webView = null;
                });
            }

            if (localServer == null || !localServer.IsRunning)
            {
                tcs.SetException(new Exception("Local server is not running"));
                return tcs.Task;
            }

            // UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;
            UniWebView.SetAllowJavaScriptOpenWindow(true);
            GameObject webViewGameObject = new GameObject("UniWebView - Generated");
            UniWebView webView = webViewGameObject.AddComponent<UniWebView>();
#if UNITY_EDITOR
            webView.Frame = new Rect(0, 0, 1920, 1080);
#else
            webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
#endif
            webView.SetSupportMultipleWindows(true, true);
            webView.SetAcceptThirdPartyCookies(true);
            webView.SetAllowHTTPAuthPopUpWindow(true);
            webView.SetVerticalScrollBarEnabled(true);
            webView.BackgroundColor = Color.black;
            webView.EmbeddedToolbar.Show();
            webView.EmbeddedToolbar.HideNavigationButtons();
            Log("Default WebView UserAgent: " + webView.GetUserAgent(), LogType.Log);
            if (!string.IsNullOrEmpty(_currentUserAgent))
            {
                webView.SetUserAgent(_currentUserAgent);
            }

            Log("Selected UserAgent: " + webView.GetUserAgent(), LogType.Log);
            // webview.OnMessageReceived += WebViewMessageReceived;
            webView.OnMessageReceived += WebViewMessageReceived;
            webView.OnShouldClose += view =>
            {
                if (!webViewMessageHandled)
                {
                    tcs.SetException(new UnhandledWebViewMessageException("WebView message not handled"));
                }

                return true;
            };

            webView.OnLoadingErrorReceived += (view, code, message, payload) =>
            {
                tcs.SetException(new Exception($"WebView error: Code: {code}, Message:{message}"));
                CloseWebView(view);
            };

            string url = localServer.Url;
            if (!string.IsNullOrEmpty(route))
            {
                url += route;
            }

            webView.Load(url);
            Log($"WebView loaded: Url: {url}", LogType.Log);
            if (!isSilent)
            {
                webView.Show(false, UniWebViewTransitionEdge.Bottom);
            }
#endif

            return tcs.Task;
        }

        /// <summary>
        /// Disposes the CoconutWax instance and releases all resources used by the local server.
        /// </summary>
        public void Dispose()
        {
            localServer?.Dispose();
        }
    }
}