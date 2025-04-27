#if UNITY_2019_1_OR_NEWER
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    class DefaultCompileChecker : ICompileChecker {
        const string recompileFilePath = PackageConst.LibraryCachePath + "/recompile.txt";
        public bool hasCompileErrors { get; private set;  }
        bool recompile;
        public DefaultCompileChecker() {
            CompilationPipeline.assemblyCompilationFinished += DetectCompileErrors;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            var currentSessionId = EditorAnalyticsSessionInfo.id;
            Task.Run(() => {
                try {
                    var compileSessionId = File.ReadAllText(recompileFilePath);
                    if(compileSessionId == currentSessionId.ToString()) {
                        ThreadUtility.RunOnMainThread(() => {
                            recompile = true;
                            _onCompilationFinished?.Invoke();
                        });
                    }
                    File.Delete(recompileFilePath);
                } catch(DirectoryNotFoundException) {
                   //dir doesn't exist -> no recompile required
                } catch(FileNotFoundException) {
                   //file doesn't exist -> no recompile required
                } catch(Exception ex) {
                    Log.Warning("compile checker encountered issue: {0} {1}", ex.GetType().Name, ex.Message);
                }
            });
        }
        
        void DetectCompileErrors(string _, CompilerMessage[] messages) {
            for (int i = 0; i < messages.Length; i++) {
                if (messages[i].type == CompilerMessageType.Error) {
                    hasCompileErrors = true;
                    return;
                }
            }
            hasCompileErrors = false;
        }

        void OnCompilationFinished(object _) {
            //Don't recompile on compile errors
            if(!hasCompileErrors) {
                Directory.CreateDirectory(Path.GetDirectoryName(recompileFilePath));
                File.WriteAllText(recompileFilePath, EditorAnalyticsSessionInfo.id.ToString());
            }
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