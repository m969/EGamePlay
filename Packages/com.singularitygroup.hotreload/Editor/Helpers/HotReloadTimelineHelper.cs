using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


namespace SingularityGroup.HotReload.Editor {
    internal enum TimelineType {
        Suggestions,
        Timeline,
    }
    
    internal enum AlertType {
        Suggestion,
        UnsupportedChange,
        CompileError,
        PartiallySupportedChange,
        AppliedChange,
        UndetectedChange,
    }
    
    internal enum AlertEntryType {
        Error,
        Failure,
        InlinedMethod,
        PatchApplied,
        PartiallySupportedChange,
        UndetectedChange,
    }
    
    internal enum EntryType {
        Parent,
        Child,
        Standalone,
        Foldout,
    }
    
    internal class PersistedAlertData {
        public readonly AlertData[] alertDatas;

        public PersistedAlertData(AlertData[] alertDatas) {
            this.alertDatas = alertDatas;
        }
    }

    internal class AlertData {
        public readonly AlertEntryType alertEntryType;
        public readonly string errorString;
        public readonly string methodName;
        public readonly string methodSimpleName;
        public readonly PartiallySupportedChange partiallySupportedChange;
        public readonly EntryType entryType;
        public readonly bool detiled;
        public readonly DateTime createdAt;
        public readonly string[] patchedMembersDisplayNames;

        public AlertData(AlertEntryType alertEntryType, DateTime createdAt, bool detiled = false, EntryType entryType = EntryType.Standalone, string errorString = null, string methodName = null, string methodSimpleName = null, PartiallySupportedChange partiallySupportedChange = default(PartiallySupportedChange), string[] patchedMembersDisplayNames = null) {
            this.alertEntryType = alertEntryType;
            this.createdAt = createdAt;
            this.detiled = detiled;
            this.entryType = entryType;
            this.errorString = errorString;
            this.methodName = methodName;
            this.methodSimpleName = methodSimpleName;
            this.partiallySupportedChange = partiallySupportedChange;
            this.patchedMembersDisplayNames = patchedMembersDisplayNames;
        }
    }
    
    internal class AlertEntry {
        internal readonly AlertType alertType;
        internal readonly string title;
        internal readonly DateTime timestamp;
        internal readonly string description;
        [CanBeNull] internal readonly Action actionData;
        internal readonly AlertType iconType;
        internal readonly string shortDescription;
        internal readonly EntryType entryType;
        internal readonly AlertData alertData;
        internal readonly bool hasExitButton;

        internal AlertEntry(AlertType alertType, string title, string description, DateTime timestamp, string shortDescription = null, Action actionData = null, AlertType? iconType = null, EntryType entryType = EntryType.Standalone, AlertData alertData = default(AlertData), bool hasExitButton = true) {
            this.alertType = alertType;
            this.title = title;
            this.description = description;
            this.shortDescription = shortDescription;
            this.actionData = actionData;
            this.iconType = iconType ?? alertType;
            this.timestamp = timestamp;
            this.entryType = entryType;
            this.alertData = alertData;
            this.hasExitButton = hasExitButton;
        }
    }

    internal static class HotReloadTimelineHelper {
        internal const int maxVisibleEntries = 40;
        
        private static List<AlertEntry> eventsTimeline = new List<AlertEntry>();
        internal static List<AlertEntry> EventsTimeline => eventsTimeline;

        static readonly string filePath = Path.Combine(PackageConst.LibraryCachePath, "eventEntries.json");

