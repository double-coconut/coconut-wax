using System;
using System.Linq;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using Samples.CoconutWaxWallet.Scripts.UI.Popup;
using UnityEngine;
using UnityEngine.UI;
using Wax;
using Wax.Payload;

namespace Samples.CoconutWaxWallet.Scripts.UI.View
{
    /// <summary>
    /// The AuthenticationView class is used to manage the authentication view in the application's user interface.
    /// </summary>
    public class AuthenticationView : AbstractView
    {
        /// <summary>
        /// The main container for the authentication view.
        /// </summary>
        [SerializeField] private RectTransform mainContainer;
        /// <summary>
        /// The button for initiating authentication.
        /// </summary>
        [SerializeField] private Button authButton;
        /// <summary>
        /// The button for accessing token settings.
        /// </summary>
        [SerializeField] private Button tokenSettingsButton;

        /// <summary>
        /// Method called to setup button click listeners.
        /// </summary>
        private void Start()
        {
            authButton.onClick.AddListener(OnAuthButtonClicked);
            tokenSettingsButton.onClick.AddListener(OnTokenSettingsButtonClicked);
        }
        /// <summary>
        /// Method called when the authentication button is clicked. It initiates the authentication process.
        /// </summary>
        private async void OnAuthButtonClicked()
        {
            try
            {
                string[] contracts = ApplicationSettings.TokenSettings.Contracts
                    .Select(setting => setting.Contract)
                    .Distinct()
                    .ToArray();
                AuthenticationPayloadData payloadData =
                    await CoconutWaxRuntime.Instance.CoconutWax.Authenticate(tokenContracts: contracts);
                Session.UserAccount = payloadData;
                UIManager.Instance.ShowView<ProfileView>();
            }
            catch (Exception e)
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Failed to Authenticate",
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// Method called when the token settings button is clicked. It shows the token settings popup.
        /// </summary>
        private void OnTokenSettingsButtonClicked()
        {
            UIManager.Instance.ShowPopup<TokenSettingsPopup>();
        }
    }
}