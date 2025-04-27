#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace SingularityGroup.HotReload {
    /// <summary>
    /// Information about the Unity Player build.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This info is used by the HotReload Server to compile your project in the same way that the Unity Player build was compiled.<br/>
    /// For example, when building for Android, Unity sets a bunch of define symbols like UNITY_ANDROID.
    /// </para>
    /// <para>
    /// Information that changes between builds is generated at build-time and put in StreamingAssets/.<br/>
    /// This approach means that builds do not need to modify a project file (making file dirty in git). For example,
    /// whenever user makes a mono build, the CommitHash changes and we need to regenerate the BuildInfo.
    /// </para>
    /// </remarks>
    [Serializable]
    class BuildInfo {
        /// <summary>
        /// Uniquely identifies the Unity project.
        /// </summary>
        /// <remarks>
        /// Used on-device to check if Hot Reload server is compatible with the Unity project (same project).<br/>
        /// When your computer has multiple Unity projects open, each project should provide a different value.<br/>
        /// This identifier must also be the same between two different computers that are collaborating on the same project.
        ///
        /// <para>
        /// Edge-case: when a user copy pastes an entire Unity project and has both open at once,
        /// then it's fine for this identifier to be the same.
        /// </para> 
        /// </remarks>
        public string projectIdentifier;

        /// <summary>
        /// Git commit hash
        /// </summary>
        /// <remarks>
        /// Used to detect that your code is different to when the build was made.
        /// </remarks>
        public string commitHash;

        /// <summary>
        /// List of define symbols that were active when this build was made.
        /// </summary>
        /// <remarks>
        /// Separate the symbols with a semi-colon character ';'
        /// </remarks>
        public string defineSymbols;
        
        /// <summary>
        /// A regex of C# project names (*.csproj) to be omitted from compilation.  
        /// </summary>
        /// <example>
        /// "MyTests|MyEditorAssembly"
        /// </example>
        [FormerlySerializedAs("projectExclusionRegex")]
        public string projectOmissionRegex;

        /// <summary>
        /// The computer that made the Android (or Standalone etc) build.<br/>
        /// The hostname (ip address) where Hot Reload server would be listening.
        /// </summary>
        public string buildMachineHostName;
        
        /// <summary>
        /// The computer that made the Android (or Standalone etc) build.<br/>
        /// The port where Hot Reload server would be listening.
        /// </summary> 
        public int buildMachinePort;

        /// <summary>
        /// Selected build target in Unity Editor.
        /// </summary>
        public string activeBuildTarget;
        
        /// <summary>
        /// Used to pass in the origin onto the phone which is used to identify the correct server.
        /// </summary>
        public string buildMachineRequestOrigin;

        [JsonIgnore]
        public HashSet<string> DefineSymbolsAsHashSet {
            get {
                var symbols = defineSymbols.Trim().Split(';');
                // split on an empty string produces 1 empty string
                if (symbols.Length == 1 && symbols[0] == string.Empty) {
                    return new HashSet<string>();
                }
                return new HashSet<string>(symbols);
            }
        }

        [JsonIgnore]
        public PatchServerInfo BuildMachineServer {
            get {
                if (buildMachineHostName == null || buildMachinePort == 0) {
                    return null;
                }
                return new PatchServerInfo(buildMachineHostName, buildMachinePort, commitHash, null, customRequestOrigin: buildMachineRequestOrigin);
            }
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(this);
        }

        [CanBeNull]
        public static BuildInfo FromJson(string json) {
            if (string.IsNullOrEmpty(json)) {
                return null;
            }
            return JsonConvert.DeserializeObject<BuildInfo>(json);
        }

        /// <summary>
        /// Path to read/write the json file to.
        /// </summary>
        /// <returns>A filepath that is inside the player build</returns>
        public static string GetStoredPath() {
            return Path.Combine(Application.streamingAssetsPath, GetStoredName());
        }

        public static string GetStoredName() {
            return "HotReload_BuildInfo.json";
        }

        /// <returns>True if the commit hashes are definately different, otherwise False</returns>
        public bool IsDifferentCommit(string remoteCommit) {
            if (commitHash == PatchServerInfo.UnknownCommitHash) {
                return false;
            }

            return !SameCommit(commitHash, remoteCommit);
        }

        /// <summary>
        /// Checks whether the commits are equivalent.
        /// </summary>
        /// <param name="commitA"></param>
        /// <param name="commitB"></param>
        /// <returns>False if the commit hashes are definately different, otherwise True</returns>
        public static bool SameCommit(string commitA, string commitB) {
            if (commitA == null) {
                // unknown commit hash, so approve anything
                return true;
            }

            if (commitA.Length == commitB.Length) {
                return commitA == commitB;
            } else if (commitA.Length >= 6 && commitB.Length >= 6) {
                // depending on OS, the git log pretty output has different length (7 or 8 chars)
                // if the longer hash starts with the shorter hash, return true
                // Assumption: commits have different length.
                var longer = commitA.Length > commitB.Length ? commitA : commitB;
                var shorter = commitA.Length > commitB.Length ? commitB : commitA;
                
                return longer.StartsWith(shorter);
            }
            return false;
        }
    }
}
#endif
