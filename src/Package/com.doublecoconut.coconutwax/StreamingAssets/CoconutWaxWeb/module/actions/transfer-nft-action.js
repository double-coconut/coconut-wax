import {Action} from './action.js';
/// <summary>
/// The TransferNftAction class extends the Action class and is used to handle the "transferNFT" action in the application.
/// </summary>
export class TransferNftAction extends Action {
    constructor(wax) {
        super(wax, "transferNFT");
    }
    /// <summary>
    /// Handles the NFT transfer process. It logs in the user if not already logged in, initiates a transaction to transfer the NFTs, and sets the transactionId property on the result.data object.
    /// </summary>
    async internalHandler(params) {
        if (!params.toAccount || !params.assetIds) {
            throw new Error("To Account and Asset Ids are required");
        }
        params.assetIds = params.assetIds.split(',');

        if (!this.wax.api) {
            await this.wax.login();
        }

        const transactionResult = await this.wax.api.transact({
            actions: [{
                account: 'atomicassets',
                name: 'transfer',
                authorization: [{
                    actor: this.wax.userAccount,
                    permission: 'active',
                }],
                data: {
                    from: this.wax.userAccount,
                    to: params.toAccount,
                    asset_ids: params.assetIds,
                    memo: params.memoContent
                },
            }]
        }, {
            blocksBehind: 3,
            expireSeconds: 30
        });

        if (transactionResult.transaction_id) {
            this.result.data.transactionId = transactionResult.transaction_id;
        }
    }
} 