        public static void InitPersistedEvents() {
            if (!File.Exists(filePath)) {
                return;
            }
            var redDotShown = HotReloadState.ShowingRedDot;
            try {
                var persistedAlertData = JsonConvert.DeserializeObject<PersistedAlertData>(File.ReadAllText(filePath));
                eventsTimeline = new List<AlertEntry>(persistedAlertData.alertDatas.Length);
                for (int i = persistedAlertData.alertDatas.Length - 1; i >= 0; i--) {
                    AlertData alertData = persistedAlertData.alertDatas[i];
                    switch (alertData.alertEntryType) {
                        case AlertEntryType.Error:
                            CreateErrorEventEntry(errorString: alertData.errorString, entryType: alertData.entryType, createdAt: alertData.createdAt);
                            break;
#if UNITY_2020_1_OR_NEWER
                        case AlertEntryType.InlinedMethod:
                            CreateInlinedMethodsEntry(alertData.patchedMembersDisplayNames, alertData.entryType, alertData.createdAt);
                            break;
#endif
                        case AlertEntryType.Failure:
                            if (alertData.entryType == EntryType.Parent) {
                                CreateReloadFinishedWithWarningsEventEntry(createdAt: alertData.createdAt, patchedMembersDisplayNames: alertData.patchedMembersDisplayNames);
                            } else {
                                CreatePatchFailureEventEntry(errorString: alertData.errorString, methodName: alertData.methodName, methodSimpleName: alertData.methodSimpleName, entryType: alertData.entryType, createdAt: alertData.createdAt);
                            }
                            break;
                        case AlertEntryType.PatchApplied:
                            CreateReloadFinishedEventEntry(
                                createdAt: alertData.createdAt,
                                patchedMethodsDisplayNames: alertData.patchedMembersDisplayNames
                            );
                            break;
                        case AlertEntryType.PartiallySupportedChange:
                            if (alertData.entryType == EntryType.Parent) {
                                CreateReloadPartiallyAppliedEventEntry(createdAt: alertData.createdAt, patchedMethodsDisplayNames: alertData.patchedMembersDisplayNames);
                            } else {
                                CreatePartiallyAppliedEventEntry(alertData.partiallySupportedChange, entryType: alertData.entryType, detailed: alertData.detiled, createdAt: alertData.createdAt);
                            }
                            break;
                        case AlertEntryType.UndetectedChange:
                            CreateReloadUndetectedChangeEventEntry(createdAt: alertData.createdAt);
                            break;
                    }
                }
            } catch (Exception e) {
                Log.Warning($"Failed initializing Hot Reload event entries on start: {e}");
            } finally {
                // Ensure red dot is not triggered for existing entries
                HotReloadState.ShowingRedDot = redDotShown;
            }
        }

        internal static void PersistTimeline() {
            var alertDatas = new AlertData[eventsTimeline.Count];
            for (var i = 0; i < eventsTimeline.Count; i++) {
                alertDatas[i] = eventsTimeline[i].alertData;
            }
            var persistedData = new PersistedAlertData(alertDatas);
            try {
                File.WriteAllText(path: filePath, contents: JsonConvert.SerializeObject(persistedData));
            } catch (Exception e) {
                Log.Warning($"Failed persisting Hot Reload event entries: {e}");
            }
        }
        
        internal static void ClearPersistance() {
            try {
                File.Delete(filePath);
            } catch {
                // ignore
            }
            eventsTimeline = new List<AlertEntry>();
        }
        
        internal static readonly Dictionary<AlertType, string> alertIconString = new Dictionary<AlertType, string> {
            { AlertType.Suggestion, "alert_info" },
            { AlertType.UnsupportedChange, "warning" },
            { AlertType.CompileError, "error" },
            { AlertType.PartiallySupportedChange, "infos" },
            { AlertType.AppliedChange, "applied_patch" },
            { AlertType.UndetectedChange, "undetected" },
        };
        
#pragma warning disable CS0612 // obsolete
        public static Dictionary<PartiallySupportedChange, string> partiallySupportedChangeDescriptions = new Dictionary<PartiallySupportedChange, string> {
            {PartiallySupportedChange.LambdaClosure, "A lambda closure was edited (captured variable was added or removed). Changes to it will only be visible to the next created lambda(s)."},
            {PartiallySupportedChange.EditAsyncMethod, "An async method was edited. Changes to it will only be visible the next time this method is called."},
            {PartiallySupportedChange.AddMonobehaviourMethod, "A new method was added. It will not show up in the Inspector until the next full recompilation."},
            {PartiallySupportedChange.EditMonobehaviourField, "A field in a MonoBehaviour was removed or reordered. The inspector will not notice this change until the next full recompilation."},
            {PartiallySupportedChange.EditCoroutine, "An IEnumerator/IEnumerable was edited. When used as a coroutine, changes to it will only be visible the next time the coroutine is created."},
            {PartiallySupportedChange.EditGenericFieldInitializer, "A field initializer inside generic class was edited. Field initializer will not have any effect until the next full recompilation."},
            {PartiallySupportedChange.AddEnumMember, "An enum member was added. ToString and other reflection methods work only after the next full recompilation. Additionally, changes to the enum order may not apply until you patch usages in other places of the code."},
            {PartiallySupportedChange.EditFieldInitializer, "A field initializer was edited. Changes will only apply to new instances of that type, since the initializer for an object only runs when it is created."},
            {PartiallySupportedChange.AddMethodWithAttributes, "A method with attributes was added. Method attributes will not have any effect until the next full recompilation."},
            {PartiallySupportedChange.AddFieldWithAttributes, "A field with attributes was added. Field attributes will not have any effect until the next full recompilation."},
            {PartiallySupportedChange.GenericMethodInGenericClass, "A generic method was edited. Usages in non-generic classes applied, but usages in the generic classes are not supported."},
            {PartiallySupportedChange.NewCustomSerializableField, "A new custom serializable field was added. The inspector will not notice this change until the next full recompilation."},
            {PartiallySupportedChange.MultipleFieldsEditedInTheSameType, "Multiple fields modified in the same type during a single patch. Their values have been reset."},
        };
#pragma warning restore CS0612
        
