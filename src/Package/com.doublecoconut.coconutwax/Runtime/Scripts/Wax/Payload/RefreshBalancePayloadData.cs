using System;

namespace Wax.Payload
{
    /// <summary>
    /// The RefreshBalancePayloadData class represents the payload data for a balance refresh operation.
    /// </summary>
    [Serializable]
    public class RefreshBalancePayloadData
    {
        /// <summary>
        /// Gets or sets the balance information.
        /// </summary>
        public BalanceInfo Balance { get; set; }
    }
}