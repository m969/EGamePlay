using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Build.Player;

namespace ET
{
    public static class BuildAssembliesHelper
    {
        public class Define
        {
            public static string BuildOutputDir = "./DllDatas";
        }
        /// <summary>
        /// Unity线程的同步上下文
        /// </summary>
        static SynchronizationContext unitySynchronizationContext { get; set; }

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            unitySynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// 执行编译代码流程
        /// </summary>
        //[MenuItem("EcsNode/DoCompile")]
        public static void DoCompile()
        {
            // 强制刷新一下，防止关闭auto refresh，编译出老代码
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            bool isCompileOk = CompileDlls();
            if (!isCompileOk)
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }
                return;
            }

            Debug.Log("DoCompile finished");
        }

        /// <summary>
        /// 编译成dll
        /// </summary>
        static bool CompileDlls()
        {
            // 运行时编译需要先设置为UnitySynchronizationContext, 编译完再还原为CurrentContext
            var lastSynchronizationContext = Application.isPlaying ? SynchronizationContext.Current : null;
            SynchronizationContext.SetSynchronizationContext(unitySynchronizationContext);

            bool isCompileOk = false;

            try
            {
                Directory.CreateDirectory(Define.BuildOutputDir);
                //BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                var target = BuildTarget.Stadia;
                var group = BuildPipeline.GetBuildTargetGroup(target);
                var scriptCompilationSettings = new ScriptCompilationSettings()
                {
                    group = group,
                    target = target,
                    extraScriptingDefines = new[] { "UNITY_COMPILE", "UNITY" },
                    options = EditorUserBuildSettings.development ? ScriptCompilationOptions.DevelopmentBuild : ScriptCompilationOptions.None
                };
                var result = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, Define.BuildOutputDir);
                isCompileOk = result.assemblies.Count > 0;
                EditorUtility.ClearProgressBar();
            }
            finally
            {
                if (lastSynchronizationContext != null)
                {
                    SynchronizationContext.SetSynchronizationContext(lastSynchronizationContext);
                }
            }

            return isCompileOk;
        }
    }
}