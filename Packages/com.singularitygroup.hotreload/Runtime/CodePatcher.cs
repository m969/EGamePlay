#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using JetBrains.Annotations;
using SingularityGroup.HotReload.Burst;
using SingularityGroup.HotReload.HarmonyLib;
using SingularityGroup.HotReload.JsonConverters;
using SingularityGroup.HotReload.MonoMod.Utils;
using SingularityGroup.HotReload.Newtonsoft.Json;
using SingularityGroup.HotReload.RuntimeDependencies;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.Editor")]

namespace SingularityGroup.HotReload {
    class RegisterPatchesResult {
        // note: doesn't include removals and method definition changes (e.g. renames)
        public readonly List<MethodPatch> patchedMethods = new List<MethodPatch>();
        public List<SField> addedFields = new List<SField>();
        public readonly List<SMethod> patchedSMethods = new List<SMethod>();
        public bool inspectorModified;
        public readonly List<Tuple<SMethod, string>> patchFailures = new List<Tuple<SMethod, string>>();
        public readonly List<string> patchExceptions = new List<string>();
    }

    class FieldHandler {
        public readonly Action<Type, FieldInfo> storeField;
        public readonly Action<Type, FieldInfo, FieldInfo> registerInspectorFieldAttributes;
        public readonly Func<Type, string, bool> hideField;

        public FieldHandler(Action<Type, FieldInfo> storeField, Func<Type, string, bool> hideField, Action<Type, FieldInfo, FieldInfo> registerInspectorFieldAttributes) {
            this.storeField = storeField;
            this.hideField = hideField;
            this.registerInspectorFieldAttributes = registerInspectorFieldAttributes;
        }
    }
    
    class CodePatcher {
        public static readonly CodePatcher I = new CodePatcher();
        /// <summary>Tag for use in Debug.Log.</summary>
        public const string TAG = "HotReload";
        
        internal int PatchesApplied { get; private set; }
        string PersistencePath {get;}
        
        List<MethodPatchResponse> pendingPatches;
        readonly List<MethodPatchResponse> patchHistory;
        readonly HashSet<string> seenResponses = new HashSet<string>();
        string[] assemblySearchPaths;
        SymbolResolver symbolResolver;
        readonly string tmpDir;
        public FieldHandler fieldHandler;
        public bool debuggerCompatibilityEnabled;
        
