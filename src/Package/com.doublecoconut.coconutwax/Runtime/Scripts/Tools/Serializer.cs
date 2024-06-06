using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Tools
{
    /// <summary>
    /// Provides methods for serializing and deserializing objects to and from JSON format.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Settings for JSON serialization and deserialization.
        /// </summary>
        private static readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Initializes the <see cref="Serializer"/> class.
        /// </summary>
        static Serializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">The formatting type.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string Serialize(object value, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(value, formatting, _serializerSettings);
        }

        /// <summary>
        /// Deserializes the JSON string to a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON string to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _serializerSettings);
        }

        /// <summary>
        /// Tries to serialize the specified object to a JSON string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="result">The resulting JSON string if the serialization is successful.</param>
        /// <param name="formatting">The formatting type.</param>
        /// <returns>true if the object was serialized successfully; otherwise, false.</returns>
        public static bool TrySerialize(object value, out string result, Formatting formatting = Formatting.Indented)
        {
            try
            {
                result = Serialize(value, formatting);
            }
            catch
            {
                result = null;
            }

            return result != null;
        }

        /// <summary>
        /// Tries to deserialize the JSON string to a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON string to deserialize.</param>
        /// <param name="result">The resulting object if the deserialization is successful.</param>
        /// <returns>true if the JSON string was deserialized successfully; otherwise, false.</returns>
        public static bool TryDeserialize<T>(string value, out T result)
        {
            try
            {
                result = Deserialize<T>(value);
                return true;
            }
            catch
            {
                result = default;
            }

            return false;
        }
    }
}