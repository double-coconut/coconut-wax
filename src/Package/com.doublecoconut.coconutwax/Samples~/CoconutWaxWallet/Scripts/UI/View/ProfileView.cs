using System;
using System.Collections.Generic;
using System.Globalization;
#if !UNITY_2022_3
using System.Threading;
#endif
using System.Threading.Tasks;
using AtomicHub;
using RestClient.Response;
using Samples.CoconutWaxWallet.Scripts.Data;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using Samples.CoconutWaxWallet.Scripts.UI.Items;
using Samples.CoconutWaxWallet.Scripts.UI.Popup;
using Samples.CoconutWaxWallet.Scripts.UI.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wax;
using Wax.Payload;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;

namespace Samples.CoconutWaxWallet.Scripts.UI.View
{
    public class ProfileView : AbstractView
    {
        /// <summary>
        /// The main container for the profile view.
        /// </summary>
        [SerializeField] private RectTransform mainContainer;
        /// <summary>
        /// The image for the user's avatar.
        /// </summary>
        [SerializeField] private RawImage avatarImage;
        /// <summary>
        /// The text for the user's account.
        /// </summary>
        [SerializeField] private TextMeshProUGUI accountText;
        /// <summary>
        /// The text for the user's trust score.
        /// </summary>
        [SerializeField] private TextMeshProUGUI trustScoreText;
        /// <summary>
        /// The container for the balance items.
        /// </summary>
        [SerializeField] private RectTransform balanceItemsContainer;
        /// <summary>
        /// The main container for the NFT and balance items in the profile view.
        /// </summary>
        [SerializeField] private RectTransform container;
        /// <summary>
        /// The RectTransform used to display a loading animation while retrieving NFTs.
        /// </summary>
        [SerializeField] private RectTransform loadingRectTransform;
        /// <summary>
        /// The prefab used to instantiate new NFT items in the profile view.
        /// </summary>
        [SerializeField] private NftItem nftItemPrefab;
        /// <summary>
        /// The prefab used to instantiate new balance items in the profile view.
        /// </summary>
        [SerializeField] private BalanceItem balanceItemPrefab;
        /// <summary>
        /// The button for transferring tokens.
        /// </summary>
        [SerializeField] private Button tokenTransferButton;
        /// <summary>
        /// The button for refreshing NFTs.
        /// </summary>
        [SerializeField] private Button refreshNftsButton;
        /// <summary>
        /// The button for logging out.
        /// </summary>
        [SerializeField] private Button logoutButton;

        private readonly List<NftItem> _items = new List<NftItem>();
        private readonly List<BalanceItem> _balanceItems = new List<BalanceItem>();
        
#if !UNITY_2022_3
        private CancellationTokenSource _destroyCancellationTokenSource;

        private CancellationToken destroyCancellationToken
        {
            get
            {
                if (_destroyCancellationTokenSource==null)
                {
                    _destroyCancellationTokenSource = new CancellationTokenSource();
                }

                return _destroyCancellationTokenSource.Token;
            }
        }
#endif

        private void Start()
        {
            mainContainer.FitInSafeArea(FitmentType.Top);
            logoutButton.onClick.AddListener(OnLogoutButtonClicked);
            refreshNftsButton.onClick.AddListener(OnRefreshNftsClicked);
            tokenTransferButton.onClick.AddListener(OnTokenTransferClicked);
        }

        private void OnDestroy()
        {
#if !UNITY_2022_3
            _destroyCancellationTokenSource?.Cancel(false);
#endif
        }

