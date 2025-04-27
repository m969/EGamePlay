using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;

#pragma warning disable CS0618
namespace SingularityGroup.HotReload.Editor {
    public class BuildGenerateBuildInfo : IPreprocessBuild, IPostprocessBuild {
        public int callbackOrder => 10;

        public void OnPreprocessBuild(BuildTarget target, string path) {
            try {
                if (!HotReloadBuildHelper.IncludeInThisBuild()) {
                    return;
                }
                // write BuildInfo json into the StreamingAssets directory
                GenerateBuildInfo(BuildInfo.GetStoredPath(), target);
            } catch (BuildFailedException) {
                throw;
            } catch (Exception e) {
                throw new BuildFailedException(e);
            }
        }
        
        private static void GenerateBuildInfo(string buildFilePath, BuildTarget buildTarget) {
            var buildInfo = BuildInfoHelper.GenerateBuildInfoMainThread(buildTarget);
            // write to StreamingAssets
            // create StreamingAssets folder if not exists (in-case project has no StreamingAssets files)
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(buildFilePath));
            File.WriteAllText(buildFilePath, buildInfo.ToJson());
        }
        
        public void OnPostprocessBuild(BuildTarget target, string path) {
            try {
                File.Delete(BuildInfo.GetStoredPath());
            } catch {
                // ignore 
            }
        }
    }
}