        internal static List<AlertEntry> Suggestions = new List<AlertEntry>();
        internal static int UnsupportedChangesCount => EventsTimeline.Count(alert => alert.alertType == AlertType.UnsupportedChange && alert.entryType != EntryType.Child);
        internal static int PartiallySupportedChangesCount => EventsTimeline.Count(alert => alert.alertType == AlertType.PartiallySupportedChange && alert.entryType != EntryType.Child);
        internal static int UndetectedChangesCount => EventsTimeline.Count(alert => alert.alertType == AlertType.UndetectedChange && alert.entryType != EntryType.Child);
        internal static int CompileErrorsCount => EventsTimeline.Count(alert => alert.alertType == AlertType.CompileError);
        internal static int AppliedChangesCount => EventsTimeline.Count(alert => alert.alertType == AlertType.AppliedChange);

        static Regex shortDescriptionRegex = new Regex(@"^(\w+)\s(\w+)(?=:)", RegexOptions.Compiled);
        
        internal static int GetRunTabTimelineEventCount() {
            int total = 0;
            if (HotReloadPrefs.RunTabUnsupportedChangesFilter) {
                total += UnsupportedChangesCount;
            }
            if (HotReloadPrefs.RunTabPartiallyAppliedPatchesFilter) {
                total += PartiallySupportedChangesCount;
            }
            if (HotReloadPrefs.RunTabUndetectedPatchesFilter) {
                total += UndetectedChangesCount;
            }
            if (HotReloadPrefs.RunTabCompileErrorFilter) {
                total += CompileErrorsCount;
            }
            if (HotReloadPrefs.RunTabAppliedPatchesFilter) {
                total += AppliedChangesCount;
            }
            return total;
        }
        
        internal static List<AlertEntry> expandedEntries = new List<AlertEntry>();
        
        internal static void RenderCompileButton() {
            if (GUILayout.Button("Recompile", GUILayout.Width(80))) {
                HotReloadRunTab.RecompileWithChecks();
            }
        }
        
        private static float maxScrollPos;
        internal static void RenderErrorEventActions(string description, ErrorData errorData) {
            int maxLen = 2400;
            string text = errorData.stacktrace;
            if (text.Length > maxLen) {
                text = text.Substring(0, maxLen) + "...";
            }

            GUILayout.TextArea(text, HotReloadWindowStyles.StacktraceTextAreaStyle);

            if (errorData.file || !errorData.stacktrace.Contains("error CS")) {
                GUILayout.Space(10f);
            }
            
            using (new EditorGUILayout.HorizontalScope()) {
                if (!errorData.stacktrace.Contains("error CS")) {
                    RenderCompileButton();
                }
            
                // Link
                if (errorData.file) {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(errorData.linkString, HotReloadWindowStyles.LinkStyle)) {
                        AssetDatabase.OpenAsset(errorData.file, Math.Max(errorData.lineNumber, 1));
                    }
                }
            }
        }

