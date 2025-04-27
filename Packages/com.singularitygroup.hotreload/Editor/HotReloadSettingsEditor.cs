using System.IO;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    static class HotReloadSettingsEditor {
        /// Ensure settings asset file is created and saved
        public static void EnsureSettingsCreated(HotReloadSettingsObject asset) {
            if (!SettingsExists()) {
                CreateNewSettingsFile(asset, HotReloadSettingsObject.editorAssetPath);
            }
        }

        /// Load existing settings asset or return the default settings
        public static HotReloadSettingsObject LoadSettingsOrDefault() {
            if (SettingsExists()) {
                return AssetDatabase.LoadAssetAtPath<HotReloadSettingsObject>(HotReloadSettingsObject.editorAssetPath);
            } else {
                // create an instance with default values
                return ScriptableObject.CreateInstance<HotReloadSettingsObject>();
            }
        }

        /// <summary>
        /// Create settings asset file
        /// </summary>
        /// <remarks>Assume that settings asset doesn't exist yet</remarks>
        /// <returns>The settings asset</returns>
        static void CreateNewSettingsFile(HotReloadSettingsObject asset, string editorAssetPath) {
            // create new settings asset
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(editorAssetPath));
            if (asset == null) {
                asset = ScriptableObject.CreateInstance<HotReloadSettingsObject>();
            }
            AssetDatabase.CreateAsset(asset, editorAssetPath);
            // Saving the asset isn't needed right after you created it. Unity will save it at the appropriate time.
            // Troy: I tested in Unity 2018 LTS, first Android build creates the asset file and asset is included in the build.
        }

        #region include/exclude in build

        private static bool SettingsExists() {
            return AssetExists(HotReloadSettingsObject.editorAssetPath);
        }

        private static bool AssetExists(string assetPath) {
            return AssetDatabase.GetMainAssetTypeAtPath(assetPath) != null;
        }

        public static void AddOrRemoveFromBuild(bool includeSettingsInBuild) {
            AssetDatabase.StartAssetEditing();
            var so = LoadSettingsOrDefault();
            try {
                if (includeSettingsInBuild) {
                    // Note: don't need to force create settings because we know the defaults in player.
                    so.EnsurePrefabSetCorrectly();
                    EnsureSettingsCreated(so);
                } else {
                    // this block shouldn't create the asset file, but it's also fine if it does
                    so.EnsurePrefabNotInBuild();
                }
            } finally {
                AssetDatabase.StopAssetEditing();
            }
        }

        #endregion
    }
}