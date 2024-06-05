using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tools
{
    [Serializable]
    public class Payload
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the data of the payload.
        /// </summary>
        public JObject Data { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an error.
        /// </summary>
        [JsonIgnore]
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}