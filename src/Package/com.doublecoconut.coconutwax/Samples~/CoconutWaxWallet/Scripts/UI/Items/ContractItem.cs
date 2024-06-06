using Samples.CoconutWaxWallet.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Samples.CoconutWaxWallet.Scripts.UI.Items
{
    /// <summary>
    /// The ContractItem class represents a UI component for managing a contract setting.
    /// It extends the MonoBehaviour class, allowing it to be attached to a GameObject in a Unity scene.
    /// </summary>
    public class ContractItem : MonoBehaviour
    {
        /// <summary>
        /// The input field for the contract.
        /// </summary>
        [SerializeField] private TMP_InputField contractInputField;
        /// <summary>
        /// The button for deleting the contract setting.
        /// </summary>
        [SerializeField] private Button deleteButton;
        /// <summary>
        /// The event that is triggered when the delete button is clicked.
        /// </summary>
        public readonly UnityEvent<ContractSetting> DeleteEvent = new UnityEvent<ContractSetting>();
      
        /// <summary>
        /// The contract setting.
        /// </summary>
        public ContractSetting Setting { get; private set; }

        private void Start()
        {
            deleteButton.onClick.AddListener(() => DeleteEvent.Invoke(Setting));
            contractInputField.onValueChanged.AddListener(OnValueChanged);
        }
        /// <summary>
        /// Method to handle the value change of the contract input field.
        /// </summary>
        private void OnValueChanged(string value)
        {
            Setting.Contract = value;
        }
        /// <summary>
        /// Method to set up the contract item with the given contract setting.
        /// </summary>
        public void Setup(ContractSetting setting)
        {
            Setting = setting;
            UpdateUI();
        }
        /// <summary>
        /// Method to update the UI of the contract item.
        /// </summary>
        private void UpdateUI()
        {
            contractInputField.text = Setting.Contract;
            deleteButton.interactable = !Setting.IsDefault;
            contractInputField.interactable = !Setting.IsDefault;
        }
    }
}