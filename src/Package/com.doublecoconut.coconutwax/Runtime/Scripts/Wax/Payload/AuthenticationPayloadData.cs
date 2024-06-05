using System;
using System.Collections.Generic;

namespace Wax.Payload
{
    /// <summary>
    /// The BalanceInfo class represents the balance information for a specific contract.
    /// </summary>
    public class BalanceInfo
    {
        /// <summary>
        /// Gets or sets the contract.
        /// </summary>
        public string Contract { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        public float Balance { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{nameof(Contract)}: {Contract}, {nameof(Balance)}: {Balance}, {nameof(Symbol)}: {Symbol}";
        }
    }

    /// <summary>
    /// The AuthenticationPayloadData class represents the authentication payload data.
    /// </summary>
    [Serializable]
    public class AuthenticationPayloadData
    {
        /// <summary>
        /// Gets or sets the user account.
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is temporary.
        /// </summary>
        public bool IsTemp { get; set; }

        /// <summary>
        /// Gets or sets the public keys.
        /// </summary>
        public string[] Keys { get; set; }

        /// <summary>
        /// Gets or sets the trust score.
        /// </summary>
        public float TrustScore { get; set; }

        /// <summary>
        /// Gets or sets the trust score provider.
        /// </summary>
        public string TrustScoreProvider { get; set; }

        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        public List<BalanceInfo> Balance { get; set; }

        /// <summary>
        /// Updates the balance with the specified balance information.
        /// </summary>
        /// <param name="balanceInfo">The balance information.</param>
        public void UpdateBalance(BalanceInfo balanceInfo)
        {
            BalanceInfo balance = Balance.Find(info => info.Contract == balanceInfo.Contract);
            if (balance == null)
            {
                Balance.Add(balanceInfo);
                return;
            }

            balance.Balance = balanceInfo.Balance;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return
                $"{nameof(UserAccount)}: {UserAccount}, {nameof(AvatarUrl)}: {AvatarUrl}, {nameof(IsTemp)}: {IsTemp}, {nameof(Keys)}: {Keys}, {nameof(TrustScore)}: {TrustScore}, {nameof(TrustScoreProvider)}: {TrustScoreProvider}, {nameof(Balance)}: {Balance}";
        }
    }
}