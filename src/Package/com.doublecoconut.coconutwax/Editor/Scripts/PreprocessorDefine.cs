using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace CoconutWax.Editor.Scripts
{
    static class PreprocessorDefine
    {
        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void AddDefineSymbols()
        {
            // we want to do this when the package setup is done
            if (!CoconutWaxEditorWindow.IsSetupDone)
            {
                return;
            }

#if UNITY_2021_2_OR_NEWER
            string currentDefines = PlayerSettings
                .GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget
                    .FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
#else
            // Deprecated in Unity 2023.1
            string currentDefines = PlayerSettings
                .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            HashSet<string> defines = new HashSet<string>(currentDefines.Split(';'))
            {
                "UNIWEBVIEW",
                "COCONUT_WAX"
            };

            // only touch PlayerSettings if we actually modified it,
            // otherwise it shows up as changed in git each time.
            string newDefines = string.Join(";", defines);
            if (newDefines != currentDefines)
            {
#if UNITY_2021_2_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(
                    UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings
                        .selectedBuildTargetGroup), newDefines);
#else
                // Deprecated in Unity 2023.1
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
#endif
            }
        }
    }
}