        CodePatcher() {
            pendingPatches = new List<MethodPatchResponse>();
            patchHistory = new List<MethodPatchResponse>(); 
            if(UnityHelper.IsEditor) {
                tmpDir = PackageConst.LibraryCachePath;
            } else {
                tmpDir = UnityHelper.TemporaryCachePath;
            }
            if(!UnityHelper.IsEditor) {
                PersistencePath = Path.Combine(UnityHelper.PersistentDataPath, "HotReload", "patches.json");
                try {
                    LoadPatches(PersistencePath);
                } catch(Exception ex) {
                    Log.Error("Encountered exception when loading patches from disk:\n{0}", ex);
                }
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeUnityEvents() {
            UnityEventHelper.Initialize();
        }

        
        void LoadPatches(string filePath) {
            PlayerLog("Loading patches from file {0}", filePath);
            var file = new FileInfo(filePath);
            if(file.Exists) {
                var bytes = File.ReadAllText(filePath);
                var patches = JsonConvert.DeserializeObject<List<MethodPatchResponse>>(bytes);
                PlayerLog("Loaded {0} patches from disk", patches.Count.ToString());
                foreach (var patch in patches) {
                    RegisterPatches(patch, persist: false);
                }
            }  
        }

        
        internal IReadOnlyList<MethodPatchResponse> PendingPatches => pendingPatches;
        internal SymbolResolver SymbolResolver => symbolResolver;
        
        
        internal string[] GetAssemblySearchPaths() {
            EnsureSymbolResolver();
            return assemblySearchPaths;
        }
       
        internal RegisterPatchesResult RegisterPatches(MethodPatchResponse patches, bool persist) {
            PlayerLog("Register patches.\nWarnings: {0} \nMethods:\n{1}", string.Join("\n", patches.failures), string.Join("\n", patches.patches.SelectMany(p => p.modifiedMethods).Select(m => m.displayName)));
            pendingPatches.Add(patches);
            return ApplyPatches(persist);
        }
        
        RegisterPatchesResult ApplyPatches(bool persist) {
            PlayerLog("ApplyPatches. {0} patches pending.", pendingPatches.Count);
            EnsureSymbolResolver();

            var result = new RegisterPatchesResult();
            
            try {
                int count = 0;
                foreach(var response in pendingPatches) {
                    if (seenResponses.Contains(response.id)) {
                        continue;
                    }
                    foreach (var patch in response.patches) {
                        var asm = Assembly.Load(patch.patchAssembly, patch.patchPdb);
                        SymbolResolver.AddAssembly(asm);
                    }
                    HandleRemovedUnityMethods(response.removedMethod);
#if UNITY_EDITOR
                    HandleAlteredFields(response.id, result, response.alteredFields);
#endif
                    // needs to come before RegisterNewFieldInitializers
                    RegisterNewFieldDefinitions(response);
                    // Note: order is important here. Reshaped fields require new field initializers to be added
                    // because the old initializers must override new initilaizers for existing holders.
                    // so that the initializer is not invoked twice
                    RegisterNewFieldInitializers(response);
                    HandleReshapedFields(response);
                    RemoveOldFieldInitializers(response);
#if UNITY_EDITOR
                    RegisterInspectorFieldAttributes(result, response);
#endif

                    HandleMethodPatchResponse(response, result);
                    patchHistory.Add(response);

                    seenResponses.Add(response.id);
                    count += response.patches.Length;
                }
                if (count > 0) {
                    Dispatch.OnHotReload(result.patchedMethods).Forget();
                }
            } catch(Exception ex) {
                Log.Warning("Exception occured when handling method patch. Exception:\n{0}", ex);
            } finally {
                pendingPatches.Clear();
            }
            
            if(PersistencePath != null && persist) {
                SaveAppliedPatches(PersistencePath).Forget();
            }

            PatchesApplied++;
            return result;
        }
        
        internal void ClearPatchedMethods() {
            PatchesApplied = 0;
        }

        static bool didLog;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void WarnOnSceneLoad() {
            SceneManager.sceneLoaded += (_, __) => {
                if (didLog || !UnityEventHelper.UnityMethodsAdded()) {
                    return;
                }
                Log.Warning("A new Scene was loaded while new unity event methods were added at runtime. MonoBehaviours in the Scene will not trigger these new events.");
                didLog = true;
            };
        }

        void HandleMethodPatchResponse(MethodPatchResponse response, RegisterPatchesResult result) {
            EnsureSymbolResolver();

            foreach(var patch in response.patches) {
                try {
                    foreach(var sMethod in patch.newMethods) {
                        var newMethod = SymbolResolver.Resolve(sMethod);
                        try {
                            UnityEventHelper.EnsureUnityEventMethod(newMethod);
                        } catch(Exception ex) {
                            Log.Warning("Encountered exception in EnsureUnityEventMethod: {0} {1}", ex.GetType().Name, ex.Message);
                        }
                        MethodUtils.DisableVisibilityChecks(newMethod);
                        if (!patch.patchMethods.Any(m => m.metadataToken == sMethod.metadataToken)) {
                            result.patchedMethods.Add(new MethodPatch(null, null, newMethod));
                            result.patchedSMethods.Add(sMethod);
                            previousPatchMethods[newMethod] = newMethod;
                            newMethods.Add(newMethod);
                        }
                    }
                    
                    for (int i = 0; i < patch.modifiedMethods.Length; i++) {
                        var sOriginalMethod = patch.modifiedMethods[i];
                        var sPatchMethod = patch.patchMethods[i];
                        var err = PatchMethod(response.id, sOriginalMethod: sOriginalMethod, sPatchMethod: sPatchMethod, containsBurstJobs: patch.unityJobs.Length > 0, patchesResult: result);
                        if (!string.IsNullOrEmpty(err)) {
                            result.patchFailures.Add(Tuple.Create(sOriginalMethod, err));
                        }
                    }
                    foreach (var job in patch.unityJobs) {
                        var type = SymbolResolver.Resolve(new SType(patch.assemblyName, job.jobKind.ToString(), job.metadataToken));
                        JobHotReloadUtility.HotReloadBurstCompiledJobs(job, type);
                    }
#if UNITY_EDITOR
                    HandleNewFields(patch.patchId, result, patch.newFields);
#endif
                } catch (Exception ex) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.Exception), new EditorExtraData {
                        { StatKey.PatchId, patch.patchId },
                        { StatKey.Detailed_Exception, ex.ToString() },
                    }).Forget();
                    result.patchExceptions.Add($"Edit requires full recompile to apply: Encountered exception when applying a patch.\nCommon causes: editing code that failed to patch previously, an unsupported change, or a real bug in Hot Reload.\nIf you think this is a bug, please report the issue on Discord and include a code-snippet before/after.\nException: {ex}");
                }
            }
        }
        
