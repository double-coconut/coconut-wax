import {Action} from './action.js';
import {formatToEightDecimalPlaces} from '../tools/utilities.js';
/// <summary>
/// The TransferTokenAction class extends the Action class and is used to handle the "transferToken" action in the application.
/// </summary>
export class TransferTokenActionAction extends Action {
    constructor(wax) {
        super(wax, "transferToken");
    }

    /// <summary>
    /// Handles the token transfer process. It logs in the user if not already logged in, initiates a transaction to transfer the tokens, and sets the transactionId and tokenContract properties on the result.data object.
    /// </summary>
    async internalHandler(params) {
        if (!params.toAccount || !params.amount || !params.symbol) {
            throw new Error("To Account, amount and symbol are required");
        }

        if (!this.wax.api) {
            await this.wax.login();
        }

        const transactionResult = await this.wax.api.transact({
            actions: [{
                account: params.tokenContract,
                name: 'transfer',
                authorization: [{
                    actor: this.wax.userAccount,
                    permission: 'active',
                }],
                data: {
                    from: this.wax.userAccount,
                    to: params.toAccount,
                    quantity: `${formatToEightDecimalPlaces(params.amount)} ${params.symbol}`,
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
        this.result.data.tokenContract = params.tokenContract;
    }
} 