using System;
using System.Threading.Tasks;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using Samples.CoconutWaxWallet.Scripts.UI.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.CoconutWaxWallet.Scripts.UI.Popup
{
    /// <summary>
    /// The AlertPopup class is used to manage the alert popup in the application's user interface.
    /// </summary>
    public class AlertPopup : AbstractView
    {
        /// <summary>
        /// The InitializationArgs class is used to initialize the AlertPopup.
        /// </summary>
        public class InitializationArgs : IInitializationArgs
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string ActionButtonText { get; set; }
            public bool CloseAfterAction { get; set; }
            public Action Action { get; set; }
        }
        /// <summary>
        /// The text for the title of the alert.
        /// </summary>
        [SerializeField] private TextMeshProUGUI titleText;
        /// <summary>
        /// The text for the message of the alert.
        /// </summary>
        [SerializeField] private TextMeshProUGUI messageText;
        /// <summary>
        /// The text for the action button of the alert.
        /// </summary>
        [SerializeField] private TextMeshProUGUI actionButtonText;
        /// <summary>
        /// The button for closing the alert.
        /// </summary>
        [SerializeField] private Button closeButton;
        /// <summary>
        /// The button for performing the action of the alert.
        /// </summary>
        [SerializeField] private Button actionButton;

        /// <summary>
        /// The result of the alert.
        /// </summary>
        public enum Result
        {
            Action,
            Close
        }
        /// <summary>
        /// The task completion source for the result of the alert.
        /// </summary>
        private readonly TaskCompletionSource<Result> _tcs = new TaskCompletionSource<Result>();
        /// <summary>
        /// The initialization arguments for the alert.
        /// </summary>
        private InitializationArgs _initializationArgs;
       
        private void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                _tcs.SetResult(Result.Close);
                UIManager.Instance.ClosePopup();
            });
            actionButton.onClick.AddListener(() =>
            {
                _initializationArgs.Action?.Invoke();
                _tcs.SetResult(Result.Action);
                if (_initializationArgs.CloseAfterAction)
                {
                    UIManager.Instance.ClosePopup();
                }
            });
        }
        /// <summary>
        /// Method to await the result of the alert.
        /// </summary>
        public Task<Result> AwaitResult()
        {
            return _tcs.Task;
        }
        /// <summary>
        /// Method to initialize the view.
        /// </summary>
        public override void Initialize(IInitializationArgs args)
        {
            _initializationArgs = args.CastTo<InitializationArgs>();
            titleText.text = _initializationArgs.Title;
            messageText.text = _initializationArgs.Message;
            actionButtonText.text = _initializationArgs.ActionButtonText;
            actionButton.gameObject.SetActive(_initializationArgs.Action != null);
        }
    }
}