using Samples.CoconutWaxWallet.Scripts.UI;
using Samples.CoconutWaxWallet.Scripts.UI.View;
using UnityEngine;

namespace Samples.CoconutWaxWallet.Scripts
{
    /// <summary>
    /// The Application class is used to manage the application's lifecycle..
    /// </summary>
    public class Application : MonoBehaviour
    {
        /// <summary>
        /// Called before the first frame update. Sets up the application settings and user session. If a user session exists, it proceeds to the profile view, otherwise, it proceeds to the authentication view.
        /// </summary>
        private void Start()
        {
            ApplicationSettings.Setup();
            Session.Setup();
            if (Session.HasUserAccount)
            {
                ProceedProfile();
                return;
            }

            ProceedAuthentication();
        }
        /// <summary>
        /// Navigates to the ProfileView.
        /// </summary>
        private void ProceedProfile() => UIManager.Instance.ShowView<ProfileView>();
        /// <summary>
        /// Navigates to the AuthenticationView.
        /// </summary>
        private void ProceedAuthentication() => UIManager.Instance.ShowView<AuthenticationView>();
        /// <summary>
        /// Called when the application is about to quit. It saves the user session and application settings.
        /// </summary>
        private void OnApplicationQuit()
        {
            Session.Save();
            ApplicationSettings.Save();
        }
    }
}