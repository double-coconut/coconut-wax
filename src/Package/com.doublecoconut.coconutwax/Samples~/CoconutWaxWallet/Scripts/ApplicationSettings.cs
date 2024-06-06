using Samples.CoconutWaxWallet.Scripts.Data;
using Samples.CoconutWaxWallet.Scripts.UI.Items;
using Tools;
using UnityEditor;
using UnityEngine;

namespace Samples.CoconutWaxWallet.Scripts
{
    /// <summary>
    /// The ApplicationSettings class is used to manage the application's settings, specifically the token settings.
    /// </summary>
    public static class ApplicationSettings
    {
        private const string TokenSettingsKey = "tokenSettings";
        public static TokenSettings TokenSettings { get; private set; }
        /// <summary>
        /// Initializes the application settings. If the TokenSettingsKey exists in PlayerPrefs, it deserializes the stored string into a TokenSettings object and assigns it to the TokenSettings property. If the key does not exist, it creates default token settings and saves them.
        /// </summary>
        public static void Setup()
        {
            if (PlayerPrefs.HasKey(TokenSettingsKey))
            {
                TokenSettings = Serializer.Deserialize<TokenSettings>(PlayerPrefs.GetString(TokenSettingsKey));
            }
            else
            {
                TokenSettings= TokenSettings.CreateDefault();
                Save();
            }
        }
        /// <summary>
        /// Saves the token settings. It serializes the TokenSettings object into a string and stores it in PlayerPrefs with the TokenSettingsKey.
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.SetString(TokenSettingsKey, Serializer.Serialize(TokenSettings));
        }
        /// <summary>
        /// Clears the application settings. It sets the TokenSettings property to null and removes the TokenSettingsKey from PlayerPrefs.
        /// </summary>
        public static void Delete()
        {
            TokenSettings = null;
            PlayerPrefs.DeleteKey(TokenSettingsKey);
            PlayerPrefs.Save();
        }

#if UNITY_EDITOR
        [MenuItem("Coconut Wax/Sample/Reset Token Settings")]
        public static void ClearTokenSettingsTool()
        {
            Delete();
            Logger.CoconutWaxLogger.Log("[EDITOR]: Token Settings Cleared", Logger.LogType.Verbose);
        }
#endif
    }
}