using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CoconutWax.Editor.Scripts
{
    public class CoconutWaxEditorWindow : EditorWindow
    {
        [Serializable]
        private class PackageJson
        {
            public string displayName;
            public string version;
            public string description;
        }

        public const string PackageName = "com.doublecoconut.coconutwax";

        private const string _sampleName = "CoconutWaxWallet";
        private string _samplePath => $"{PackagePathLocator.PackageBasePath}/Samples~/{_sampleName}";
        private string _sampleDestinationPath => $"Assets/Samples/{PackageName}/{_sampleName}";

        private const string _streamingAssetsFolder = "StreamingAssets";
        private static readonly string _destinationFolder = $"Assets/{_streamingAssetsFolder}";

        private string _uniWebViewPackagePath =>
            $"{PackagePathLocator.PackageBasePath}/Packages/UniWebView_DLL_Packages.unitypackage";

        private static PackageJson _packageJson;
        public static bool IsSetupDone => Directory.Exists(_destinationFolder);
        private bool _sampleImported;


        [MenuItem("Coconut Wax/Setup")]
        public static async void ShowWindow()
        {
            bool success = await FetchPackage();
            if (!success)
            {
                return;
            }

            GetWindow<CoconutWaxEditorWindow>($"{_packageJson.displayName} Editor");
        }

        private static async Task<bool> FetchPackage()
        {
            EditorUtility.DisplayProgressBar("Coconut Wax", "Preparing... ", 0.75f);
            try
            {
                await PackagePathLocator.RequestPackagePath(PackageName);
                EditorUtility.DisplayProgressBar("Coconut Wax", "Preparing... ", 1.0f);
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
                var packageJsonPath = $"{PackagePathLocator.PackageBasePath}/package.json";
                _packageJson = ReadPackageJson(packageJsonPath);
            }
            catch (Exception e)
            {
                bool canShowMore = EditorUtility.DisplayDialog("Error",
                    "An error occurred while locating the package path.", "Show more", "Ok");
                if (canShowMore)
                {
                    EditorUtility.DisplayDialog("Error - More Info",
                        $"An error occurred while locating the package path: {e.Message}", "Ok");
                }

                return false;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return true;
        }

        private static PackageJson ReadPackageJson(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PackageJson packageJson = JsonUtility.FromJson<PackageJson>(json);
                return packageJson;
            }

            Debug.LogError($"package.json not found at path: {path}");
            return null;
        }


        private async void OnEnable()
        {
            if (_packageJson == null)
            {
                bool success = await FetchPackage();
                if (!success)
                {
                    Close();
                    return;
                }
            }

            _sampleImported = Directory.Exists(_sampleDestinationPath);
        }


        private void ImportSample(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }

                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }

                AssetDatabase.Refresh();
                Debug.Log($"Sample '{_sampleName}' imported successfully to '{destinationPath}'.");
            }
            else
            {
                Debug.LogError($"Sample path '{sourcePath}' does not exist.");
            }
        }


        private void OnGUI()
        {
            if (_packageJson == null)
            {
                GUIStyle notFoundStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                notFoundStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("Loading...", notFoundStyle);
                return;
            }

            // Draw title
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.alignment = TextAnchor.UpperCenter;
            titleStyle.fontSize = 20;
            GUILayout.Label(_packageJson?.displayName ?? "Name not found.", titleStyle, GUILayout.ExpandWidth(true));

            // Draw description message above the button
            GUILayout.FlexibleSpace();
            GUIStyle descriptionStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            descriptionStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(_packageJson?.description ?? "Description not found.", descriptionStyle);

            // Draw setup button centered in the middle of the screen
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(IsSetupDone);
            if (GUILayout.Button("Setup", GUILayout.Width(200), GUILayout.Height(40)))
            {
                ImportUnityPackage(_uniWebViewPackagePath);
                CopyFolder(Path.Combine(PackagePathLocator.PackageBasePath, _streamingAssetsFolder),
                    _destinationFolder);
                AssetDatabase.Refresh();
                Debug.Log("Setup completed: Folder copied");
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(_sampleImported || !IsSetupDone);
            if (GUILayout.Button("Import Sample", GUILayout.Width(200), GUILayout.Height(40)))
            {
                ImportSample(_samplePath, _sampleDestinationPath);
                AssetDatabase.Refresh();
                _sampleImported = true;
                Debug.Log($"Import completed: Sample imported: {_sampleDestinationPath}");
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (IsSetupDone)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle messageStyle = new GUIStyle(EditorStyles.label);
                messageStyle.alignment = TextAnchor.MiddleCenter;
                messageStyle.normal.textColor = Color.green;
                GUILayout.Label("Package already configured!", messageStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (_sampleImported)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUIStyle messageStyle = new GUIStyle(EditorStyles.label);
                messageStyle.alignment = TextAnchor.MiddleCenter;
                messageStyle.normal.textColor = Color.green;
                GUILayout.Label("Sample already imported!", messageStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();


            // Draw version in bottom right corner
            GUIStyle versionStyle = new GUIStyle(EditorStyles.label);
            versionStyle.alignment = TextAnchor.LowerRight;
            versionStyle.fontSize = 10;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_packageJson?.version ?? "Version not found!", versionStyle);
            GUILayout.EndHorizontal();
        }

        private void CopyFolder(string source, string destination)
        {
            if (!Directory.Exists(source))
            {
                Debug.LogError("Source folder does not exist: " + source);
                return;
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            foreach (string file in Directory.GetFiles(source))
            {
                string destFile = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string folder in Directory.GetDirectories(source))
            {
                string destFolder = Path.Combine(destination, Path.GetFileName(folder));
                CopyFolder(folder, destFolder);
            }
        }

        private void ImportUnityPackage(string packagePath)
        {
            if (File.Exists(packagePath))
            {
                AssetDatabase.ImportPackage(packagePath, false);
                Debug.Log("Package imported successfully.");
            }
            else
            {
                Debug.LogError("Package not found at path: " + packagePath);
            }
        }
    }
}