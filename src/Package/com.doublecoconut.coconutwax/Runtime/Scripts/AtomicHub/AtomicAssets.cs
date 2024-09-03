using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestClient;
using RestClient.Response;
using Tools;
using UnityEngine;
using UnityEngine.Networking;

namespace AtomicHub
{
    /// <summary>
    /// Provides methods for interacting with the AtomicAssets API. This class contains methods for getting user assets and asset images.
    /// </summary>
    public static class AtomicAssets
    {
        /// <summary>
        /// The base URL for the AtomicAssets API.
        /// </summary>
        private const string BaseUrl = "https://wax.api.atomicassets.io/";

        /// <summary>
        /// Sets default headers for the requests.
        /// </summary>
        static AtomicAssets()
        {
            RequestClient.SetDefaultHeader("Content-Type", "application/json");
            RequestClient.SetDefaultHeader("Accept", "application/json");
        }

        /// <summary>
        /// Gets user assets from the AtomicAssets API.
        /// </summary>
        public static Task<UserAssetsResponse<TAssetData>> GetUserAssets<TAssetData>(string owner,
            bool? isTransferable = null,
            int page = 1, int limit = 50,
            string collectionName=null,
            CancellationToken cancellationToken = default)
        {
            string url = $"{BaseUrl}atomicassets/v1/assets?owner={owner}&page={page}&limit={limit}";
            if (isTransferable != null)
            {
                url += $"&is_transferable={isTransferable.ToString().ToLower()}";
            }
            if(!string.IsNullOrEmpty(collectionName)){
                url+=$"&collection_name={collectionName}";
            }

            IRequestClientSender request = RequestClient
                .Create()
                .SetUrl(url, UnityWebRequest.kHttpVerbGET)
                .BuildRequest();

            return Send<UserAssetsResponse<TAssetData>>(request, cancellationToken);
        }

        /// <summary>
        /// Sends a request to the AtomicAssets API and returns the response.
        /// </summary>
        private static async Task<TResponse> Send<TResponse>(IRequestClientSender client,
            CancellationToken cancellationToken)
        {
            byte[] result = await client.Send(cancellationToken);
            client.Dispose();
            string content = Encoding.UTF8.GetString(result);
            TResponse response = Serializer.Deserialize<TResponse>(content);
            return response;
        }

        /// <summary>
        /// Gets an asset image from the AtomicAssets API.
        /// </summary>
        public static async Task<Texture2D> GetAssetImage(string imageKey,
            CancellationToken cancellationToken = default)
        {
            const string ImageBaseUrl = "https://ipfs.io/ipfs/";
            string url = $"{ImageBaseUrl}{imageKey}";
            IRequestClientSender request = RequestClient
                .Create()
                .SetUrl(url, UnityWebRequest.kHttpVerbGET)
                .SetIgnoreDefaultHeaders(true)
                .SetDownloadHandler(new DownloadHandlerTexture(true))
                .BuildRequest();
            return await request.SendAsTexture(cancellationToken);
        }
    }
}
