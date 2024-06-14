import {Action} from './action.js';
import {getUserBalance} from '../tools/utilities.js';
/// <summary>
/// The AuthenticationAction class extends the Action class and is used to handle the "authenticate" action in the application.
/// </summary>
export class AuthenticationAction extends Action {
    constructor(wax) {
        super(wax, "authenticate");
    }

    /// <summary>
    /// Handles the authentication process. It logs in the user, gets the user balance, and sets various properties on the result.data object.
    /// </summary>
    async internalHandler(params) {
        if (!params.tokenContracts) {
            throw new Error("TokenContracts are required");
        }
        const userAccount = await this.wax.login();
        const balance = await getUserBalance(this.wax, params.tokenContracts.split(','), userAccount);
        this.result.data.userAccount = userAccount;
        this.result.data.avatarUrl = this.wax.user.avatarUrl;
        this.result.data.isTemp = this.wax.user.isTemp;
        this.result.data.keys = this.wax.user.keys;
        this.result.data.trustScore = this.wax.user.trustScore || 0;
        this.result.data.trustScoreProvider = this.wax.user.trustScoreProvider;
        this.result.data.balance = balance;
    }
}