        /// <summary>
        /// Method to initialize the view.
        /// </summary>
        public override void Initialize(IInitializationArgs args)
        {
            UpdateUI();
            RetrieveNfTs();
        }
        /// <summary>
        /// Method to update the UI.
        /// </summary>
        private void UpdateUI()
        {
            accountText.text = Session.UserAccount.UserAccount;
            trustScoreText.text = Session.UserAccount.TrustScore.ToString(CultureInfo.InvariantCulture);
            ClearBalanceItems();
            
            foreach (BalanceInfo info in Session.UserAccount.Balance)
            {
                BalanceItem item = Instantiate(balanceItemPrefab, balanceItemsContainer);
                item.RefreshEvent.AddListener(OnRefreshBalanceClicked);
                item.Setup(info);
                _balanceItems.Add(item);
            }

            RetrieveAvatarImage();
        }
        /// <summary>
        /// Method to retrieve the avatar image.
        /// </summary>
        private async void RetrieveAvatarImage()
        {
            if (string.IsNullOrEmpty(Session.UserAccount.AvatarUrl))
            {
                return;
            }

            try
            {
                Texture2D texture =
                    await AtomicAssets.GetAssetImage(Session.UserAccount.AvatarUrl, destroyCancellationToken);
                avatarImage.texture = texture;
            }
            catch (Exception e)
            {
                Log($"Failed to retrieve image: {e}", LogType.Error);
            }
        }
        /// <summary>
        /// Method to refresh the balance.
        /// </summary>
        private async void OnRefreshBalanceClicked(string tokenContract)
        {
            try
            {
                BalanceItem balanceItem = _balanceItems.Find(item => item.BalanceInfo.Contract == tokenContract);
                if (balanceItem == null)
                {
                    throw new Exception("Balance item not found");
                }

                balanceItem.SetPlaceholder("------");
                RefreshBalancePayloadData response =
                    await CoconutWaxRuntime.Instance.CoconutWax.RefreshBalance(tokenContract,
                        Session.UserAccount.UserAccount);
                Session.UserAccount.UpdateBalance(response.Balance);
                Session.Save();
                BalanceInfo balanceInfo = Session.UserAccount.Balance.Find(info => info.Contract == tokenContract);
                balanceItem.Setup(balanceInfo);
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Failed to Refresh Balance",
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// Method to refresh the NFTs.
        /// </summary>
        private void OnRefreshNftsClicked()
        {
            RetrieveNfTs();
        }
        /// <summary>
        /// Method to handle the logout button click.
        /// </summary>
        private void OnLogoutButtonClicked()
        {
            UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
            {
                Title = "Confirm Logout",
                Message = "Are you sure you want to logout?",
                ActionButtonText = "Logout",
                CloseAfterAction = true,
                Action = () =>
                {
                    Session.Delete();
                    UIManager.Instance.ShowView<AuthenticationView>();
                }
            });
        }
        /// <summary>
        /// Method to clear the NFTs.
        /// </summary>
        private void ClearNfts()
        {
            while (_items.Count > 0)
            {
                Destroy(_items[0].gameObject);
                _items.RemoveAt(0);
            }
        }
        /// <summary>
        /// Method to clear the balance items.
        /// </summary>
        private void ClearBalanceItems()
        {
            while (_balanceItems.Count > 0)
            {
                Destroy(_balanceItems[0].gameObject);
                _balanceItems.RemoveAt(0);
            }

            _balanceItems.Clear();
        }
        /// <summary>
        /// Method to retrieve the NFTs.
        /// </summary>
        private async void RetrieveNfTs()
        {
            ClearNfts();
            loadingRectTransform.gameObject.SetActive(true);
            try
            {
                UserAssetsResponse<AlienWorldsAssetData> result =
                    await AtomicAssets.GetUserAssets<AlienWorldsAssetData>(Session.UserAccount.UserAccount,
                        cancellationToken: destroyCancellationToken);

                if (!result.Success)
                {
                    throw new Exception("Result is not success");
                }

                foreach (AssetItem<AlienWorldsAssetData> assetItem in result.Data)
                {
                    NftItem nftItem = Instantiate(nftItemPrefab, container);
                    nftItem.TransferEvent.AddListener(OnNFTTransferInitiated);
                    nftItem.Setup(assetItem);
                    _items.Add(nftItem);
                }
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Failed to Retrieve NFTs",
                    Message = e.Message
                });
            }

            loadingRectTransform.gameObject.SetActive(false);
        }
        /// <summary>
        /// Method to handle the token transfer.
        /// </summary>
        private async void OnTokenTransferClicked()
        {
            try
            {
                TransferTokenPayloadData
                    result = await UIManager.Instance.ShowPopup<TransferTokenPopup>().AwaitResult();
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Transfer Successful",
                    Message = $"Token transferred successfully\nTransaction ID: {result.TransactionId}",
                    ActionButtonText = "Open Transaction",
                    CloseAfterAction = true,
                    Action = () =>
                        UnityEngine.Application.OpenURL($"https://waxblock.io/transaction/{result.TransactionId}")
                });
                OnRefreshBalanceClicked(result.TokenContract);
            }
            catch (TaskCanceledException)
            {
                //ignored
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Error",
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// Method to handle the NFT transfer.
        /// </summary>
        private async void OnNFTTransferInitiated(string assetId)
        {
            try
            {
                TransferNFTPayloadData result = await UIManager.Instance.ShowPopup<TransferNFTPopup>(
                        new TransferNFTPopup.InitializationArgs
                        {
                            AssetId = assetId
                        })
                    .AwaitResult();
                int itemIndex = _items.FindIndex(item => item.AssetId == assetId);
                if (itemIndex < 0)
                {
                    throw new Exception("Item not found");
                }

                Destroy(_items[itemIndex].gameObject);
                _items.RemoveAt(itemIndex);

                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Transfer Successful",
                    Message = $"Asset transferred successfully\nTransaction ID: {result.TransactionId}",
                    ActionButtonText = "Open Transaction",
                    CloseAfterAction = true,
                    Action = () =>
                        UnityEngine.Application.OpenURL($"https://waxblock.io/transaction/{result.TransactionId}")
                });
            }
            catch (TaskCanceledException)
            {
                //ignored
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Error",
                    Message = e.Message
                });
            }
        }
    }
}