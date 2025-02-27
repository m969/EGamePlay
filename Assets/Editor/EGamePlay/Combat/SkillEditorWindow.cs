using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;
using ET;
using EGamePlay.Combat;
using Sirenix.OdinInspector;
using DG.DemiEditor;

public enum AbilityType
{
    Skill,
    Buff
}

public enum ConditionType
{
    StateCheck,
}

public class SkillEditorWindow : OdinMenuEditorWindow
{
    public static string SkillConfigObjectsPath => AbilityManagerObject.Instance.SkillAssetFolder;
    public static string ExecutionObjectsPath => AbilityManagerObject.Instance.ExecutionAssetFolder;

    public Dictionary<string, ExecutionObject> ExecutionObjects = new Dictionary<string, ExecutionObject>();


    public class SkillConfigData
    {
        public AbilityConfigObject ConfigObject;
        public ExecutionObject ExecutionObject;
    }

    [MenuItem("Tools/EGamePlay/SkillEditorWindow")]
    private static void Open()
    {
        var window = GetWindow<SkillEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 800);
        //window.OnEndGUI += window.OnEndGUIAction;
    }

    AbilityConfigCategory SkillConfigCategory;

    private List<string> allConditions = new List<string>();
    int totalCount = 0;
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 32.00f;
        tree.Config.DrawSearchToolbar = true;

        var configsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bundles/Configs.prefab");
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(TimerManager));
        var configsCollector = configsPrefab.GetComponent<ReferenceCollector>();
        if (configsCollector != null)
        {
            var configText = configsCollector.Get<TextAsset>("AbilityConfig");
            var configTypeName = $"ET.AbilityConfig";
            var configType = assembly.GetType(configTypeName);
            var typeName = $"ET.AbilityConfigCategory";
            var configCategoryType = assembly.GetType(typeName);
            var configCategory = Activator.CreateInstance(configCategoryType) as ET.AbilityConfigCategory;
            configCategory.ConfigText = configText.text;
            configCategory.BeginInit();
            SkillConfigCategory = configCategory;
        }
        var allSkill = SkillConfigCategory.GetAll();
        foreach (var item in allSkill.Values)
        {
            if (item.Type == "Buff") continue;
            var path = $"{SkillConfigObjectsPath}/Skill_{item.Id}.asset";
            var path2 = $"{ExecutionObjectsPath}/Execution_{item.Id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<AbilityConfigObject>(path);
            var asset2 = AssetDatabase.LoadAssetAtPath<ExecutionObject>(path2);
            if (asset != null)
            {
                foreach (TriggerConfig triggerConfig in asset.TriggerActions)
                {
                    if (triggerConfig.StateCheckList.Count > 0)
                    {
                        continue;
                    }
                    allConditions.AddRange(triggerConfig.StateCheckList);
                }
            }

            //var data = new SkillConfigData();
            //data.ConfigObject = asset;
            //data.ExecutionObject = asset2;
            var key = $"{item.Id}_{item.Name}";
            tree.Add(key, asset);
            ExecutionObjects[key] = asset2;
            //tree.AddAllAssetsAtPath("", "Assets/EGPsExamples/Resources/SkillConfigs/", typeof(SkillConfigObject), true);
        }
        //tree.AddAllAssetsAtPath("", "Assets/Plugins/Sirenix/Demos/SAMPLE - RPG Editor/Items", typeof(Item), true);
        //.ForEach(this.AddDragHandles);

        //tree.EnumerateTree().AddIcons<Item>(x => x.Icon);

        return tree;
    }

    private AbilityType enumType = AbilityType.Skill;
    private ConditionType conditionType = ConditionType.StateCheck;
    protected override void DrawMenu()
    {
        //GUILayout.BeginHorizontal();
        //if (SirenixEditorGUI.Button("Skill", ButtonSizes.Medium))
        //{

        //}
        //if (SirenixEditorGUI.Button("Buff", ButtonSizes.Medium))
        //{

        //}
        //GUILayout.EndHorizontal();
        enumType = (AbilityType)EditorGUILayout.EnumPopup(enumType, GUILayout.ExpandWidth(true));
        base.DrawMenu();
    }

    protected override void OnImGUI()
    {
        base.OnImGUI();
    }

    protected override void OnBeginDrawEditors()
    {

        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        bool changed = false;
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            var data = selected?.Value as AbilityConfigObject;
            EditorGUILayout.ObjectField(data, typeof(AbilityConfigObject), false);
            if (GUILayout.Button("Select In Editor"))
            {
                Selection.objects = this.MenuTree.Selection.Where(_ => _.Value is AbilityConfigObject)
                    .Select(_ => (_.Value as AbilityConfigObject)).ToArray();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();

        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            var data = ExecutionObjects[selected?.Name];
            if (data != null)
            {
                EditorGUILayout.ObjectField(data, typeof(ExecutionObject), false);
                if (GUILayout.Button("Select In Editor"))
                {
                    EditorGUIUtility.PingObject(data);
                }
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();

        if (changed)
        {
            ForceMenuTreeRebuild();
        }
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();
    }

    protected override void OnEndDrawEditors()
    {
        base.OnEndDrawEditors();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal(GUILayout.Width(360));
        GUILayout.BeginVertical();
        conditionType = (ConditionType)EditorGUILayout.EnumPopup(conditionType, GUILayout.ExpandWidth(true));
        foreach (var condition in allConditions)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("复制", GUILayout.Width(60)))
            {

            }
            GUILayout.Label(condition);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();
    }

    protected void OnEndGUIAction()
    {

    }
}


