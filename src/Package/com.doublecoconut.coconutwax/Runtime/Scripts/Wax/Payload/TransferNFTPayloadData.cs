using System;

namespace Wax.Payload
{
    /// <summary>
    /// The TransferNFTPayloadData class represents the payload data for a NFT transfer operation.
    /// </summary>
    [Serializable]
    public class TransferNFTPayloadData
    {
        /// <summary>
        /// Gets or sets the transaction ID.
        /// </summary>
        public string TransactionId { get; set; }
    }
}