using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Wax.Payload;

namespace Samples.CoconutWaxWallet.Scripts.UI.Items
{
    /// <summary>
    /// The BalanceItem class represents a UI component for displaying a user's balance information.
    /// It extends the MonoBehaviour class, allowing it to be attached to a GameObject in a Unity scene.
    /// </summary>
    public class BalanceItem : MonoBehaviour
    {
        /// <summary>
        /// The text field for displaying the balance.
        /// </summary>
        [SerializeField] private TextMeshProUGUI balanceText;
        /// <summary>
        /// The button for refreshing the balance.
        /// </summary>
        [SerializeField] private Button refreshButton;
        /// <summary>
        /// The event that is triggered when the refresh button is clicked.
        /// </summary>
        public readonly UnityEvent<string> RefreshEvent = new UnityEvent<string>();

        /// <summary>
        /// The balance information.
        /// </summary>
        public BalanceInfo BalanceInfo { get; private set; }

        private void Start()
        {
            refreshButton.onClick.AddListener(() => RefreshEvent.Invoke(BalanceInfo.Contract));
        }
        /// <summary>
        /// Method to set up the balance item with the given balance information.
        /// </summary>
        public void Setup(BalanceInfo balanceInfo)
        {
            if (balanceInfo != null)
            {
                BalanceInfo = balanceInfo;
            }

            UpdateUI();
        }
        /// <summary>
        /// Method to set a placeholder for the balance text.
        /// </summary>
        public void SetPlaceholder(string placeholder)
        {
            balanceText.text = placeholder;
        }
        /// <summary>
        /// Method to update the UI of the balance item.
        /// </summary>
        private void UpdateUI()
        {
            balanceText.text = $"{BalanceInfo.Balance} {BalanceInfo.Symbol}";
        }

        /// <summary>
        /// Method called when the balance item is destroyed. It removes all listeners from the refresh event.
        /// </summary>
        private void OnDestroy()
        {
            RefreshEvent.RemoveAllListeners();
        }
    }
}