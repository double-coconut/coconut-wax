using System;
using Newtonsoft.Json;

namespace RestClient.Response
{
    /// <summary>
    /// Represents a basic response from a REST API. This class contains a single property, Success, 
    /// which indicates whether the API request was successful or not.
    /// </summary>
    [Serializable]
    public class BaseResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the API request was successful or not.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}