using System;
using AtomicHub;
using RestClient.Response;
using Samples.CoconutWaxWallet.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;

namespace Samples.CoconutWaxWallet.Scripts.UI.Items
{
    /// <summary>
    /// The NftItem class represents a UI component for displaying a non-fungible token (NFT) item.
    /// It extends the MonoBehaviour class, allowing it to be attached to a GameObject in a Unity scene.
    /// </summary>
    public class NftItem : MonoBehaviour
    {
        /// <summary>
        /// The image of the NFT.
        /// </summary>
        [SerializeField] private RawImage image;
        /// <summary>
        /// The text field for displaying the title of the NFT.
        /// </summary>
        [SerializeField] private TextMeshProUGUI titleText;
        /// <summary>
        /// The button for transferring the NFT.
        /// </summary>
        [SerializeField] private Button transferButton;

        /// <summary>
        /// The event that is triggered when the transfer button is clicked.
        /// </summary>
        public readonly UnityEvent<string> TransferEvent = new UnityEvent<string>();
        /// <summary>
        /// The NFT asset.
        /// </summary>
        private AssetItem<AlienWorldsAssetData> _asset;
        /// <summary>
        /// The ID of the NFT asset.
        /// </summary>
        public string AssetId => _asset?.AssetId ?? throw new Exception("Asset not found.");

        private void Start()
        {
            transferButton.onClick.AddListener(() => TransferEvent.Invoke(_asset.AssetId));
        }
        /// <summary>
        /// Method to set up the NFT item with the given asset.
        /// </summary>
        public void Setup(AssetItem<AlienWorldsAssetData> asset)
        {
            _asset = asset;
            UpdateUI();
        }
        /// <summary>
        /// Method to update the UI of the NFT item.
        /// </summary>
        private void UpdateUI()
        {
            transferButton.interactable = _asset.IsTransferable;
            titleText.text = _asset.Name;
            if (!string.IsNullOrEmpty(_asset.Data.ImageKey))
            {
                RetrieveImage();
            }
        }
        /// <summary>
        /// Method to retrieve the image of the NFT.
        /// </summary>
        private async void RetrieveImage()
        {
            try
            {
                Texture2D texture = await AtomicAssets.GetAssetImage(_asset.Data.ImageKey, destroyCancellationToken);
                image.texture = texture;
            }
            catch (Exception e)
            {
                Log($"Failed to retrieve image: {e}", LogType.Error);
            }
        }
        /// <summary>
        /// Method called when the NFT item is destroyed. It removes all listeners from the transfer event.
        /// </summary>
        private void OnDestroy()
        {
            TransferEvent.RemoveAllListeners();
        }
    }
}