/// <summary>
/// The Messaging class provides utility methods for sending messages and simulating delays.
/// </summary>
export class Messaging {
    /// <summary>
    /// Returns a promise that resolves after a specified number of milliseconds.
    /// </summary>
    static sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }


    /// <summary>
    /// Sends a message with a specific action and result. The result is encoded as a base64 string and appended to a URL.
    /// </summary>
    static async sendMessage(action, result) {
        await this.sleep(200);
        const encodedString = btoa(JSON.stringify(result));
        location.href = "uniwebview://result?action=" + action + "&payload=" + encodedString;
    }
}