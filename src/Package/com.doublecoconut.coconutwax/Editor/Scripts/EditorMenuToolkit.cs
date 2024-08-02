using UnityEditor;

namespace CoconutWax.Editor.Scripts
{
    public static class EditorMenuToolkit
    {
        [MenuItem("Coconut Wax/Toolkit/Clear Web View Cookies")]
        private static void ClearWebViewCookies()
        {
            UniWebViewLogger.Instance.Info("Clearing WebView cookies...");
            UniWebView.ClearCookies();
        }
    }
}