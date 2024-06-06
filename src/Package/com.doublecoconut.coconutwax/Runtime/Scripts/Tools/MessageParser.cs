using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wax.Tools;

namespace Tools
{
    public static class MessageParser
    {
        /// <summary>
        /// Parses the raw message into a specific payload data type.
        /// </summary>
        /// <typeparam name="TPayloadData">The type of the payload data.</typeparam>
        /// <param name="rawMessage">The raw message to parse.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the parsed payload data.</returns>
        public static Task<TPayloadData> ParseMessage<TPayloadData>(string rawMessage)
        {
            TaskCompletionSource<TPayloadData> tcs = new TaskCompletionSource<TPayloadData>();
            if (!rawMessage.Contains("result"))
            {
                tcs.TrySetException(new Exception($"Message does not contain result: {rawMessage}"));
                return tcs.Task;
            }

            if (!Utils.TryParseQuery(rawMessage, out IReadOnlyDictionary<string, string> queryDictionary))
            {
                tcs.TrySetException(new Exception("Failed to parse query"));
                return tcs.Task;
            }

            const string actionKey = "action";
            const string payloadKey = "payload";

            if (!queryDictionary.ContainsKey(actionKey) || !queryDictionary.ContainsKey(payloadKey))
            {
                tcs.TrySetException(new Exception("Missing action or payload"));
                return tcs.Task;
            }

            string action = queryDictionary[actionKey];
            string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(queryDictionary[payloadKey]));

            if (!Serializer.TryDeserialize(payloadJson, out Payload payload))
            {
                tcs.TrySetException(new Exception("Failed to deserialize payload"));
                return tcs.Task;
            }

            if (payload.HasError)
            {
                tcs.TrySetException(new Exception($"Payload Error: {payload.Error}"));
                return tcs.Task;
            }

            TPayloadData payloadData = payload.Data.ToObject<TPayloadData>();
            tcs.SetResult(payloadData);
            return tcs.Task;
        }
    }
}