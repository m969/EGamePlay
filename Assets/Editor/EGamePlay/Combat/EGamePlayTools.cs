using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine;
using UnityEditor;

namespace EGamePlay
{
    public static class EGamePlayTools
    {
        [MenuItem("Tools/EGamePlay/使用技能Excel配置")]
        public static void UseExcel()
        {
            var dedfine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, $"{dedfine};EGAMEPLAY_EXCEL");
        }

        [MenuItem("Tools/EGamePlay/使用技能Excel配置", true)]
        public static bool IsUseExcel()
        {
            var dedfine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            return !dedfine.Contains("EGAMEPLAY_EXCEL");
        }

        [MenuItem("Tools/EGamePlay/使用技能ScripableObj配置")]
        public static void UseScripableObj()
        {
            var dedfine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            dedfine = dedfine.Replace(";EGAMEPLAY_EXCEL", "");
            dedfine = dedfine.Replace("EGAMEPLAY_EXCEL;", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, dedfine);
        }

        [MenuItem("Tools/EGamePlay/使用技能ScripableObj配置", true)]
        public static bool IsUseScripableObj()
        {
            var dedfine = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            return dedfine.Contains("EGAMEPLAY_EXCEL");
        }
    }
}