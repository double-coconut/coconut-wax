using System.Collections.Generic;
using Wax;

namespace Samples.CoconutWaxWallet.Scripts.Data
{
    /// <summary>
    /// The TokenSettings class is used to manage the settings for tokens in the application.
    /// </summary>
    public class TokenSettings
    {
        public List<ContractSetting> Contracts { get; set; }

        public static TokenSettings CreateDefault()
        {
            return new TokenSettings
            {
                Contracts = new List<ContractSetting>
                {
                    new ContractSetting
                    {
                        Id = "1f6ae6ba-c024-45bb-aaa0-228ae0e674b9",
                        Contract = CoconutWax.DefaultWaxTokenContract,
                        IsDefault = true
                    }
                }
            };
        }
    }
}