        void HandleRemovedUnityMethods(SMethod[] removedMethods) {
            if (removedMethods == null) {
                return;
            }
            foreach(var sMethod in removedMethods) {
                try {
                    var oldMethod = SymbolResolver.Resolve(sMethod);
                    UnityEventHelper.RemoveUnityEventMethod(oldMethod);
                } catch (SymbolResolvingFailedException) {
                    // ignore, not a unity event method if can't resolve
                } catch(Exception ex) {
                    Log.Warning("Encountered exception in RemoveUnityEventMethod: {0} {1}", ex.GetType().Name, ex.Message);
                }
            }
        }
        
        // Important: must come before applying any patches
        void RegisterNewFieldInitializers(MethodPatchResponse resp) {
            for (var i = 0; i < resp.addedFieldInitializerFields.Length; i++) {
                var sField = resp.addedFieldInitializerFields[i];
                var sMethod = resp.addedFieldInitializerInitializers[i];
                try {
                    var declaringType = SymbolResolver.Resolve(sField.declaringType);
                    var method = SymbolResolver.Resolve(sMethod);
                    if (!(method is MethodInfo initializer)) {
                        Log.Warning($"Failed registering initializer for field {sField.fieldName} in {sField.declaringType.typeName}. Field value might not be initialized correctly. Invalid method.");
                        continue;
                    }
                    // We infer if the field is static by the number of parameters the method has
                    // because sField is old field
                    var isStatic = initializer.GetParameters().Length == 0;
                    MethodUtils.DisableVisibilityChecks(initializer);
                    // Initializer return type is used in place of fieldType because latter might be point to old field if the type changed
                    FieldInitializerRegister.RegisterInitializer(declaringType, sField.fieldName, initializer.ReturnType, initializer, isStatic);
                    
                } catch (Exception e) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.RegisterFieldInitializer), new EditorExtraData {
                        { StatKey.PatchId, resp.id },
                        { StatKey.Detailed_Exception, e.ToString() },
                    }).Forget();
                    Log.Warning($"Failed registering initializer for field {sField.fieldName} in {sField.declaringType.typeName}. Field value might not be initialized correctly. Exception: {e.Message}");
                }
            }
        }
        
        void RegisterNewFieldDefinitions(MethodPatchResponse resp) {
            foreach (var sField in resp.newFieldDefinitions) {
                try {
                    var declaringType = SymbolResolver.Resolve(sField.declaringType);
                    var fieldType = SymbolResolver.Resolve(sField).FieldType;
                    FieldResolver.RegisterFieldType(declaringType, sField.fieldName, fieldType);
                } catch (Exception e) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.RegisterFieldDefinition), new EditorExtraData {
                        { StatKey.PatchId, resp.id },
                        { StatKey.Detailed_Exception, e.ToString() },
                    }).Forget();
                    Log.Warning($"Failed registering new field definitions for field {sField.fieldName} in {sField.declaringType.typeName}. Exception: {e.Message}");
                }
            }
        }
            
        // Important: must come before applying any patches
        // Note: server might decide not to report removed field initializer at all if it can handle it
        void RemoveOldFieldInitializers(MethodPatchResponse resp) {
            foreach (var sField in resp.removedFieldInitializers) {
                try {
                    var declaringType = SymbolResolver.Resolve(sField.declaringType);
                    var fieldType = SymbolResolver.Resolve(sField.declaringType);
                    FieldInitializerRegister.UnregisterInitializer(declaringType, sField.fieldName, fieldType, sField.isStatic);
                } catch (Exception e) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.UnregisterFieldInitializer), new EditorExtraData {
                        { StatKey.PatchId, resp.id },
                        { StatKey.Detailed_Exception, e.ToString() },
                    }).Forget();
                    Log.Warning($"Failed removing initializer for field {sField.fieldName} in {sField.declaringType.typeName}. Field value might not be initialized correctly. Exception: {e.Message}");
                }
            }
        }
        
        // Important: must come before applying any patches
        // Should also come after RegisterNewFieldInitializers so that new initializers are not invoked for existing objects
        internal void HandleReshapedFields(MethodPatchResponse resp) {
            foreach(var patch in resp.patches) {
                var removedReshapedFields = patch.deletedFields;
                var renamedReshapedFieldsFrom = patch.renamedFieldsFrom;
                var renamedReshapedFieldsTo = patch.renamedFieldsTo;
                
                foreach (var f in removedReshapedFields) {
                    try {
                        var declaringType = SymbolResolver.Resolve(f.declaringType);
                        var fieldType = SymbolResolver.Resolve(f).FieldType;
                        FieldResolver.ClearHolders(declaringType, f.isStatic, f.fieldName, fieldType);
                    } catch (Exception e) {
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.ClearHolders), new EditorExtraData {
                            { StatKey.PatchId, resp.id },
                            { StatKey.Detailed_Exception, e.ToString() },
                        }).Forget();
                        Log.Warning($"Failed removing field value from {f.fieldName} in {f.declaringType.typeName}. Field value in code might not be up to date. Exception: {e.Message}");
                    }
                }
                for (var i = 0; i < renamedReshapedFieldsFrom.Length; i++) {
                    var fromField = renamedReshapedFieldsFrom[i];
                    var toField = renamedReshapedFieldsTo[i];
                    try {
                        var declaringType = SymbolResolver.Resolve(fromField.declaringType);
                        var fieldType = SymbolResolver.Resolve(fromField).FieldType;
                        var toFieldType = SymbolResolver.Resolve(toField).FieldType;
                        if (!AreSTypesCompatible(fromField.declaringType, toField.declaringType)
                            || fieldType != toFieldType
                            || fromField.isStatic != toField.isStatic
                        ) {
                            FieldResolver.ClearHolders(declaringType, fromField.isStatic, fromField.fieldName, fieldType);
                            continue;
                        }
                        FieldResolver.MoveHolders(declaringType, fromField.fieldName, toField.fieldName, fieldType, fromField.isStatic);
                    } catch (Exception e) {
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.MoveHolders), new EditorExtraData {
                            { StatKey.PatchId, resp.id },
                            { StatKey.Detailed_Exception, e.ToString() },
                        }).Forget();
                        Log.Warning($"Failed moving field value from {fromField} to {toField} in {toField.declaringType.typeName}. Field value in code might not be up to date. Exception: {e.Message}");
                    }
                }
            }
        }

        internal bool AreSTypesCompatible(SType one, SType two) {
            if (one.isGenericParameter != two.isGenericParameter) {
                return false;
            }
            if (one.metadataToken != two.metadataToken) {
                return false;
            }
            if (one.assemblyName != two.assemblyName) {
                return false;
            }
            if (one.genericParameterPosition != two.genericParameterPosition) {
                return false;
            }
            if (one.typeName != two.typeName) {
                return false;
            }
            return true;
        }

