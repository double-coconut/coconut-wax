/// <summary>
/// The utilities.js file provides utility functions for getting user balance and formatting numbers.
/// </summary>

/// <summary>
/// Gets the user balance for a specific contract. The result is returned as an object containing the contract name, balance, and symbol.
/// </summary>
export async function getUserBalanceByContract(wax, tokenContract, accountName) {
    var response = {
        contract: tokenContract
    };
    try {
        const result = await wax.rpc.get_currency_balance(tokenContract, accountName);
        const balanceSplitted = result[0].split(' ');
        response.balance = parseFloat(balanceSplitted[0]);
        response.symbol = balanceSplitted[1];
    } catch (e) {
        response.balance = null;
    }
    return response;
}

/// <summary>
/// Gets the user balance for multiple contracts. It uses the getUserBalanceByContract function to get the balance for each contract.
/// </summary>
export async function getUserBalance(wax, tokenContracts, accountName) {
    let requests = [];
    for (let contract of tokenContracts) {
        requests.push(getUserBalanceByContract(wax, contract, accountName));
    }

    var result = await Promise.all(requests);
    return result.filter(item => item.balance != null);
}

/// <summary>
/// Formats a number to eight decimal places.
/// </summary>
export function formatToEightDecimalPlaces(input) {
    const num = parseFloat(input);
    return num.toFixed(8);
}