        private static Texture2D GetFilterIcon(int count, AlertType alertType) {
            if (count == 0) {
                return GUIHelper.ConvertToGrayscale(alertIconString[alertType]);
            }
            return GUIHelper.GetLocalIcon(alertIconString[alertType]);
        }
        
        internal static void RenderAlertFilters() {
            using (new EditorGUILayout.HorizontalScope()) {
                var text = AppliedChangesCount > 999 ? "999+" : " " + AppliedChangesCount;
                
                HotReloadPrefs.RunTabAppliedPatchesFilter = GUILayout.Toggle(
                    HotReloadPrefs.RunTabAppliedPatchesFilter,
                    new GUIContent(text, GetFilterIcon(AppliedChangesCount, AlertType.AppliedChange)), 
                    HotReloadWindowStyles.EventFiltersStyle);
                
                GUILayout.Space(-1f);
                
                text = UndetectedChangesCount > 999 ? "999+" : " " + UndetectedChangesCount;
                HotReloadPrefs.RunTabUndetectedPatchesFilter = GUILayout.Toggle(
                    HotReloadPrefs.RunTabUndetectedPatchesFilter,
                    new GUIContent(text, GetFilterIcon(UnsupportedChangesCount, AlertType.UndetectedChange)), 
                    HotReloadWindowStyles.EventFiltersStyle);
                
                GUILayout.Space(-1f);
                
                text = PartiallySupportedChangesCount > 999 ? "999+" : " " + PartiallySupportedChangesCount;
                HotReloadPrefs.RunTabPartiallyAppliedPatchesFilter = GUILayout.Toggle(
                    HotReloadPrefs.RunTabPartiallyAppliedPatchesFilter,
                    new GUIContent(text, GetFilterIcon(PartiallySupportedChangesCount, AlertType.PartiallySupportedChange)), 
                    HotReloadWindowStyles.EventFiltersStyle);
                
                GUILayout.Space(-1f);
                
                text = UnsupportedChangesCount > 999 ? "999+" : " " + UnsupportedChangesCount;
                HotReloadPrefs.RunTabUnsupportedChangesFilter = GUILayout.Toggle(
                    HotReloadPrefs.RunTabUnsupportedChangesFilter, 
                    new GUIContent(text, GetFilterIcon(UnsupportedChangesCount, AlertType.UnsupportedChange)), 
                    HotReloadWindowStyles.EventFiltersStyle);
                
                GUILayout.Space(-1f);
                
                text = CompileErrorsCount > 999 ? "999+" : " " + CompileErrorsCount;
                HotReloadPrefs.RunTabCompileErrorFilter = GUILayout.Toggle(
                    HotReloadPrefs.RunTabCompileErrorFilter,
                    new GUIContent(text, GetFilterIcon(CompileErrorsCount, AlertType.CompileError)), 
                    HotReloadWindowStyles.EventFiltersStyle);
            }
        }