#if UNITY_EDITOR
        internal void RegisterInspectorFieldAttributes(RegisterPatchesResult result, MethodPatchResponse resp) {
            foreach (var patch in resp.patches) {
                var propertyAttributesFieldOriginal = patch.propertyAttributesFieldOriginal ?? Array.Empty<SField>();
                var propertyAttributesFieldUpdated = patch.propertyAttributesFieldUpdated ?? Array.Empty<SField>();
                for (var i = 0; i < propertyAttributesFieldOriginal.Length; i++) {
                    var original = propertyAttributesFieldOriginal[i];
                    var updated = propertyAttributesFieldUpdated[i];
                    try {
                        var declaringType = SymbolResolver.Resolve(original.declaringType);
                        var originalField = SymbolResolver.Resolve(original);
                        var updatedField = SymbolResolver.Resolve(updated);
                        fieldHandler?.registerInspectorFieldAttributes?.Invoke(declaringType, originalField, updatedField);
                        result.inspectorModified = true;
                    } catch (Exception e) {
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.MoveHolders), new EditorExtraData {
                            { StatKey.PatchId, resp.id },
                            { StatKey.Detailed_Exception, e.ToString() },
                        }).Forget();
                        Log.Warning($"Failed updating field attributes of {original.fieldName} in {original.declaringType.typeName}. Updates might not reflect in the inspector. Exception: {e.Message}");
                    }
                }
            }
        }
        
        internal void HandleNewFields(string patchId, RegisterPatchesResult result, SField[] sFields) {
            foreach (var sField in sFields) {
                if (!sField.serializable) {
                    continue;
                }
                try {
                    var declaringType = SymbolResolver.Resolve(sField.declaringType);
                    var field = SymbolResolver.Resolve(sField);
                    fieldHandler?.storeField?.Invoke(declaringType, field);
                    result.inspectorModified = true;
                } catch (Exception e) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.AddInspectorField), new EditorExtraData {
                        { StatKey.PatchId, patchId },
                        { StatKey.Detailed_Exception, e.ToString() },
                    }).Forget();
                    Log.Warning($"Failed adding field {sField.fieldName}:{sField.declaringType.typeName} to the inspector. Field will not be displayed. Exception: {e.Message}");
                }
            }
            result.addedFields.AddRange(sFields);
        }
        
        // IMPORTANT: must come before HandleNewFields. Might contain new fields which we don't want to hide
        internal void HandleAlteredFields(string patchId, RegisterPatchesResult result, SField[] alteredFields) {
            if (alteredFields == null) {
                return;
            }
            bool alteredFieldHidden = false;
            foreach(var sField in alteredFields) {
                try {
                    var declaringType = SymbolResolver.Resolve(sField.declaringType);
                    if (fieldHandler?.hideField?.Invoke(declaringType, sField.fieldName) == true) {
                        alteredFieldHidden = true;
                    }
                } catch(Exception e) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.HideInspectorField), new EditorExtraData {
                        { StatKey.PatchId, patchId },
                        { StatKey.Detailed_Exception, e.ToString() },
                    }).Forget();
                    Log.Warning($"Failed hiding field {sField.fieldName}:{sField.declaringType.typeName} from the inspector. Exception: {e.Message}");
                }
            }
            if (alteredFieldHidden) {
                result.inspectorModified = true;
            }
        }
