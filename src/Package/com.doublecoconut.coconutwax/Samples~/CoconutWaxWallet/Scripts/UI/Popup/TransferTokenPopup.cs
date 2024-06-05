using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wax;
using Wax.Payload;
using static Logger.CoconutWaxLogger;
using LogType = Logger.LogType;

namespace Samples.CoconutWaxWallet.Scripts.UI.Popup
{
    /// <summary>
    /// The TransferTokenPopup class is used to manage the transfer token popup in the application's user interface.
    /// </summary>
    public class TransferTokenPopup : AbstractView
    {
        /// <summary>
        /// The text for the balance of the token.
        /// </summary>
        [SerializeField] private TextMeshProUGUI balanceText;
        /// <summary>
        /// The dropdown for selecting the symbol of the token.
        /// </summary>
        [SerializeField] private TMP_Dropdown symbolDropDown;
        /// <summary>
        /// The input field for the account to transfer the token to.
        /// </summary>
        [SerializeField] private TMP_InputField toAccountInput;
        /// <summary>
        /// The input field for the amount of the token to transfer.
        /// </summary>
        [SerializeField] private TMP_InputField amountInput;
        /// <summary>
        /// The input field for the memo of the transfer.
        /// </summary>
        [SerializeField] private TMP_InputField memoInput;
        /// <summary>
        /// The button for transferring the token.
        /// </summary>
        [SerializeField] private Button transferButton;
        /// <summary>
        /// The button for closing the transfer token popup.
        /// </summary>
        [SerializeField] private Button closeButton;
        /// <summary>
        /// The task completion source for the result of the transfer.
        /// </summary>
        private readonly TaskCompletionSource<TransferTokenPayloadData> _tcs =
            new TaskCompletionSource<TransferTokenPayloadData>();
        /// <summary>
        /// Method to initialize the view.
        /// </summary>
        public override void Initialize(IInitializationArgs args)
        {
            UpdateUI();
        }

        private void Start()
        {
            transferButton.onClick.AddListener(OnTransferClicked);
            closeButton.onClick.AddListener(Hide);
            amountInput.onEndEdit.AddListener(OnAmountEndEdit);
            symbolDropDown.onValueChanged.AddListener(OnBalanceDropDownValueChanged);
        }
        /// <summary>
        /// Updates the amount input and balance text when the selected index of the balance dropdown changes.
        /// </summary>
        private void OnBalanceDropDownValueChanged(int index)
        {
            amountInput.text = string.Empty;
            balanceText.text = Session.UserAccount.Balance[index].Balance.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Validates and updates the amount input when the user finishes editing it.
        /// If the input amount is greater than the current balance, it is set to the current balance.
        /// If the input amount is not a valid number, it is cleared and an error is logged.
        /// </summary>
        private void OnAmountEndEdit(string content)
        {
            try
            {
                float inputAmount = float.Parse(content);
                float currentBalance = Session.UserAccount.Balance
                    .Find(info => info.Symbol == symbolDropDown.options[symbolDropDown.value].text).Balance;
                inputAmount = Mathf.Min(inputAmount, currentBalance);
                amountInput.text = inputAmount.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                amountInput.text = string.Empty;
                Log($"Failed to parse amount: {e}", LogType.Error);
            }
        }
        /// <summary>
        /// Method to handle the transfer button click.
        /// </summary>
        private async void OnTransferClicked()
        {
            string toAccount = toAccountInput.text;
            if (string.IsNullOrEmpty(toAccount) || string.IsNullOrEmpty(amountInput.text))
            {
                UIManager.Instance.ShowPopup<AlertPopup>(new AlertPopup.InitializationArgs
                {
                    Title = "Error",
                    Message = "To account and amount are required."
                });
                return;
            }

            try
            {
                string symbol = symbolDropDown.options[symbolDropDown.value].text;
                BalanceInfo balance = Session.UserAccount.Balance.Find(info => info.Symbol == symbol);
                if (balance==null)
                {
                    throw new Exception("Balance not found");
                }

                TransferTokenPayloadData result = await CoconutWaxRuntime.Instance.CoconutWax.TransferToken(
                    toAccount,
                    float.Parse(amountInput.text),
                    balance.Contract,
                    balance.Symbol,
                    memoInput.text
                );
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
        public Task<TransferTokenPayloadData> AwaitResult()
        {
            return _tcs.Task;
        }
        /// <summary>
        /// Closes the transfer token popup and cancels the task completion source.
        /// </summary>
        private void Hide()
        {
            UIManager.Instance.ClosePopup();
            _tcs.SetCanceled();
        }
        /// <summary>
        /// Updates the UI of the transfer token popup. It populates the symbol dropdown with the symbols of the user's balances
        /// and updates the amount input and balance text based on the selected balance.
        /// </summary>
        private void UpdateUI()
        {
            IEnumerable<TMP_Dropdown.OptionData> options = Session.UserAccount.Balance
                .Select(info => new TMP_Dropdown.OptionData
                {
                    text = info.Symbol
                });

            symbolDropDown.ClearOptions();
            symbolDropDown.AddOptions(options.ToList());
            OnBalanceDropDownValueChanged(0);
        }
    }
}