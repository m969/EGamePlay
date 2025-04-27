using System;
using System.Collections.Generic;
using System.Reflection;
using SingularityGroup.HotReload.HarmonyLib;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    using IndicationStatus = EditorIndicationState.IndicationStatus;
    
    // Before Unity 2021.3, value is 0 or 1. Only value of 1 is a problem.
    // From Unity 2021.3 onwards, the key is "kAutoRefreshMode".
    // kAutoRefreshMode options are:
    //   0: disabled
    //   1: enabled 
    //   2: enabled outside playmode
    // 
    // On newer Unity versions, Visual Studio is also checking the kAutoRefresh setting (but it should only check kAutoRefreshMode).
    // This is making hot reload unusable and so this setting needs to also get disabled.
    internal static class AutoRefreshSettingChecker {
        const string autoRefreshKey = "kAutoRefresh";
        #if UNITY_2021_3_OR_NEWER
        const string autoRefreshModeKey = "kAutoRefreshMode";
        #endif
        
        const int desiredValue = 0;

        public static void Apply() {
            if (HotReloadPrefs.AppliedAutoRefresh) {
                return;
            }
            
            var defaultPref = EditorPrefs.GetInt(autoRefreshKey);
            HotReloadPrefs.DefaultAutoRefresh = defaultPref;
            EditorPrefs.SetInt(autoRefreshKey, desiredValue);
            
            #if UNITY_2021_3_OR_NEWER
            var defaultModePref = EditorPrefs.GetInt(autoRefreshModeKey);
            HotReloadPrefs.DefaultAutoRefreshMode = defaultModePref;
            EditorPrefs.SetInt(autoRefreshModeKey, desiredValue);
            #endif

            HotReloadPrefs.AppliedAutoRefresh = true;
        }

        public static void Check() {
            if (!HotReloadPrefs.AppliedAutoRefresh) {
                return;
            }
            
            if (EditorPrefs.GetInt(autoRefreshKey) != desiredValue) {
                HotReloadPrefs.DefaultAutoRefresh = -1;
            }
            
            #if UNITY_2021_3_OR_NEWER
            if (EditorPrefs.GetInt(autoRefreshModeKey) != desiredValue) {
                HotReloadPrefs.DefaultAutoRefreshMode = -1;
            }
            #endif
        }

        public static void Reset() {
            if (!HotReloadPrefs.AppliedAutoRefresh) {
                return;
            }
            
            if (EditorPrefs.GetInt(autoRefreshKey) == desiredValue
                && HotReloadPrefs.DefaultAutoRefresh != -1
            ) {
                EditorPrefs.SetInt(autoRefreshKey, HotReloadPrefs.DefaultAutoRefresh);
            }
            HotReloadPrefs.DefaultAutoRefresh = -1;
            
            #if UNITY_2021_3_OR_NEWER
            if (EditorPrefs.GetInt(autoRefreshModeKey) == desiredValue 
                && HotReloadPrefs.DefaultAutoRefreshMode != -1
            ) {
                EditorPrefs.SetInt(autoRefreshModeKey, HotReloadPrefs.DefaultAutoRefreshMode);
            }
            HotReloadPrefs.DefaultAutoRefreshMode = -1;
            #endif

            HotReloadPrefs.AppliedAutoRefresh = false;
        }
    }
    
    internal static class ScriptCompilationSettingChecker {
        const string scriptCompilationKey = "ScriptCompilationDuringPlay";
        
        const int recompileAndContinuePlaying = 0;
        static int? recompileAfterFinishedPlaying = (int?)typeof(EditorWindow).Assembly.GetType("UnityEditor.ScriptChangesDuringPlayOptions")?
            .GetField("RecompileAfterFinishedPlaying", BindingFlags.Static | BindingFlags.Public)?
            .GetValue(null);

        public static void Apply() {
            if (HotReloadPrefs.AppliedScriptCompilation) {
                return;
            }
            
            var defaultPref = EditorPrefs.GetInt(scriptCompilationKey);
            HotReloadPrefs.DefaultScriptCompilation = defaultPref;
            EditorPrefs.SetInt(scriptCompilationKey, GetRecommendedAutoScriptCompilationKey());

            HotReloadPrefs.AppliedScriptCompilation = true;
        }
        
        public static void Check() {
            if (!HotReloadPrefs.AppliedScriptCompilation) {
                return;
            }
            if (EditorPrefs.GetInt(scriptCompilationKey) != GetRecommendedAutoScriptCompilationKey()) {
                HotReloadPrefs.DefaultScriptCompilation = -1;
            }
        }

        public static void Reset() {
            if (!HotReloadPrefs.AppliedScriptCompilation) {
                return;
            }
            if (EditorPrefs.GetInt(scriptCompilationKey) == GetRecommendedAutoScriptCompilationKey()
                && HotReloadPrefs.DefaultScriptCompilation != -1
            ) {
                EditorPrefs.SetInt(scriptCompilationKey, HotReloadPrefs.DefaultScriptCompilation);
            }
            HotReloadPrefs.DefaultScriptCompilation = -1;
            
            HotReloadPrefs.AppliedScriptCompilation = false;
        }
        
        static int GetRecommendedAutoScriptCompilationKey() {
            // In some projects due to an unknown reason both "RecompileAndContinuePlaying" and "StopPlayingAndRecompile" cause issues
            // We were unable to identify the cause and therefore we always try to default to "RecompileAfterFinishedPlaying"
            // The exact issue users are experiencing is that domain reload happens shortly after entering play mode causing nullrefs
            return recompileAfterFinishedPlaying ?? recompileAndContinuePlaying;
        }
    }
    
    internal static class PlaymodeTintSettingChecker {
        private static readonly Color unsupportedPlaymodeColor = new Color(1f, 0.8f, 0f, 1f);
        private static readonly Color compilePlaymodeErrorColor = new Color(1f, 0.7f, 0.7f, 1f);
        
        public static void Apply() {
            if (HotReloadPrefs.AppliedEditorTint != null || !UnitySettingsHelper.I.playmodeTintSupported) {
                return;
            }
            var defaultPref = HotReloadPrefs.DefaultEditorTint ?? UnitySettingsHelper.I.GetCurrentPlaymodeColor();
            if (defaultPref == null) {
                return;
            }
            HotReloadPrefs.DefaultEditorTint = defaultPref.Value;
            var currentPlaymodeTint = GetModifiedPlaymodeTint() ?? defaultPref.Value;
            SetPlaymodeTint(currentPlaymodeTint);
        }
        
        public static void Check() {
            if (HotReloadPrefs.AppliedEditorTint == null || !UnitySettingsHelper.I.playmodeTintSupported) {
                return;
            }
            // if user modifies the settings manually, prevent the setting to be changed
            if (HotReloadPrefs.DefaultEditorTint == null || UnitySettingsHelper.I.GetCurrentPlaymodeColor() != HotReloadPrefs.AppliedEditorTint) {
                HotReloadPrefs.DefaultEditorTint = null;
                return;
            }
            var color = GetModifiedPlaymodeTint();
            if (color != null && color != HotReloadPrefs.AppliedEditorTint) {
                SetPlaymodeTint(color.Value);
            }
        }
        

        public static void Reset() {
            if (HotReloadPrefs.AppliedEditorTint == null || !UnitySettingsHelper.I.playmodeTintSupported) {
                return;
            }
            var color = HotReloadPrefs.DefaultEditorTint;
            if (color != null && UnitySettingsHelper.I.GetCurrentPlaymodeColor() == HotReloadPrefs.AppliedEditorTint) {
                SetPlaymodeTint(color.Value);
            }
            
            HotReloadPrefs.DefaultEditorTint = null;
            HotReloadPrefs.AppliedEditorTint = null;
        }
        
        
        private static void SetPlaymodeTint(Color color) {
            UnitySettingsHelper.I.SetPlaymodeTint(color);
            HotReloadPrefs.AppliedEditorTint = color;
        }

        private static Color? GetModifiedPlaymodeTint() {
            switch (EditorIndicationState.CurrentIndicationStatus) {
                case IndicationStatus.CompileErrors:
                    return compilePlaymodeErrorColor;
                case IndicationStatus.Unsupported:
                    return unsupportedPlaymodeColor;
                default:
                    return HotReloadPrefs.DefaultEditorTint;
            }
        }
    }
    
    internal static class CompileMethodDetourer {
        static bool detouredMethod;
        static List<IDisposable> reverters = new List<IDisposable>();

        public static void Apply() {
            if (detouredMethod) {
                return;
            }
            detouredMethod = true;

            var originAssetRefresh = typeof(AssetDatabase).GetMethod(nameof(AssetDatabase.Refresh), Type.EmptyTypes);
            var targetAssetRefresh = typeof(CompileMethodDetourer).GetMethod(nameof(DetouredAssetRefresh));

            DetourMethod(originAssetRefresh, targetAssetRefresh);
            
            var originAssetRefreshWithParams = typeof(AssetDatabase).GetMethod(nameof(AssetDatabase.Refresh), new[] { typeof(ImportAssetOptions) });
            var targetAssetRefreshWithParams = typeof(CompileMethodDetourer).GetMethod(nameof(DetouredAssetRefresh));

            DetourMethod(originAssetRefreshWithParams, targetAssetRefreshWithParams);
            
            var originCompilation = typeof(CompilationPipeline).GetMethod(nameof(CompilationPipeline.RequestScriptCompilation), Type.EmptyTypes);
            var targetCompilation = typeof(CompileMethodDetourer).GetMethod(nameof(RequestScriptCompilation));

            DetourMethod(originCompilation, targetCompilation);
        }

        static void DetourMethod(MethodBase original, MethodBase replacement) {
            DetourResult result;
            DetourApi.DetourMethod(original, replacement, out result);

            if (!result.success) {
                Debug.LogWarning($"Detouring {original.Name} method failed. {result.exception?.GetType()} {result.exception}");
            } else {
                reverters.Add(result.patchRecord);
            }
        }

        public static void Reset() {
            if (!detouredMethod) {
                return;
            }

            detouredMethod = false;

            // don't revert for now
            // foreach (var reverter in reverters) {
            //     try {
            //         reverter.Dispose(); 
            //     } catch (Exception exc) {
            //         Debug.LogWarning($"Reverting method detour failed. {exc.GetType()} {exc}");
            //     }
            // }

            reverters.Clear();

            // hack to undo changes to Editor assemblies.
            // Doing this when starting hotreload cancels the start
            // Exit playmode right away to prevent delayed compiling
            EditorApplication.isPlaying = false;
            
            EditorApplication.ExecuteMenuItem("Assets/Refresh");
            EditorUtility.RequestScriptReload(); //this will undo the modifications to the assemblies
        }

        public static void DetouredAssetRefresh(ImportAssetOptions options) { }
        public static void RequestScriptCompilation() { }
    }
}