import {Action} from './action.js';
import {getUserBalanceByContract} from '../tools/utilities.js';
/// <summary>
/// The RefreshBalanceAction class extends the Action class and is used to handle the "refreshBalance" action in the application.
/// </summary>
export class RefreshBalanceAction extends Action {
    constructor(wax) {
        super(wax, "refreshBalance");
    }

    /// <summary>
    /// Handles the balance refreshing process. It gets the user balance for a specific contract and sets the balance property on the result.data object.
    /// </summary>
    async internalHandler(params) {
        if (!params.userAccount || !params.tokenContract) {
            throw new Error("UserAccount and TokenContract are required");
        }

        const balance = await getUserBalanceByContract(this.wax, params.tokenContract, params.userAccount);
        this.result.data.balance = balance;
    }
} 