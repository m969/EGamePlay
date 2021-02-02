using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

public class ColliderSpawnEmitter : SignalEmitter
{
    [LabelText("碰撞体名称")]
    public string ColliderName;
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

[System.Serializable]
public class ColliderProjectile
{
    public bool IsProjectile;

}

#if UNITY_EDITOR
[CustomEditor(typeof(ColliderSpawnEmitter))]
public class ColliderSpawnEmitterInspector : OdinEditor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        var emitter = target as ColliderSpawnEmitter;
        emitter.time = EditorGUILayout.FloatField("Time", (float)emitter.time);
        emitter.retroactive = EditorGUILayout.Toggle("Retroactive", emitter.retroactive);
        emitter.emitOnce = EditorGUILayout.Toggle("EmitOnce", emitter.emitOnce);
        EditorGUILayout.Space(20);
        emitter.ColliderName = EditorGUILayout.TextField("碰撞体名称", emitter.ColliderName);
        emitter.ExistTime = EditorGUILayout.FloatField("存活时间", emitter.ExistTime);
        emitter.EffectApplyType = (EffectApplyType)EditorGUILayout.EnumPopup("应用效果", emitter.EffectApplyType);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
