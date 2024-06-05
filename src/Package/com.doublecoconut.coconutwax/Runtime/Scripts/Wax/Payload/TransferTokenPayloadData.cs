using System;

namespace Wax.Payload
{
    
    /// <summary>
    /// The TransferTokenPayloadData class represents the payload data for a token transfer operation.
    /// </summary>
    [Serializable]
    public class TransferTokenPayloadData
    {
        /// <summary>
        /// Gets or sets the token contract.
        /// </summary>
        public string TokenContract { get; set; }
        /// <summary>
        /// Gets or sets the transaction ID.
        /// </summary>
        public string TransactionId { get; set; }
    }
}