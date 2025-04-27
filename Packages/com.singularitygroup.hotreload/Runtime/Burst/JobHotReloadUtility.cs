#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Reflection;
using SingularityGroup.HotReload.DTO;

namespace SingularityGroup.HotReload.Burst {
    public static class JobHotReloadUtility {
        public static void HotReloadBurstCompiledJobs(SUnityJob jobData, Type proxyJobType) {
            JobPatchUtility.PatchBurstCompiledJobs(jobData, proxyJobType, unityMajorVersion:
    #if UNITY_2022_2_OR_NEWER
                2022
    #elif UNITY_2021_3_OR_NEWER
                2021
    #elif UNITY_2020_3_OR_NEWER
                2020
    #elif UNITY_2019_4_OR_NEWER
                2019
    #else
                2018
    #endif
            );
        }
    }
}
#endif
