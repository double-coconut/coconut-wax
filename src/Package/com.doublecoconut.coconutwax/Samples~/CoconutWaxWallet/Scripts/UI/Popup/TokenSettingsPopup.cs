using System.Collections.Generic;
using System.Linq;
using Samples.CoconutWaxWallet.Scripts.Data;
using Samples.CoconutWaxWallet.Scripts.UI.Abstraction;
using Samples.CoconutWaxWallet.Scripts.UI.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.CoconutWaxWallet.Scripts.UI.Popup
{
    /// <summary>
    /// The TokenSettingsPopup class is used to manage the token settings popup in the application's user interface.
    /// </summary>
    public class TokenSettingsPopup : AbstractView
    {
        /// <summary>
        /// The prefab used to instantiate new contract items in the token settings popup.
        /// </summary>
        [SerializeField] private ContractItem contractItemPrefab;
        /// <summary>
        /// The main container for the contract items in the token settings popup.
        /// </summary>
        [SerializeField] private RectTransform container;
        /// <summary>
        /// The button for closing the token settings popup.
        /// </summary>
        [SerializeField] private Button closeButton;
        /// <summary>
        /// The button for adding a new contract in the token settings popup.
        /// </summary>
        [SerializeField] private Button newButton;
        /// <summary>
        /// The button for saving the changes made in the token settings popup.
        /// </summary>
        [SerializeField] private Button saveButton;
        /// <summary>
        /// The list of contract items in the token settings popup.
        /// </summary>
        private readonly List<ContractItem> _items = new List<ContractItem>();
        /// <summary>
        /// The list of contract settings in the token settings popup.
        /// </summary>
        private List<ContractSetting> _contracts;

        private void Start()
        {
            closeButton.onClick.AddListener(Hide);
            saveButton.onClick.AddListener(SaveButtonClicked);
            newButton.onClick.AddListener(AddNewContract);
        }
        /// <summary>
        /// Creates a new contract with default settings, adds it to the list of contracts, 
        /// creates a corresponding contract item, and adds it to the list of contract items.
        /// </summary>
        private void AddNewContract()
        {
            ContractSetting contract = ContractSetting.CreateDefault();
            _contracts.Add(contract);
            ContractItem item = CreateContract(contract);
            _items.Add(item);
        }
        /// <summary>
        /// Saves the valid contracts to the application settings, saves the application settings, 
        /// and closes the token settings popup.
        /// </summary>
        private void SaveButtonClicked()
        {
            ApplicationSettings.TokenSettings.Contracts = _contracts.Where(setting => setting.IsValid).ToList();
            ApplicationSettings.Save();
            Hide();
        }
        /// <summary>
        /// Closes the token settings popup.
        /// </summary>
        private void Hide()
        {
            UIManager.Instance.ClosePopup();
        }

        /// <summary>
        /// Method to initialize the view.
        /// </summary>
        public override void Initialize(IInitializationArgs args)
        {
            _contracts = new List<ContractSetting>(ApplicationSettings.TokenSettings.Contracts);
            UpdateUI();
        }
        /// <summary>
        /// Method to update the UI.
        /// </summary>
        private void UpdateUI()
        {
            ClearContractsItems();
            foreach (ContractSetting contractSetting in _contracts)
            {
                ContractItem item = CreateContract(contractSetting);
                _items.Add(item);
            }
        }
        /// <summary>
        /// Method to create a contract item.
        /// </summary>
        private ContractItem CreateContract(ContractSetting setting)
        {
            ContractItem item = Instantiate(contractItemPrefab, container);
            item.DeleteEvent.AddListener(OnContractDeleteClicked);
            item.Setup(setting);
            return item;
        }
        /// <summary>
        /// Method to handle the contract delete click.
        /// </summary>
        private void OnContractDeleteClicked(ContractSetting setting)
        {
            ContractItem item = _items.Find(i => i.Setting.Id == setting.Id);
            Destroy(item.gameObject);
            _contracts.RemoveAll(s => s.Id == setting.Id);
        }
        /// <summary>
        /// Method to clear the contract items.
        /// </summary>
        private void ClearContractsItems()
        {
            while (_items.Count > 0)
            {
                Destroy(_items[0].gameObject);
                _items.RemoveAt(0);
            }

            _items.Clear();
        }
    }
}