using Tools;
using UnityEditor;
using UnityEngine;
using Wax.Payload;

namespace Samples.CoconutWaxWallet.Scripts
{
    /// <summary>
    /// The Session class is used to manage the user's session data in the application.
    /// </summary>
    public static class Session
    {
        private const string UserAccountKey = "userAccount";
        public static AuthenticationPayloadData UserAccount { get; set; }
        public static bool HasUserAccount => UserAccount != null;
        /// <summary>
        /// Initializes the session. If the UserAccountKey exists in PlayerPrefs, it deserializes the stored string into an AuthenticationPayloadData object and assigns it to the UserAccount property.
        /// </summary>
        public static void Setup()
        {
            if (PlayerPrefs.HasKey(UserAccountKey))
            {
                UserAccount = Serializer.Deserialize<AuthenticationPayloadData>(PlayerPrefs.GetString(UserAccountKey));
            }
        }
        /// <summary>
        /// Saves the user's account data. It serializes the UserAccount object into a string and stores it in PlayerPrefs with the UserAccountKey.
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.SetString(UserAccountKey, Serializer.Serialize(UserAccount));
        }
        /// <summary>
        /// Clears the user's session. It sets the UserAccount property to null and removes the UserAccountKey from PlayerPrefs.
        /// </summary>
        public static void Delete()
        {
            UserAccount = null;
            PlayerPrefs.DeleteKey(UserAccountKey);
            PlayerPrefs.Save();
        }

#if UNITY_EDITOR
        [MenuItem("Coconut Wax/Sample/Clear User Session")]
        public static void ClearUserEditorTool()
        {
            Delete();
            Logger.CoconutWaxLogger.Log("[EDITOR]: User Session Cleared", Logger.LogType.Verbose);
        }
#endif
    }
}