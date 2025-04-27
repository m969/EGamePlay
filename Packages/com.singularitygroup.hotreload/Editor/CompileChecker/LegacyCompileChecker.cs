#if !UNITY_2019_1_OR_NEWER
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SingularityGroup.HotReload.Editor {
    class LegacyCompileChecker : ICompileChecker {
        const string timestampFilePath = PackageConst.LibraryCachePath + "/lastCompileTimestamp.txt";
        public bool hasCompileErrors { get; }
        const string assemblyPath = "Library/ScriptAssemblies";
        bool recompile;
        public LegacyCompileChecker() {
            Task.Run(() => {
                var info = new DirectoryInfo(assemblyPath);
                if(!info.Exists) {
                    return;
                }
                var currentCompileTimestamp = default(DateTime);
                foreach (var file in info.GetFiles("*.dll")) {
                    var fileWriteDate = file.LastWriteTimeUtc;
                    if(fileWriteDate > currentCompileTimestamp) {
                        currentCompileTimestamp = fileWriteDate;
                    }
                }
                if(File.Exists(timestampFilePath)) {
                    var lastTimestampStr = File.ReadAllText(timestampFilePath);
                    var lastTimestamp = DateTime.ParseExact(lastTimestampStr, "o", CultureInfo.CurrentCulture).ToUniversalTime();
                    if(currentCompileTimestamp > lastTimestamp) {
                        ThreadUtility.RunOnMainThread(() => {
                            recompile = true;
                            _onCompilationFinished?.Invoke();
                        });
                    }
                }
                Directory.CreateDirectory(Path.GetDirectoryName(timestampFilePath));
                File.WriteAllText(timestampFilePath, currentCompileTimestamp.ToString("o"));
            });
        }

        Action _onCompilationFinished;
        public event Action onCompilationFinished {
            add {
                if(recompile && value != null) {
                    value();
                }
                _onCompilationFinished += value;
            }
            remove {
                _onCompilationFinished -= value;
            }
        }
    }
}
#endif