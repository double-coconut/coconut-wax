using System;
using System.Collections.Generic;

namespace Wax.Tools
{
    /// <summary>
    /// The Utils class provides utility methods for handling common tasks.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Tries to parse the query parameters from the specified URL.
        /// </summary>
        /// <param name="url">The URL to parse the query parameters from.</param>
        /// <param name="queryDictionary">When this method returns, contains the query parameters as key-value pairs, if the conversion succeeded, or null if the conversion failed. The conversion fails if the url parameter is null, is an empty string (""), or does not contain valid query parameters. This parameter is passed uninitialized.</param>
        /// <returns>true if url was converted successfully; otherwise, false.</returns>
        public static bool TryParseQuery(string url, out IReadOnlyDictionary<string, string> queryDictionary)
        {
            try
            {
                // Create a dictionary to hold the key-value pairs
                Dictionary<string, string> queryDict = new Dictionary<string, string>();

                // Check if the URL contains query parameters
                if (url.Contains("?"))
                {
                    // Extract the query part
                    string query = url.Split('?')[1];

                    // Split the query string into key-value pairs
                    var queryParams = query.Split('&');

                    foreach (var param in queryParams)
                    {
                        // Split only on the first '=' character
                        var keyValue = param.Split(new[] { '=' }, 2);
                        if (keyValue.Length == 2)
                        {
                            string key = Uri.UnescapeDataString(keyValue[0]);
                            string value = Uri.UnescapeDataString(keyValue[1]);
                            queryDict[key] = value;
                        }
                    }
                }

                queryDictionary = queryDict;
            }
            catch
            {
                queryDictionary = null;
            }

            return queryDictionary != null;
        }
    }
}