using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {

	internal static class HotReloadSuggestionsHelper {
        internal static void SetSuggestionsShown(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (EditorPrefs.GetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}")) {
                return;
            }
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", true);
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}", true);
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                HotReloadState.ShowingRedDot = true;
            }
        }
        
        internal static bool CheckSuggestionActive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            return EditorPrefs.GetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}");
        }
        
        internal static bool CheckSuggestionShown(HotReloadSuggestionKind hotReloadSuggestionKind) {
            return EditorPrefs.GetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}");
        }

        internal static bool CanShowServerSuggestion(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (hotReloadSuggestionKind == HotReloadSuggestionKind.FieldInitializerWithSideEffects) {
                return !HotReloadState.ShowedFieldInitializerWithSideEffects;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.FieldInitializerExistingInstancesEdited) {
                return !HotReloadState.ShowedFieldInitializerExistingInstancesEdited;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.FieldInitializerExistingInstancesUnedited) {
                return !HotReloadState.ShowedFieldInitializerExistingInstancesUnedited;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.AddMonobehaviourMethod) {
                return !HotReloadState.ShowedAddMonobehaviourMethods;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.DetailedErrorReportingIsEnabled) {
                return !CheckSuggestionShown(HotReloadSuggestionKind.DetailedErrorReportingIsEnabled);
            }
            return false;
        }
        
        internal static void SetServerSuggestionShown(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (hotReloadSuggestionKind == HotReloadSuggestionKind.DetailedErrorReportingIsEnabled) {
                HotReloadSuggestionsHelper.SetSuggestionsShown(hotReloadSuggestionKind);
                return;
            } 
            if (hotReloadSuggestionKind == HotReloadSuggestionKind.FieldInitializerWithSideEffects) {
                HotReloadState.ShowedFieldInitializerWithSideEffects = true;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.FieldInitializerExistingInstancesEdited) {
                HotReloadState.ShowedFieldInitializerExistingInstancesEdited = true;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.FieldInitializerExistingInstancesUnedited) {
                HotReloadState.ShowedFieldInitializerExistingInstancesUnedited = true;
            } else if (hotReloadSuggestionKind == HotReloadSuggestionKind.AddMonobehaviourMethod) {
                HotReloadState.ShowedAddMonobehaviourMethods = true;
            } else {
                return;
            }
            HotReloadSuggestionsHelper.SetSuggestionActive(hotReloadSuggestionKind);
        }
        
        // used for cases where suggestion might need to be shown more than once
        internal static void SetSuggestionActive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            if (EditorPrefs.GetBool($"HotReloadWindow.SuggestionsShown.{hotReloadSuggestionKind}")) {
                return;
            }
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", true);
            
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                HotReloadState.ShowingRedDot = true;
            }
        }
        
        internal static void SetSuggestionInactive(HotReloadSuggestionKind hotReloadSuggestionKind) {
            EditorPrefs.SetBool($"HotReloadWindow.SuggestionsActive.{hotReloadSuggestionKind}", false);
            AlertEntry entry;
            if (suggestionMap.TryGetValue(hotReloadSuggestionKind, out entry)) {
                HotReloadTimelineHelper.Suggestions.Remove(entry);
            }
        }
        
        internal static void InitSuggestions() {
            foreach (HotReloadSuggestionKind value in Enum.GetValues(typeof(HotReloadSuggestionKind))) {
                if (!CheckSuggestionActive(value)) {
                    continue;
                }
                AlertEntry entry;
                if (suggestionMap.TryGetValue(value, out entry) && !HotReloadTimelineHelper.Suggestions.Contains(entry)) {
                    HotReloadTimelineHelper.Suggestions.Insert(0, entry);
                }
            }
        }
        
        internal static HotReloadSuggestionKind? FindSuggestionKind(AlertEntry targetEntry) {
            foreach (KeyValuePair<HotReloadSuggestionKind, AlertEntry> pair in suggestionMap) {
                if (pair.Value.Equals(targetEntry)) {
                    return pair.Key;
                }
            }
            return null;
        }
        
        internal static readonly OpenURLButton recompileTroubleshootingButton = new OpenURLButton("Docs", Constants.RecompileTroubleshootingURL);
        internal static readonly OpenURLButton featuresDocumentationButton = new OpenURLButton("Docs", Constants.FeaturesDocumentationURL);
        internal static readonly OpenURLButton multipleEditorsDocumentationButton = new OpenURLButton("Docs", Constants.MultipleEditorsURL);
        internal static readonly OpenURLButton debuggerDocumentationButton = new OpenURLButton("More Info", Constants.DebuggerURL);
        public static Dictionary<HotReloadSuggestionKind, AlertEntry> suggestionMap = new Dictionary<HotReloadSuggestionKind, AlertEntry> {
            { HotReloadSuggestionKind.UnityBestDevelopmentToolAward2023, new AlertEntry(
                AlertType.Suggestion, 
                "Vote for the \"Best Development Tool\" Award!", 
                "Hot Reload was nominated for the \"Best Development Tool\" Award. Please consider voting. Thank you!",
                actionData: () => {
                    GUILayout.Space(6f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Vote ")) {
                            Application.OpenURL(Constants.VoteForAwardURL);
                            SetSuggestionInactive(HotReloadSuggestionKind.UnityBestDevelopmentToolAward2023);
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.UnsupportedChanges, new AlertEntry(
                AlertType.Suggestion, 
                "Which changes does Hot Reload support?", 
                "Hot Reload supports most code changes, but there are some limitations. Generally, changes to methods and fields are supported. Things like adding new types is not (yet) supported. See the documentation for the list of current features and our current roadmap",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        featuresDocumentationButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.UnsupportedPackages, new AlertEntry(
                AlertType.Suggestion, 
                "Unsupported package detected",
                "The following packages are only partially supported: ECS, Mirror, Fishnet, and Photon. Hot Reload will work in the project, but changes specific to those packages might not hot-reload",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges, new AlertEntry(
                AlertType.Suggestion, 
                "Unity recompiles on enter/exit play mode?",
                "If you have an issue with the Unity Editor recompiling when the Play Mode state changes, more info is available in the docs. Feel free to reach out if you require assistance. We'll be glad to help.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        recompileTroubleshootingButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.discordButton.OnGUI();
                        GUILayout.Space(5f);
                        HotReloadAboutTab.contactButton.OnGUI();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
#if UNITY_2022_1_OR_NEWER
            { HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022, new AlertEntry(
                AlertType.Suggestion, 
                "Unsupported setting detected",
                "The 'Sprite Packer Mode' setting can cause unintended recompilations if set to 'Sprite Atlas V1 - Always Enabled'",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Use \"Build Time Only Atlas\" ")) {
                            if (EditorSettings.spritePackerMode == SpritePackerMode.SpriteAtlasV2) {
                                EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2Build;
                            } else {
                                EditorSettings.spritePackerMode = SpritePackerMode.BuildTimeOnlyAtlas;
                            }
                        }
                        if (GUILayout.Button(" Open Settings ")) {
                            SettingsService.OpenProjectSettings("Project/Editor");
                        }
                        if (GUILayout.Button(" Ignore suggestion ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
                        }
                        
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                hasExitButton: false
            )},
#endif
            { HotReloadSuggestionKind.MultidimensionalArrays, new AlertEntry(
                AlertType.Suggestion, 
                "Use jagged instead of multidimensional arrays",
                "Hot Reload doesn't support methods with multidimensional arrays ([,]). You can work around this by using jagged arrays ([][])",
                iconType: AlertType.UnsupportedChange,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Learn more ")) {
                            Application.OpenURL("https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1814");
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout
            )},
            { HotReloadSuggestionKind.EditorsWithoutHRRunning, new AlertEntry(
                AlertType.Suggestion, 
                "Some Unity instances don't have Hot Reload running.",
                "Make sure that either: \n1) Hot Reload is installed and running on all Editor instances, or \n2) Hot Reload is stopped in all Editor instances where it is installed.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Stop Hot Reload ")) {
                            EditorCodePatcher.StopCodePatcher().Forget();
                        }
                        GUILayout.Space(5f);
                        
                        multipleEditorsDocumentationButton.OnGUI();
                        GUILayout.Space(5f);
                        
                        if (GUILayout.Button(" Don't show again ")) {
                            HotReloadSuggestionsHelper.SetSuggestionsShown(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                            HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.UnsupportedChange
            )},
            // Not in use (never reported from the server)
            { HotReloadSuggestionKind.FieldInitializerWithSideEffects, new AlertEntry(
                AlertType.Suggestion, 
                "Field initializer with side-effects detected",
                "A field initializer update might have side effects, e.g. calling a method or creating an object.\n\nWhile Hot Reload does support this, it can sometimes be confusing when the initializer logic runs at 'unexpected times'.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" OK ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.FieldInitializerWithSideEffects);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Don't show again ")) {
                            SetSuggestionsShown(HotReloadSuggestionKind.FieldInitializerWithSideEffects);
                            SetSuggestionInactive(HotReloadSuggestionKind.FieldInitializerWithSideEffects);
                        }
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.Suggestion
            )},
            { HotReloadSuggestionKind.DetailedErrorReportingIsEnabled, new AlertEntry(
                AlertType.Suggestion, 
                "Detailed error reporting is enabled",
                "When an error happens in Hot Reload, the exception stacktrace is sent as telemetry to help diagnose and fix the issue.\nThe exception stack trace is only included if it originated from the Hot Reload package or binary. Stacktraces from your own code are not sent.\nYou can disable detailed error reporting to prevent telemetry from including any information about your project.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.Space(4f);
                        if (GUILayout.Button("    OK    ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.DetailedErrorReportingIsEnabled);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Disable ")) {
                            HotReloadSettingsTab.DisableDetailedErrorReportingInner(true);
                            SetSuggestionInactive(HotReloadSuggestionKind.DetailedErrorReportingIsEnabled);
                        }
                        GUILayout.Space(10f);
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.Suggestion
            )},
            // Not in use (never reported from the server)
            { HotReloadSuggestionKind.FieldInitializerExistingInstancesEdited, new AlertEntry(
                AlertType.Suggestion, 
                "Field initializer edit updated the value of existing class instances",
                "By default, Hot Reload updates field values of existing object instances when new field initializer has constant value.\n\nIf you want to change this behavior, disable the \"Apply field initializer edits to existing class instances\" option in Settings or click the button below.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Turn off ")) {
                            #pragma warning disable CS0618
                            HotReloadSettingsTab.ApplyApplyFieldInitializerEditsToExistingClassInstances(false);
                            #pragma warning restore CS0618
                            SetSuggestionInactive(HotReloadSuggestionKind.FieldInitializerExistingInstancesEdited);
                        }
                        if (GUILayout.Button(" Open Settings ")) {
                            HotReloadWindow.Current.SelectTab(typeof(HotReloadSettingsTab));
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Don't show again ")) {
                            SetSuggestionsShown(HotReloadSuggestionKind.FieldInitializerExistingInstancesEdited);
                            SetSuggestionInactive(HotReloadSuggestionKind.FieldInitializerExistingInstancesEdited);
                        }
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.Suggestion
            )},
            { HotReloadSuggestionKind.FieldInitializerExistingInstancesUnedited, new AlertEntry(
                AlertType.Suggestion, 
                "Field initializer edits don't apply to existing objects",
                "By default, Hot Reload applies field initializer edits of existing fields only to new objects (newly instantiated classes), just like normal C#.\n\nFor rapid prototyping, you can use static fields which will update across all instances.",
                actionData: () => {
                    GUILayout.Space(8f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" OK ")) {
                            SetSuggestionsShown(HotReloadSuggestionKind.FieldInitializerExistingInstancesUnedited);
                            SetSuggestionInactive(HotReloadSuggestionKind.FieldInitializerExistingInstancesUnedited);
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.Suggestion
            )},
            { HotReloadSuggestionKind.AddMonobehaviourMethod, new AlertEntry(
                AlertType.Suggestion, 
                "New MonoBehaviour methods are not shown in the inspector",
                "New methods in MonoBehaviours are not shown in the inspector until the script is recompiled. This is a limitation of Hot Reload handling of Unity's serialization system.\n\nYou can use the button below to auto recompile partially supported changes such as this one.",
                actionData: () => {
                    GUILayout.Space(8f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" OK ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.AddMonobehaviourMethod);
                        }
                        if (GUILayout.Button(" Auto Recompile ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.AddMonobehaviourMethod);
                            HotReloadPrefs.AutoRecompilePartiallyUnsupportedChanges = true;
                            HotReloadPrefs.DisplayNewMonobehaviourMethodsAsPartiallySupported = true;
                            HotReloadRunTab.RecompileWithChecks();
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Don't show again ")) {
                            SetSuggestionsShown(HotReloadSuggestionKind.AddMonobehaviourMethod);
                            SetSuggestionInactive(HotReloadSuggestionKind.AddMonobehaviourMethod);
                        }
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.Suggestion
            )},
#if UNITY_2020_1_OR_NEWER
            { HotReloadSuggestionKind.SwitchToDebugModeForInlinedMethods, new AlertEntry(
                AlertType.Suggestion, 
                "Switch code optimization to Debug Mode",
                "In Release Mode some methods are inlined, which prevents Hot Reload from applying changes. A clear warning is always shown when this happens, but you can use Debug Mode to avoid the issue altogether",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Switch to Debug mode ") && HotReloadRunTab.ConfirmExitPlaymode("Switching code optimization will stop Play Mode.\n\nDo you wish to proceed?")) {
                            HotReloadRunTab.SwitchToDebugMode();
                        }
                        GUILayout.FlexibleSpace();
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.UnsupportedChange
            )},
#endif
            { HotReloadSuggestionKind.HotReloadWhileDebuggerIsAttached, new AlertEntry(
                AlertType.Suggestion, 
                "Hot Reload is disabled while a debugger is attached",
                "Hot Reload automatically disables itself while a debugger is attached, as it can otherwise interfere with certain debugger features.\nWhile disabled, every code change will trigger a full Unity recompilation.\n\nYou can choose to keep Hot Reload enabled while a debugger is attached, though some features like debugger variable inspection might not always work as expected.",
                actionData: () => {
                    GUILayout.Space(8f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Keep enabled during debugging ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.HotReloadWhileDebuggerIsAttached);
                            HotReloadPrefs.AutoDisableHotReloadWithDebugger = false;
                        }
                        GUILayout.FlexibleSpace();
                        debuggerDocumentationButton.OnGUI();
                        if (GUILayout.Button(" Don't show again ")) {
                            SetSuggestionsShown(HotReloadSuggestionKind.HotReloadWhileDebuggerIsAttached);
                            SetSuggestionInactive(HotReloadSuggestionKind.HotReloadWhileDebuggerIsAttached);
                        }
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.Suggestion
            )},
            { HotReloadSuggestionKind.HotReloadedMethodsWhenDebuggerIsAttached, new AlertEntry(
                AlertType.Suggestion, 
                "Hot Reload may interfere with your debugger session",
                "Some debugger features, like variable inspection, might not work as expected for methods patched during the Hot Reload session. A full Unity recompile is required to get the full debugger experience.",
                actionData: () => {
                    GUILayout.Space(8f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button(" Recompile ")) {
                            SetSuggestionInactive(HotReloadSuggestionKind.HotReloadedMethodsWhenDebuggerIsAttached);
                            if (HotReloadRunTab.ConfirmExitPlaymode("Using the Recompile button will stop Play Mode.\n\nDo you wish to proceed?")) {
                                HotReloadRunTab.Recompile();
                            }
                        }
                        GUILayout.FlexibleSpace();
                        debuggerDocumentationButton.OnGUI();
                        GUILayout.Space(8f);
                    }
                },
                timestamp: DateTime.Now,
                entryType: EntryType.Foldout,
                iconType: AlertType.UnsupportedChange,
                hasExitButton: false
            )},
            
        };
        
        static ListRequest listRequest;
        static string[] unsupportedPackages = new[] {
            "com.unity.entities",
            "com.firstgeargames.fishnet",
        };
        static List<string> unsupportedPackagesList;
        static DateTime lastPlaymodeChange;
        
        public static void Init() {
            listRequest = Client.List(offlineMode: false, includeIndirectDependencies: true);

            EditorApplication.playModeStateChanged += state => {
                lastPlaymodeChange = DateTime.UtcNow;
            };
            CompilationPipeline.compilationStarted += obj => {
                if (DateTime.UtcNow - lastPlaymodeChange < TimeSpan.FromSeconds(1) && !HotReloadState.RecompiledUnsupportedChangesOnExitPlaymode) {
                    
#if UNITY_2022_1_OR_NEWER
                    SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
#else
                    SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges);
#endif
                }
                HotReloadState.RecompiledUnsupportedChangesOnExitPlaymode = false;
            };
            InitSuggestions();
        }

        private static DateTime lastCheckedUnityInstances = DateTime.UtcNow;
        public static void Check() {
            if (listRequest.IsCompleted && 
                unsupportedPackagesList == null) 
            {
                unsupportedPackagesList = new List<string>();
                if (listRequest.Result != null) {
                    foreach (var packageInfo in listRequest.Result) {
                        if (unsupportedPackages.Contains(packageInfo.name)) {
                            unsupportedPackagesList.Add(packageInfo.name);
                        }
                    }
                }
                if (unsupportedPackagesList.Count > 0) {
                    SetSuggestionsShown(HotReloadSuggestionKind.UnsupportedPackages);
                }
            }
            
            CheckEditorsWithoutHR();

#if UNITY_2022_1_OR_NEWER
            if (EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOnAtlas || EditorSettings.spritePackerMode == SpritePackerMode.SpriteAtlasV2) {
                SetSuggestionsShown(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
            } else if (CheckSuggestionActive(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022)) { 
                SetSuggestionInactive(HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022);
                EditorPrefs.SetBool($"HotReloadWindow.SuggestionsShown.{HotReloadSuggestionKind.AutoRecompiledWhenPlaymodeStateChanges2022}", false);
            }
#endif
        }
        
        private static void CheckEditorsWithoutHR() {
            if (!ServerHealthCheck.I.IsServerHealthy) {
                HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                return;
            }
            if (checkingEditorsWihtoutHR || 
                (DateTime.UtcNow - lastCheckedUnityInstances).TotalSeconds < 5)
            {
                return;
            }
            CheckEditorsWithoutHRAsync().Forget();
        }

        static bool checkingEditorsWihtoutHR;
        private static async Task CheckEditorsWithoutHRAsync() {
            try {
                checkingEditorsWihtoutHR = true;
                var showSuggestion = await Task.Run(() => {
                    try {
                        var runningUnities = Process.GetProcessesByName("Unity Editor").Length;
                        var runningPatchers = Process.GetProcessesByName("CodePatcherCLI").Length;
                        return runningPatchers > 0 && runningUnities > runningPatchers;
                    } catch (ArgumentException) {
                        // On some devices GetProcessesByName throws ArgumentException for no good reason.
                        // it happens rarely and the feature is not the most important so proper solution is not required
                        return false;
                    }
                });
                if (!showSuggestion) {
                    HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                    return;
                }
                if (!HotReloadState.ShowedEditorsWithoutHR && ServerHealthCheck.I.IsServerHealthy) {
                    HotReloadSuggestionsHelper.SetSuggestionActive(HotReloadSuggestionKind.EditorsWithoutHRRunning);
                    HotReloadState.ShowedEditorsWithoutHR = true;
                }
            } finally {
                checkingEditorsWihtoutHR = false;
                lastCheckedUnityInstances = DateTime.UtcNow;
            }
        }
	}
}