#endif

        Dictionary<MethodBase, MethodBase> previousPatchMethods = new Dictionary<MethodBase, MethodBase>();
        public IEnumerable<MethodBase> OriginalPatchMethods => previousPatchMethods.Keys;
        List<MethodBase> newMethods = new List<MethodBase>();

        string PatchMethod(string patchId, SMethod sOriginalMethod, SMethod sPatchMethod, bool containsBurstJobs, RegisterPatchesResult patchesResult) {
            try {
                var patchMethod = SymbolResolver.Resolve(sPatchMethod);
                var start = DateTime.UtcNow;
                var state = TryResolveMethod(sOriginalMethod, patchMethod);
                if (Debugger.IsAttached && !debuggerCompatibilityEnabled) {
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.DebuggerAttached), new EditorExtraData {
                        { StatKey.PatchId, patchId },
                    }).Forget();
                    return "Patching methods is not allowed while the Debugger is attached. You can change this behavior in settings if Hot Reload is compatible with the debugger you're running.";
                }

                if (DateTime.UtcNow - start > TimeSpan.FromMilliseconds(500)) {
                    Log.Info("Hot Reload apply took {0}", (DateTime.UtcNow - start).TotalMilliseconds);
                }

                if(state.match == null) {
                    var error = "Edit requires full recompile to apply: Method mismatch: {0}, patch: {1}. \nCommon causes: editing code that failed to patch previously, an unsupported change, or a real bug in Hot Reload.\nIf you think this is a bug, please report the issue on Discord and include a code-snippet before/after.";
                    RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.MethodMismatch), new EditorExtraData {
                        { StatKey.PatchId, patchId },
                    }).Forget();
                    return string.Format(error, sOriginalMethod.simpleName, patchMethod.Name);
                }

                PlayerLog("Detour method {0:X8} {1}, offset: {2}", sOriginalMethod.metadataToken, patchMethod.Name, state.offset);
                DetourResult result;
                DetourApi.DetourMethod(state.match, patchMethod, out result);
                if (result.success) {
                    // previous method is either original method or the last patch method
                    MethodBase previousMethod;
                    if (!previousPatchMethods.TryGetValue(state.match, out previousMethod)) {
                        previousMethod = state.match;
                    }
                    MethodBase originalMethod = state.match;
                    if (newMethods.Contains(state.match)) {
                        // for function added at runtime the original method should be null
                        originalMethod = null;
                    }
                    patchesResult.patchedMethods.Add(new MethodPatch(originalMethod, previousMethod, patchMethod));
                    patchesResult.patchedSMethods.Add(sOriginalMethod);
                    previousPatchMethods[state.match] = patchMethod;
                    try {
                        Dispatch.OnHotReloadLocal(state.match, patchMethod);
                    } catch {
                        // best effort
                    }
                    return null;
                } else {
                    if(result.exception is InvalidProgramException && containsBurstJobs) {
                        //ignore. The method is likely burst compiled and can't be patched
                        return null;
                    } else {
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.Failure), new EditorExtraData {
                            { StatKey.PatchId, patchId },
                            { StatKey.Detailed_Exception, result.exception.ToString() },
                        }).Forget();
                        return HandleMethodPatchFailure(sOriginalMethod, result.exception);
                    }
                }
            } catch(Exception ex) {
                RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Patching, StatEventType.Exception), new EditorExtraData {
                    { StatKey.PatchId, patchId },
                    { StatKey.Detailed_Exception, ex.ToString() },
                }).Forget();
                return HandleMethodPatchFailure(sOriginalMethod, ex);
            }
        }
        
        struct ResolveMethodState {
            public readonly SMethod originalMethod;
            public readonly int offset;
            public readonly bool tryLowerTokens;
            public readonly bool tryHigherTokens;
            public readonly MethodBase match;
            public ResolveMethodState(SMethod originalMethod, int offset, bool tryLowerTokens, bool tryHigherTokens, MethodBase match) {
                this.originalMethod = originalMethod;
                this.offset = offset;
                this.tryLowerTokens = tryLowerTokens;
                this.tryHigherTokens = tryHigherTokens;
                this.match = match;
            }

            public ResolveMethodState With(bool? tryLowerTokens = null, bool? tryHigherTokens = null, MethodBase match = null, int? offset = null) {
                return new ResolveMethodState(
                    originalMethod, 
                    offset ?? this.offset, 
                    tryLowerTokens ?? this.tryLowerTokens,
                    tryHigherTokens ?? this.tryHigherTokens,
                    match ?? this.match);
            }
        }
        
        struct ResolveMethodResult {
            public readonly MethodBase resolvedMethod;
            public readonly bool tokenOutOfRange;
            public ResolveMethodResult(MethodBase resolvedMethod, bool tokenOutOfRange) {
                this.resolvedMethod = resolvedMethod;
                this.tokenOutOfRange = tokenOutOfRange;
            }
        }
        
        ResolveMethodState TryResolveMethod(SMethod originalMethod, MethodBase patchMethod) {
            var state = new ResolveMethodState(originalMethod, offset: 0, tryLowerTokens: true, tryHigherTokens: true, match: null);
            var result = TryResolveMethodCore(state.originalMethod, patchMethod, 0);
            if(result.resolvedMethod != null) {
                return state.With(match: result.resolvedMethod);
            }
            state = state.With(offset: 1);
            const int tries = 100000;
            while(state.offset <= tries && (state.tryHigherTokens || state.tryLowerTokens)) {
                if(state.tryHigherTokens) {
                    result = TryResolveMethodCore(originalMethod, patchMethod, state.offset);
                    if(result.resolvedMethod != null) {
                        return state.With(match: result.resolvedMethod);
                    } else if(result.tokenOutOfRange) {
                        state = state.With(tryHigherTokens: false);
                    }
                }
                if(state.tryLowerTokens) {
                    result = TryResolveMethodCore(originalMethod, patchMethod, -state.offset);
                    if(result.resolvedMethod != null) {
                        return state.With(match: result.resolvedMethod);
                    } else if(result.tokenOutOfRange) {
                        state = state.With(tryLowerTokens: false);
                    }
                }
                state = state.With(offset: state.offset + 1);
            }
            return state;
        }
        
        
        ResolveMethodResult TryResolveMethodCore(SMethod methodToResolve, MethodBase patchMethod, int offset) {
            bool tokenOutOfRange = false;
            MethodBase resolvedMethod = null;
            try {
                resolvedMethod = TryGetMethodBaseWithRelativeToken(methodToResolve, offset);
                var err = MethodCompatiblity.CheckCompatibility(resolvedMethod, patchMethod);
                if(err != null) {
                    // if (resolvedMethod.Name == patchMethod.Name) {
                    //     Log.Info(err);
                    // }
                    resolvedMethod = null;
                }
            } catch (SymbolResolvingFailedException ex) when(ex.InnerException is ArgumentOutOfRangeException) {
                tokenOutOfRange = true;
            } catch (ArgumentOutOfRangeException) {
                tokenOutOfRange = true;
            }
            return new ResolveMethodResult(resolvedMethod, tokenOutOfRange);
        }
        
        MethodBase TryGetMethodBaseWithRelativeToken(SMethod sOriginalMethod, int offset) {
            return symbolResolver.Resolve(new SMethod(sOriginalMethod.assemblyName, 
                sOriginalMethod.displayName, 
                sOriginalMethod.metadataToken + offset,
                sOriginalMethod.simpleName));
        }
    
        string HandleMethodPatchFailure(SMethod method, Exception exception) {
            return $"Edit requires full recompile to apply: Failed to apply patch for method {method.displayName} in assembly {method.assemblyName}.\nCommon causes: editing code that failed to patch previously, an unsupported change, or a real bug in Hot Reload.\nIf you think this is a bug, please report the issue on Discord and include a code-snippet before/after.\nException: {exception}";
        }

        void EnsureSymbolResolver() {
            if (symbolResolver == null) {
                var searchPaths = new HashSet<string>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var assembliesByName = new Dictionary<string, List<Assembly>>();
                for (var i = 0; i < assemblies.Length; i++) {
                    var name = assemblies[i].GetNameSafe();
                    List<Assembly> list;
                    if (!assembliesByName.TryGetValue(name, out list)) {
                        assembliesByName.Add(name, list = new List<Assembly>());
                    }
                    list.Add(assemblies[i]);
                    
                    if(assemblies[i].IsDynamic) continue;

                    var location = assemblies[i].Location;
                    if(File.Exists(location)) {
                        searchPaths.Add(Path.GetDirectoryName(Path.GetFullPath(location)));
                    }
                }
                symbolResolver = new SymbolResolver(assembliesByName);
                assemblySearchPaths = searchPaths.ToArray();
            }
        }
        
        
        //Allow one save operation at a time.
        readonly SemaphoreSlim gate = new SemaphoreSlim(1);
        public async Task SaveAppliedPatches(string filePath) {
            await gate.WaitAsync();
            try {
                await SaveAppliedPatchesNoLock(filePath);
            } finally {
                gate.Release();
            }
        }
        
        async Task SaveAppliedPatchesNoLock(string filePath) {
            if (filePath == null) {
                throw new ArgumentNullException(nameof(filePath));
            }
            filePath = Path.GetFullPath(filePath);
            var dir = Path.GetDirectoryName(filePath);
            if(string.IsNullOrEmpty(dir)) {
                throw new ArgumentException("Invalid path: " + filePath, nameof(filePath));
            }
            Directory.CreateDirectory(dir);
            var history = patchHistory.ToList();
            
            PlayerLog("Saving {0} applied patches to {1}", history.Count, filePath);

            await Task.Run(() => {
                using (FileStream fs = File.Create(filePath))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter writer = new JsonTextWriter(sw)) {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                        Converters = new List<JsonConverter> { new MethodPatchResponsesConverter() }
                    });
                    serializer.Serialize(writer, history);
                }
            });
        }
        
        public void InitPatchesBlocked(string filePath) {
            seenResponses.Clear();
            var file = new FileInfo(filePath);
            if (file.Exists) {
                using(var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                using (StreamReader sr = new StreamReader(fs))
                using (JsonReader reader = new JsonTextReader(sr)) {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                        Converters = new List<JsonConverter> { new MethodPatchResponsesConverter() }
                    });
                    pendingPatches = serializer.Deserialize<List<MethodPatchResponse>>(reader);
                }
                ApplyPatches(persist: false);
            }
        }
        
        
        [StringFormatMethod("format")]
        static void PlayerLog(string format, params object[] args) {
#if !UNITY_EDITOR
            HotReload.Log.Info(format, args);
#endif //!UNITY_EDITOR
        }
        
        class SimpleMethodComparer : IEqualityComparer<SMethod> {
            public static readonly SimpleMethodComparer I = new SimpleMethodComparer();
            SimpleMethodComparer() { }
            public bool Equals(SMethod x, SMethod y) => x.metadataToken == y.metadataToken;
            public int GetHashCode(SMethod x) {
                return x.metadataToken;
            }
        }
    }
}
#endif
