#if !SERVER
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using UnityEngine.Timeline;
#endif

[LabelText("碰撞体形状")]
public enum ColliderShape
{
    [LabelText("矩形")]
    Box,
    [LabelText("圆形")]
    Circle,
    [LabelText("扇形")]
    Sector,
}

[LabelText("碰撞体类型")]
public enum ColliderType
{
    [LabelText("固定位置碰撞体")]
    FixedPosition,
    [LabelText("固定朝向碰撞体")]
    FixedDirection,
    [LabelText("目标飞行碰撞体")]
    TargetFly,
    [LabelText("朝向飞行碰撞体")]
    ForwardFly,
}

[LabelText("应用效果")]
public enum EffectApplyType
{
    [LabelText("全部效果")]
    Effects,
    [LabelText("效果1")]
    Effect1,
    [LabelText("效果2")]
    Effect2,
    [LabelText("效果3")]
    Effect3,

    [LabelText("其他")]
    Other = 100,
}

[LabelText("执行体事件类型")]
public enum ExecutionEventType
{
    [LabelText("触发应用效果")]
    TriggerApplyEffect,
    [LabelText("生成碰撞体")]
    TriggerSpawnCollider,
}

#if !SERVER
public class ExecutionEventEmitter : SignalEmitter
{
    public ExecutionEventType ExecutionEventType;
    [LabelText("碰撞体名称")]
    public string ColliderName;
    public ColliderType ColliderType;
    [LabelText("存活时间")]
    public float ExistTime;
    public EffectApplyType EffectApplyType;


    public override void OnInitialize(TrackAsset aPent)
    {
        base.OnInitialize(aPent);
        retroactive = true;
        emitOnce = true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ExecutionEventEmitter))]
public class ExecutionEventEmitterInspector : OdinEditor
{
    protected override void OnEnable()
    {
        base.OnEnable();

        var emitter = target as ExecutionEventEmitter;
        if (emitter.asset == null)
        {
            SignalAsset signalAsset = null;
            var arr = AssetDatabase.FindAssets("t:SignalAsset", new string[] { "Assets" });
            foreach (var item in arr)
            {
                signalAsset = AssetDatabase.LoadAssetAtPath<SignalAsset>(AssetDatabase.GUIDToAssetPath(item));
                if (signalAsset != null) break;
            }
            //var signalAsset = AssetDatabase.LoadAssetAtPath<SignalAsset>("Assets/EGPsExamples/TimelineScene/效果1.signal");
            emitter.asset = signalAsset;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        //EditorGUILayout.Space(20);
        //base.OnInspectorGUI();

        serializedObject.Update();

        //var editorType = typeof(Editor);
        //editorType.InvokeMember("DoDrawDefaultInspector",
        //    System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static 
        //     | System.Reflection.BindingFlags.NonPublic, null, null, 
        //    new object[] {serializedObject});
        //return;

        var emitter = target as ExecutionEventEmitter;
        emitter.time = EditorGUILayout.FloatField("Time", (float)emitter.time);
        emitter.retroactive = EditorGUILayout.Toggle("Retroactive", emitter.retroactive);
        emitter.emitOnce = EditorGUILayout.Toggle("EmitOnce", emitter.emitOnce);
        EditorGUILayout.Space(20);
        emitter.ExecutionEventType = (ExecutionEventType)SirenixEditorFields.EnumDropdown("事件类型", emitter.ExecutionEventType);

        if (emitter.ExecutionEventType == ExecutionEventType.TriggerSpawnCollider)
        {
            emitter.ColliderName = EditorGUILayout.TextField("碰撞体名称", emitter.ColliderName);
            emitter.ColliderType = (ColliderType)SirenixEditorFields.EnumDropdown("碰撞体类型", emitter.ColliderType);
            if (emitter.ColliderType == ColliderType.FixedDirection
                || emitter.ColliderType == ColliderType.FixedPosition
                || emitter.ColliderType == ColliderType.ForwardFly
                )
            {
                //emitter.ColliderShape = (ColliderShape)SirenixEditorFields.EnumDropdown("碰撞体形状", emitter.ColliderShape);
                emitter.ExistTime = EditorGUILayout.FloatField("存活时间", emitter.ExistTime);
            }
            emitter.EffectApplyType = (EffectApplyType)EditorGUILayout.EnumPopup("应用效果", emitter.EffectApplyType);
        }

        if (emitter.ExecutionEventType == ExecutionEventType.TriggerApplyEffect)
        {
            emitter.EffectApplyType = (EffectApplyType)EditorGUILayout.EnumPopup("应用效果", emitter.EffectApplyType);
        }

        serializedObject.ApplyModifiedProperties();
        if (!EditorUtility.IsDirty(emitter))
        {
            EditorUtility.SetDirty(emitter);
        }
    }
}
#endif
#endif