using System;
using Newtonsoft.Json;

namespace RestClient.Response
{
    /// <summary>
    /// Represents an item of asset data. This class contains properties for the asset ID, 
    /// whether it is transferable, its name, and its data.
    /// </summary>
    [Serializable]
    public class AssetItem<TAssetData>
    {
        /// <summary>
        /// Gets or sets the asset ID.
        /// </summary>
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the asset is transferable.
        /// </summary>
        [JsonProperty("is_transferable")]
        public bool IsTransferable { get; set; }

        /// <summary>
        /// Gets or sets the name of the asset.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data of the asset.
        /// </summary>
        [JsonProperty("data")]
        public TAssetData Data { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(AssetId)}: {AssetId}, {nameof(IsTransferable)}: {IsTransferable}, {nameof(Name)}: {Name}, {nameof(Data)}: {Data}";
        }
    }

    /// <summary>
    /// Represents a response from a REST API that contains user asset data. 
    /// This class inherits from BaseResponse and contains an array of AssetItem objects.
    /// </summary>
    [Serializable]
    public class UserAssetsResponse<TAssetData> : BaseResponse
    {
        /// <summary>
        /// Gets or sets the array of AssetItem objects.
        /// </summary>
        [JsonProperty("data")]
        public AssetItem<TAssetData>[] Data { get; set; }
    }
}