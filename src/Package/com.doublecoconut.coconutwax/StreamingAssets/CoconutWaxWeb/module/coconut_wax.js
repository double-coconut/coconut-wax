import {ActionFactory} from "./actions/actionFactory.js";
import {Messaging} from "./tools/messaging.js";
import {AuthenticationAction} from "./actions/authentication-action.js";
import {RefreshBalanceAction} from "./actions/refresh-balance-action.js";
import {TransferTokenActionAction} from "./actions/transfer-token-action.js";
import {TransferNftAction} from "./actions/transfer-nft-action.js";
/// <summary>
/// The CoconutWax class is used to interact with the WAX blockchain. 
/// It initializes a new instance of waxjs.WaxJS and sets up an ActionFactory to handle various actions.
/// </summary>
export class CoconutWax {
    // Initializes a new instance of waxjs.WaxJS with the provided tryAutoLogin parameter
    // Sets up an ActionFactory to handle various actions like authentication, balance refreshing, and token transfers
    constructor(tryAutoLogin) {
        this.wax = new waxjs.WaxJS({
            rpcEndpoint: 'https://wax.greymass.com',
            tryAutoLogin: tryAutoLogin,
        });
        this.actionFactory = new ActionFactory();
        this.registerActions();
        this.init()
    }

    init() {
        window.addEventListener('load', this.onPageLoad.bind(this));
    }

    // Registers various actions with the ActionFactory
    registerActions() {
        this.actionFactory.register("authenticate", new AuthenticationAction(this.wax));
        this.actionFactory.register("refreshBalance", new RefreshBalanceAction(this.wax));
        this.actionFactory.register("transferToken", new TransferTokenActionAction(this.wax));
        this.actionFactory.register("transferNFT", new TransferNftAction(this.wax));
    }

    // Handles the page load event
    async onPageLoad() {
        function parseQuery(query) {
            return query.split('&').reduce((acc, param) => {
                let [key, value] = param.split('=');
                acc[decodeURIComponent(key)] = decodeURIComponent(value);
                return acc;
            }, {});
        }

        const hash = window.location.hash.substring(1);
        const [path, queryString] = hash.split('?');
        const queryParams = queryString ? parseQuery(queryString) : {};
        this.process(path, queryParams);
    }

    // Processes the provided path and parameters
    async process(path, params) {
        const result = await this.actionFactory.resolve(path, params);
        if (!result) {
            Messaging.sendMessage("error", {
                error: 'Unknown hash detected',
                data: {}
            })
            if (window) {
                window.close();
            }
            return;
        }
    }
}