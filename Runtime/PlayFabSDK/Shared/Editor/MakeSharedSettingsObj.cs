using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    [InitializeOnLoad]
    public static class PlayFabResourceInstaller
    {
        private const string ResourcesPath = "Assets/Resources";
        private const string PlayFabPath = ResourcesPath + "/PlayFab";
        private const string AssetPath = PlayFabPath + "/PlayFabSharedSettings.asset";

        static PlayFabResourceInstaller()
        {
            EnsureSharedSettingsExists();
        }

        [MenuItem("PlayFab/Create Shared Settings")]
        public static void EnsureSharedSettingsExists()
        {
            var guids = AssetDatabase.FindAssets("t:PlayFabSharedSettings", new[] { "Assets" });
            if (guids.Length > 0)
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(ResourcesPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            if (!AssetDatabase.IsValidFolder(PlayFabPath))
            {
                AssetDatabase.CreateFolder(ResourcesPath, "PlayFab");
            }

            var asset = ScriptableObject.CreateInstance<PlayFabSharedSettings>();
            AssetDatabase.CreateAsset(asset, AssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("PlayFab: Created PlayFabSharedSettings at " + AssetPath);
        }
    }
}
