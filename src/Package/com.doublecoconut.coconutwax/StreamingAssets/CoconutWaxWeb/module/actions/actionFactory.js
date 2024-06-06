/// <summary>
/// The ActionFactory class is used to manage and process actions in the application. It initializes an actions object to store registered actions.
/// </summary>
export class ActionFactory {
    constructor() {
        this.actions = {};
    }

    /// <summary>
    /// Registers a new action with a corresponding resolve function.
    /// </summary>
    register(action, resolve) {
        this.actions[action] = resolve;
    }

    /// <summary>
    /// Processes an action. It first checks if the action is registered. If the action is registered, it calls the process method of the registered action.
    /// </summary>
    async resolve(action, params) {
        if (!this.actions[action]) {
            return false;
        }
        await this.actions[action].process(params);
        return true;
    }
}