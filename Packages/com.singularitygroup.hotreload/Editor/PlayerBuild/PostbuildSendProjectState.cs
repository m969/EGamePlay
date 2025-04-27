using System;
using SingularityGroup.HotReload.Editor.Cli;
using UnityEditor;
using UnityEditor.Build;

namespace SingularityGroup.HotReload.Editor {
#pragma warning disable CS0618
    class PostbuildSendProjectState : IPostprocessBuild {
#pragma warning restore CS0618
        public int callbackOrder => 9999;
        public void OnPostprocessBuild(BuildTarget target, string path) {
            try {
                if (!HotReloadBuildHelper.IncludeInThisBuild()) {
                    return;
                }
                // after build passes, need to send again because EditorApplication.delayCall isn't called.
                var buildInfo = BuildInfoHelper.GenerateBuildInfoMainThread();
                HotReloadCli.PrepareBuildInfo(buildInfo);
            } catch (BuildFailedException) {
                throw;
            } catch (Exception e) {
                throw new BuildFailedException(e);
            }
        }
    }
}