        internal static void CreateErrorEventEntry(string errorString, EntryType entryType = EntryType.Standalone, DateTime? createdAt = null) {
            var timestamp = createdAt ?? DateTime.Now;
            var alertType = errorString.Contains("error CS")
                ? AlertType.CompileError
                : AlertType.UnsupportedChange;
            var title = errorString.Contains("error CS")
                ? "Compile error"
                : "Unsupported change";
            ErrorData errorData = ErrorData.GetErrorData(errorString);
            var description = errorData.error;
            string shortDescription = null;
            if (alertType != AlertType.CompileError) {
                shortDescription = shortDescriptionRegex.Match(description).Value;
            }
            Action actionData = () => RenderErrorEventActions(description, errorData);
            InsertEntry(new AlertEntry(
                timestamp: timestamp,
                alertType: alertType, 
                title: title, 
                description: description, 
                shortDescription: shortDescription, 
                actionData: actionData,
                entryType: entryType,
                alertData: new AlertData(AlertEntryType.Error, createdAt: timestamp, errorString: errorString, entryType: entryType)
            ));
        }
        
#if UNITY_2020_1_OR_NEWER
        internal static void CreateInlinedMethodsEntry(string[] patchedMethodsDisplayNames, EntryType entryType = EntryType.Standalone, DateTime? createdAt = null) {
            var truncated = false;
            if (patchedMethodsDisplayNames?.Length > 25) {
                patchedMethodsDisplayNames = TruncateList(patchedMethodsDisplayNames, 25);
                truncated = true;
            }
            var patchesList = patchedMethodsDisplayNames?.Length > 0 ? string.Join("\n• ", patchedMethodsDisplayNames) : "";
            var timestamp = createdAt ?? DateTime.Now;
            var entry = new AlertEntry(
                timestamp: timestamp,
                alertType : AlertType.UnsupportedChange, 
                title: "Failed applying patch to method", 
                description: $"Some methods got inlined by the Unity compiler and cannot be patched by Hot Reload. Switch to Debug mode to avoid this problem.\n\n• {(truncated ? patchesList + "\n..." : patchesList)}",
                entryType: EntryType.Parent,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        RenderCompileButton();
                        var suggestion = HotReloadSuggestionsHelper.suggestionMap[HotReloadSuggestionKind.SwitchToDebugModeForInlinedMethods];
                        if (suggestion?.actionData != null) {
                            suggestion.actionData();
                        }
                    }
                },
                alertData: new AlertData(AlertEntryType.InlinedMethod, createdAt: timestamp, patchedMembersDisplayNames: patchedMethodsDisplayNames, entryType: EntryType.Parent)
            );
            InsertEntry(entry);
            if (patchedMethodsDisplayNames?.Length > 0) {
                expandedEntries.Add(entry);
            }
        }