//[LabelText("执行体事件类型")]
//public enum ExecutionEventType
//{
//    [LabelText("触发应用效果")]
//    TriggerApplyEffect,
//    [LabelText("生成碰撞体")]
//    TriggerSpawnCollider,
//}

#if !SERVER
//public class ExecutionEventEmitter : SignalEmitter
//{
//    public ExecutionEventType ExecutionEventType;
//    [LabelText("碰撞体名称")]
//    public string ColliderName;
//    public CollisionMoveType ColliderType;
//    [LabelText("存活时间")]
//    public float ExistTime;
//    public EffectApplyType EffectApplyType;


//    public override void OnInitialize(TrackAsset aPent)
//    {
//        base.OnInitialize(aPent);
//        retroactive = true;
//        emitOnce = true;
//    }
//}

//#if UNITY_EDITOR
//[CustomEditor(typeof(ExecutionEventEmitter))]
//public class ExecutionEventEmitterInspector : OdinEditor
//{
//    protected override void OnEnable()
//    {
//        base.OnEnable();

//        var emitter = target as ExecutionEventEmitter;
//        if (emitter.asset == null)
//        {
//            SignalAsset signalAsset = null;
//            var arr = AssetDatabase.FindAssets("t:SignalAsset", new string[] { "Assets" });
//            foreach (var item in arr)
//            {
//                signalAsset = AssetDatabase.LoadAssetAtPath<SignalAsset>(AssetDatabase.GUIDToAssetPath(item));
//                if (signalAsset != null) break;
//            }
//            //var signalAsset = AssetDatabase.LoadAssetAtPath<SignalAsset>("Assets/EGPsExamples/TimelineScene/效果1.signal");
//            emitter.asset = signalAsset;
//            serializedObject.ApplyModifiedProperties();
//        }
//    }

//    public override void OnInspectorGUI()
//    {
//        //EditorGUILayout.Space(20);
//        //base.OnInspectorGUI();

//        serializedObject.Update();

//        //var editorType = typeof(Editor);
//        //editorType.InvokeMember("DoDrawDefaultInspector",
//        //    System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static 
//        //     | System.Reflection.BindingFlags.NonPublic, null, null, 
//        //    new object[] {serializedObject});
//        //return;

//        var emitter = target as ExecutionEventEmitter;
//        emitter.time = EditorGUILayout.FloatField("Time", (float)emitter.time);
//        emitter.retroactive = EditorGUILayout.Toggle("Retroactive", emitter.retroactive);
//        emitter.emitOnce = EditorGUILayout.Toggle("EmitOnce", emitter.emitOnce);
//        EditorGUILayout.Space(20);
//        emitter.ExecutionEventType = (ExecutionEventType)SirenixEditorFields.EnumDropdown("事件类型", emitter.ExecutionEventType);

//        if (emitter.ExecutionEventType == ExecutionEventType.TriggerSpawnCollider)
//        {
//            emitter.ColliderName = EditorGUILayout.TextField("碰撞体名称", emitter.ColliderName);
//            emitter.ColliderType = (CollisionMoveType)SirenixEditorFields.EnumDropdown("碰撞体类型", emitter.ColliderType);
//            if (emitter.ColliderType == CollisionMoveType.SelectedDirection
//                || emitter.ColliderType == CollisionMoveType.SelectedPosition
//                || emitter.ColliderType == CollisionMoveType.ForwardFly
//                )
//            {
//                //emitter.ColliderShape = (ColliderShape)SirenixEditorFields.EnumDropdown("碰撞体形状", emitter.ColliderShape);
//                emitter.ExistTime = EditorGUILayout.FloatField("存活时间", emitter.ExistTime);
//            }
//            emitter.EffectApplyType = (EffectApplyType)EditorGUILayout.EnumPopup("应用效果", emitter.EffectApplyType);
//        }

//        if (emitter.ExecutionEventType == ExecutionEventType.TriggerApplyEffect)
//        {
//            emitter.EffectApplyType = (EffectApplyType)EditorGUILayout.EnumPopup("应用效果", emitter.EffectApplyType);
//        }

//        serializedObject.ApplyModifiedProperties();
//        if (!EditorUtility.IsDirty(emitter))
//        {
//            EditorUtility.SetDirty(emitter);
//        }
//    }
//}
//#endif
#endif