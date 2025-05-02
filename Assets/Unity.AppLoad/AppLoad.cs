using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using ECSGame;
using ECSUnity;
using EGamePlay;
using EGamePlay.Combat;

public class Define
{
    public static string BuildOutputDir = "./DllDatas";
}

public class AppLoad : MonoBehaviour
{
    public static bool NeedReload { get; set; } = false;
    public static bool NeedReloadShare { get; set; } = false;
    private EcsNode EcsNode { get; set; }
    private Dictionary<string, string> ScriptFiles {  get; set; } = new Dictionary<string, string>();
    public GameObject ReloadPanelObj;
    public ReferenceCollector ConfigsCollector;
    public ReferenceCollector PrefabsCollector;
    public AbilityConfigObject SkillConfigObject;

    // Start is called before the first frame update
    void Awake()
    {
        NeedReloadShare = true;

        ET.ETTask.ExceptionHandler = (e) =>
        {
            Debug.LogException(e);
        };

        ConsoleLog.LogAction = Debug.Log;
        ConsoleLog.LogErrorAction = Debug.LogError;
        ConsoleLog.LogExceptionAction = Debug.LogException;

        CheckScriptFiles();

        EcsNode = new EcsNode(1);
        EcsNode.Id = EcsNode.NewInstanceId();
        StaticClient.EcsNode = EcsNode;
        EcsNode.RegisterDrive<IAwake>();
        EcsNode.RegisterDrive<IEnable>();
        EcsNode.RegisterDrive<IDisable>();
        EcsNode.RegisterDrive<IDestroy>();
        EcsNode.RegisterDrive<IInit>();
        EcsNode.RegisterDrive<IAfterInit>();
        EcsNode.RegisterDrive<IOnChange>();
        EcsNode.RegisterDrive<IUpdate>();
        EcsNode.RegisterDrive<IFixedUpdate>();
        //EcsNode.RegisterDrive<IOnComponentChange>();

        StaticClient.ConfigsCollector = ConfigsCollector;
        StaticClient.PrefabsCollector = PrefabsCollector;
        Process_GameSystem.RpgInit(EcsNode, typeof(Process_GameSystem).Assembly);
    }

    public void Reload()
    {
        Process_GameSystem.Reload(EcsNode, typeof(Process_GameSystem).Assembly);
    }

    bool CheckScriptFiles()
    {
        var changed = false;
#if UNITY_EDITOR
        var allAssets = UnityEditor.AssetDatabase.FindAssets("t:Script", new string[] { "Assets/Game.System" });
        var newAssets = new List<string>();
        foreach (var item in allAssets)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(item);
            if (path.EndsWith(".cs"))
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var time = File.GetLastWriteTimeUtc(Path.Combine(Application.dataPath, "../" + path));
                if (!ScriptFiles.ContainsKey(path))
                {
                    changed = true;
                    ScriptFiles.Add(path, time.ToString());
                }
                else
                {
                    if (!ScriptFiles[path].Equals(time.ToString()))
                    {
                        changed = true;
                    }
                    ScriptFiles[path] = time.ToString();
                    //ConsoleLog.Debug($"{path} {time.ToString()}");
                }
            }
        }
#endif

        return changed;
    }

    // Update is called once per frame
    void Update()
    {
        EcsNode?.DriveEntityUpdate();
    }

    void FixedUpdate()
    {
        EcsNode?.DriveEntityFixedUpdate();
    }
}
