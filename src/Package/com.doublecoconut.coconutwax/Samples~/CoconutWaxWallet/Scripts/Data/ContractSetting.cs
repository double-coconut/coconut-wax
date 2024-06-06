using System;

namespace Samples.CoconutWaxWallet.Scripts.Data
{
    /// <summary>
    /// The ContractSetting class is used to manage the settings for a contract in the application.
    /// </summary>
    public class ContractSetting
    {
        public string Id { get; set; }
        public string Contract { get; set; }
        public bool IsDefault { get; set; }
        
        public bool IsValid => !string.IsNullOrEmpty(Contract);
        
        public static ContractSetting CreateDefault()
        {
            return new ContractSetting
            {
                Id = Guid.NewGuid().ToString(),
                Contract = string.Empty,
                IsDefault = false
            };
        }
    }
}