#endif
        
        internal static void CreatePatchFailureEventEntry(string errorString, string methodName, string methodSimpleName = null, EntryType entryType = EntryType.Standalone, DateTime? createdAt = null) {
            var timestamp = createdAt ?? DateTime.Now;
            ErrorData errorData = ErrorData.GetErrorData(errorString);
            var title = $"Failed applying patch to method";
            Action actionData = () => RenderErrorEventActions(errorData.error, errorData);
            InsertEntry(new AlertEntry(
                timestamp: timestamp,
                alertType : AlertType.UnsupportedChange, 
                title: title, 
                description: $"{title}: {methodName}, tap here to see more.",
                shortDescription: methodSimpleName, 
                actionData: actionData,
                entryType: entryType,
                alertData: new AlertData(AlertEntryType.Failure, createdAt: timestamp, errorString: errorString, methodName: methodName, methodSimpleName: methodSimpleName, entryType: entryType)
            ));
        }

        public static T[] TruncateList<T>(T[] originalList, int len) {
            if (originalList.Length <= len) {
                return originalList;
            }
            // Create a new list with a maximum of 25 items
            T[] truncatedList = new T[len];

            for (int i = 0; i < originalList.Length && i < len; i++) {
                truncatedList[i] = originalList[i];
            }

            return truncatedList;
        }
        
        internal static void CreateReloadFinishedEventEntry(DateTime? createdAt = null, string[] patchedMethodsDisplayNames = null) {
            var truncated = false;
            if (patchedMethodsDisplayNames?.Length > 25) {
                patchedMethodsDisplayNames = TruncateList(patchedMethodsDisplayNames, 25);
                truncated = true;
            }
            var patchesList = patchedMethodsDisplayNames?.Length > 0 ? string.Join("\n• ", patchedMethodsDisplayNames) : "";
            var timestamp = createdAt ?? DateTime.Now;
            var entry = new AlertEntry(
                timestamp: timestamp,
                alertType: AlertType.AppliedChange,
                title: EditorIndicationState.IndicationText[EditorIndicationState.IndicationStatus.Reloaded],
                description: patchedMethodsDisplayNames?.Length > 0 
                    ? $"• {(truncated ? patchesList + "\n..." : patchesList)}" 
                    : "No issues found",
                entryType: patchedMethodsDisplayNames?.Length > 0 ? EntryType.Parent : EntryType.Standalone,
                alertData: new AlertData(
                    AlertEntryType.PatchApplied, 
                    createdAt: timestamp, 
                    entryType: EntryType.Standalone,
                    patchedMembersDisplayNames: patchedMethodsDisplayNames)
            );
            
            InsertEntry(entry);
            if (patchedMethodsDisplayNames?.Length > 0) {
                expandedEntries.Add(entry);
            }
        }
        
        internal static void CreateReloadFinishedWithWarningsEventEntry(DateTime? createdAt = null, string[] patchedMembersDisplayNames = null) {
            var truncated = false;
            if (patchedMembersDisplayNames?.Length > 25) {
                patchedMembersDisplayNames = TruncateList(patchedMembersDisplayNames, 25);
                truncated = true;
            }
            var patchesList = patchedMembersDisplayNames?.Length > 0 ? string.Join("\n• ", patchedMembersDisplayNames) : "";
            var timestamp = createdAt ?? DateTime.Now;
            var entry = new AlertEntry(
                timestamp: timestamp,
                alertType: AlertType.UnsupportedChange,
                title: EditorIndicationState.IndicationText[EditorIndicationState.IndicationStatus.Unsupported],
                description: patchedMembersDisplayNames?.Length > 0 ? $"• {(truncated ? patchesList + "\n...\n\nSee unsupported changes below" : patchesList + "\n\nSee unsupported changes below")}" : "See detailed entries below",
                entryType: EntryType.Parent,
                alertData: new AlertData(AlertEntryType.Failure, createdAt: timestamp, entryType: EntryType.Parent, patchedMembersDisplayNames: patchedMembersDisplayNames)
            );
            InsertEntry(entry);
            if (patchedMembersDisplayNames?.Length > 0) {
                expandedEntries.Add(entry);
            }
        }
        
        internal static void CreateReloadPartiallyAppliedEventEntry(DateTime? createdAt = null, string[] patchedMethodsDisplayNames = null) {
            var truncated = false;
            if (patchedMethodsDisplayNames?.Length > 25) {
                patchedMethodsDisplayNames = TruncateList(patchedMethodsDisplayNames, 25);
                truncated = true;
            }
            var patchesList = patchedMethodsDisplayNames?.Length > 0 ? string.Join("\n• ", patchedMethodsDisplayNames) : "";
            var timestamp = createdAt ?? DateTime.Now;
            var entry = new AlertEntry(
                timestamp: timestamp,
                alertType: AlertType.PartiallySupportedChange,
                title: EditorIndicationState.IndicationText[EditorIndicationState.IndicationStatus.PartiallySupported],
                description: patchedMethodsDisplayNames?.Length > 0 ? $"• {(truncated ? patchesList + "\n...\n\nSee partially applied changes below" : patchesList + "\n\nSee partially applied changes below")}"  : "See detailed entries below",
                entryType: EntryType.Parent,
                alertData: new AlertData(AlertEntryType.PartiallySupportedChange, createdAt: timestamp, entryType: EntryType.Parent, patchedMembersDisplayNames: patchedMethodsDisplayNames)
            );
            InsertEntry(entry);
            if (patchedMethodsDisplayNames?.Length > 0) {
                expandedEntries.Add(entry);
            }
        }
        
        internal static void CreateReloadUndetectedChangeEventEntry(DateTime? createdAt = null) {
            var timestamp = createdAt ?? DateTime.Now;
            InsertEntry(new AlertEntry(
                timestamp: timestamp,
                alertType : AlertType.UndetectedChange, 
                title: EditorIndicationState.IndicationText[EditorIndicationState.IndicationStatus.Undetected],
                description: "Code semantics didn't change (e.g. whitespace) or the change requires manual recompile.\n\n" +
                           "Recompile to force-apply changes.",
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        RenderCompileButton();
                        GUILayout.FlexibleSpace();
                        OpenURLButton.Render("Docs", Constants.UndetectedChangesURL);
                        GUILayout.Space(10f);
                    }
                },
                entryType: EntryType.Foldout,
                alertData: new AlertData(AlertEntryType.UndetectedChange, createdAt: timestamp, entryType: EntryType.Parent)
            ));
        }
        
        internal static void CreatePartiallyAppliedEventEntry(PartiallySupportedChange partiallySupportedChange, EntryType entryType = EntryType.Standalone, bool detailed = true, DateTime? createdAt = null) {
            var timestamp = createdAt ?? DateTime.Now;
            string description;
            if (!partiallySupportedChangeDescriptions.TryGetValue(partiallySupportedChange, out description)) {
                return;
            }
            InsertEntry(new AlertEntry(
                timestamp: timestamp,
                alertType : AlertType.PartiallySupportedChange, 
                title : detailed ? "Change partially applied" : ToString(partiallySupportedChange),
                description : description,
                shortDescription: detailed ? ToString(partiallySupportedChange) : null,
                actionData: () => {
                    GUILayout.Space(10f);
                    using (new EditorGUILayout.HorizontalScope()) {
                        RenderCompileButton();
                        GUILayout.FlexibleSpace();
                        if (GetPartiallySupportedChangePref(partiallySupportedChange)) {
                            if (GUILayout.Button("Ignore this event type ", HotReloadWindowStyles.LinkStyle)) {
                                HidePartiallySupportedChange(partiallySupportedChange);
                                HotReloadRunTab.RepaintInstant();
                            }
                        }
                    }
                },
                entryType: entryType,
                alertData: new AlertData(AlertEntryType.PartiallySupportedChange, createdAt: timestamp, partiallySupportedChange: partiallySupportedChange, entryType: entryType, detiled: detailed)
            ));
        }
        
        internal static void InsertEntry(AlertEntry entry) {
            eventsTimeline.Insert(0, entry);
            if (entry.alertType != AlertType.AppliedChange) {
                HotReloadState.ShowingRedDot = true;
            }
        }
        
        internal static void ClearEntries() {
            eventsTimeline.Clear();
        }
        
        internal static bool GetPartiallySupportedChangePref(PartiallySupportedChange key) {
            return EditorPrefs.GetBool($"HotReloadWindow.ShowPartiallySupportedChangeType.{key}", true);
        }
        
        internal static void HidePartiallySupportedChange(PartiallySupportedChange key) {
            EditorPrefs.SetBool($"HotReloadWindow.ShowPartiallySupportedChangeType.{key}", false);
            // loop over scroll entries to remove hidden entries
            for (var i = EventsTimeline.Count - 1; i >= 0; i--) {
                var eventEntry = EventsTimeline[i];
                if (eventEntry.alertData.partiallySupportedChange == key) {
                    EventsTimeline.Remove(eventEntry);
                }
            }
        }

        // performance optimization (Enum.ToString uses reflection)
        internal static string ToString(this PartiallySupportedChange change) {
#pragma warning disable CS0612 // obsolete
            switch (change) {
                case PartiallySupportedChange.LambdaClosure:
                    return nameof(PartiallySupportedChange.LambdaClosure);
                case PartiallySupportedChange.EditAsyncMethod:
                   return nameof(PartiallySupportedChange.EditAsyncMethod);
                case PartiallySupportedChange.AddMonobehaviourMethod:
                   return nameof(PartiallySupportedChange.AddMonobehaviourMethod);
                case PartiallySupportedChange.EditMonobehaviourField:
                    return nameof(PartiallySupportedChange.EditMonobehaviourField);
                case PartiallySupportedChange.EditCoroutine:
                   return nameof(PartiallySupportedChange.EditCoroutine);
                case PartiallySupportedChange.EditGenericFieldInitializer:
                   return nameof(PartiallySupportedChange.EditGenericFieldInitializer);
                case PartiallySupportedChange.AddEnumMember:
                   return nameof(PartiallySupportedChange.AddEnumMember);
                case PartiallySupportedChange.EditFieldInitializer:
                   return nameof(PartiallySupportedChange.EditFieldInitializer);
                case PartiallySupportedChange.AddMethodWithAttributes:
                   return nameof(PartiallySupportedChange.AddMethodWithAttributes);
                case PartiallySupportedChange.GenericMethodInGenericClass:
                   return nameof(PartiallySupportedChange.GenericMethodInGenericClass);
                case PartiallySupportedChange.AddFieldWithAttributes:
                   return nameof(PartiallySupportedChange.AddFieldWithAttributes);
                case PartiallySupportedChange.NewCustomSerializableField:
                   return nameof(PartiallySupportedChange.NewCustomSerializableField);
                case PartiallySupportedChange.MultipleFieldsEditedInTheSameType:
                   return nameof(PartiallySupportedChange.MultipleFieldsEditedInTheSameType);
#pragma warning restore CS0612
                default:
                    throw new ArgumentOutOfRangeException(nameof(change), change, null);
            }
        }
    }
}
