using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    /// <summary>Includes HotReload Resources only in development builds</summary>
    /// <remarks>
    /// This build script ensures that HotReload Resources are not included in release builds.
    /// <para>
    /// When HotReload is enabled:<br/>
    ///   - include HotReloadSettingsObject in development Android builds.<br/>
    ///   - exclude HotReloadSettingsObject from the build.<br/>
    /// When HotReload is disabled:<br/>
    ///   - excludes HotReloadSettingsObject from the build.<br/>
    /// </para>
    /// </remarks>
#pragma warning disable CS0618
    internal class PrebuildIncludeResources : IPreprocessBuild, IPostprocessBuild {
#pragma warning restore CS0618
        public int callbackOrder => 10;

        // Preprocess warnings don't show up in console
        bool warnSettingsNotSupported;
        
        public void OnPreprocessBuild(BuildTarget target, string path) {
            try {
                if (HotReloadBuildHelper.IncludeInThisBuild()) {
                    // move scriptable object into Resources/ folder
                    HotReloadSettingsEditor.AddOrRemoveFromBuild(true);
                } else {
                    // make sure HotReload resources are not in the build
                    HotReloadSettingsEditor.AddOrRemoveFromBuild(false);
                    
                    var options = HotReloadSettingsEditor.LoadSettingsOrDefault();
                    var so = new SerializedObject(options);
                    if (IncludeInBuildOption.I.GetValue(so)) {
                        warnSettingsNotSupported = true;
                    }
                }
            } catch (BuildFailedException) {
                throw;
            } catch (Exception ex) {
                throw new BuildFailedException(ex);
            }
        }
        
        public void OnPostprocessBuild(BuildTarget target, string path) {
            if (warnSettingsNotSupported) {
                Debug.LogWarning("Hot Reload was not included in the build because one or more build settings were not supported.");
            }
        }

        // Do nothing in post build. settings asset will be dirty if build fails, so not worth fixing just for successful builds.
        // [PostProcessBuild]
        // private static void PostBuild(BuildTarget target, string pathToBuiltProject) {
        // }
    }

}
