using System;
using System.Threading.Tasks;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using Samples.CoconutWaxWallet.Scripts.UI.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wax;
using Wax.Payload;

namespace Samples.CoconutWaxWallet.Scripts.UI.Popup
{
    /// <summary>
    /// The TransferNFTPopup class is used to manage the transfer NFT popup in the application's user interface.
    /// </summary>
    public class TransferNFTPopup : AbstractView
    {
        /// <summary>
        /// The InitializationArgs class is used to initialize the TransferNFTPopup.
        /// </summary>
        public class InitializationArgs : IInitializationArgs
        {
            public string AssetId { get; set; }
        }
        /// <summary>
        /// The text for the asset ID of the NFT.
        /// </summary>
        [SerializeField] private TextMeshProUGUI assetIdText;
        /// <summary>
        /// The input field for the account to transfer the NFT to.
        /// </summary>
        [SerializeField] private TMP_InputField toAccountInput;
        /// <summary>
        /// The input field for the memo of the transfer.
        /// </summary>
        [SerializeField] private TMP_InputField memoInput;
        /// <summary>
        /// The button for transferring the NFT.
        /// </summary>
        [SerializeField] private Button transferButton;
        /// <summary>
        /// The button for closing the transfer NFT popup.
        /// </summary>
        [SerializeField] private Button closeButton;
        /// <summary>
        /// The initialization arguments for the transfer NFT popup.
        /// </summary>
        private InitializationArgs _args;
        /// <summary>
        /// The task completion source for the result of the transfer.
        /// </summary>
        private readonly TaskCompletionSource<TransferNFTPayloadData> _tcs =
            new TaskCompletionSource<TransferNFTPayloadData>();
        /// <summary>
        /// Method to initialize the view.
        /// </summary>
        public override void Initialize(IInitializationArgs args)
        {
            _args = args.CastTo<InitializationArgs>();
            UpdateUI();
        }

        private void Start()
        {
            transferButton.onClick.AddListener(OnTransferClicked);
            closeButton.onClick.AddListener(Hide);
        }

        /// <summary>
        /// Method to handle the transfer button click.
        /// </summary>
        private async void OnTransferClicked()
        {
            string toAccount = toAccountInput.text;
            if (string.IsNullOrEmpty(toAccount))
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Invalid Account",
                    Message = "Account cannot be empty"
                });
                return;
            }

            try
            {
                TransferNFTPayloadData result = await CoconutWaxRuntime.Instance.CoconutWax.TransferNFT(toAccount,
                    new[] { _args.AssetId },
                    memoInput.text);
                _tcs.SetResult(result);
            }
            catch (Exception e)
            {
                _tcs.SetException(e);
            }

            Close();
        }

        /// <summary>
        /// Method to await the result of the transfer.
        /// </summary>
        public Task<TransferNFTPayloadData> AwaitResult()
        {
            return _tcs.Task;
        }
        /// <summary>
        /// Method to close the transfer NFT popup.
        /// </summary>
        private void Hide()
        {
            UIManager.Instance.ClosePopup();
            _tcs.SetCanceled();
        }
        /// <summary>
        /// Method to update the UI.
        /// </summary>
        private void UpdateUI()
        {
            assetIdText.text = _args.AssetId;
        }
    }
}