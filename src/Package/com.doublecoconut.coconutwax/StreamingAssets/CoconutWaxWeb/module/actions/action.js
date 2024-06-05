import {Messaging} from "../tools/messaging.js";

/// <summary>
/// The Action class is used to process actions in the application. It initializes the wax and action properties and sets up a result object.
/// </summary>
export class Action {
    constructor(wax, action) {
        this.wax = wax;
        this.action = action;
        this.result = {
            error: '',
            data: {}
        };
    }

    /// <summary>
    /// Handles the action. It calls the internalHandler method to handle the action and sends the result as a message.
    /// </summary>
    async process(params) {
        try {
            await this.internalHandler(params);
        } catch (e) {
            this.result.error = e.message;
        }

        await Messaging.sendMessage(this.action, this.result);
    }

    /// <summary>
    /// An empty method that is meant to be overridden in subclasses of Action.
    /// </summary>
    async internalHandler(params) {
    }
}

