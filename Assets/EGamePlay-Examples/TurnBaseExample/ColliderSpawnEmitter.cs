using System.Reflection;
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
    //public bool IsProjectile;
    public ColliderType ColliderType;
    public ColliderShape ColliderShape;
    //public FlyType FlyType;
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
        //EditorGUILayout.Space(20);
        //base.OnInspectorGUI();
        
        serializedObject.Update();

        //var editorType = typeof(Editor);
        //editorType.InvokeMember("DoDrawDefaultInspector",
        //    System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static 
        //     | System.Reflection.BindingFlags.NonPublic, null, null, 
        //    new object[] {serializedObject});
        //return;
        
        var emitter = target as ColliderSpawnEmitter;
        emitter.time = EditorGUILayout.FloatField("Time", (float)emitter.time);
        emitter.retroactive = EditorGUILayout.Toggle("Retroactive", emitter.retroactive);
        emitter.emitOnce = EditorGUILayout.Toggle("EmitOnce", emitter.emitOnce);
        EditorGUILayout.Space(20);
        emitter.ColliderName = EditorGUILayout.TextField("碰撞体名称", emitter.ColliderName);
        emitter.ColliderType = (ColliderType)SirenixEditorFields.EnumDropdown("碰撞体类型", emitter.ColliderType);
        if (emitter.ColliderType != ColliderType.TargetFly)
        {
            emitter.ColliderShape = (ColliderShape)SirenixEditorFields.EnumDropdown("碰撞体形状", emitter.ColliderShape);
            emitter.ExistTime = EditorGUILayout.FloatField("存活时间", emitter.ExistTime);
        }

        //emitter.IsProjectile = EditorGUILayout.Toggle("飞弹", emitter.IsProjectile);
        //if (emitter.IsProjectile)
        //{
        //    SirenixEditorGUI.IndentSpace();
        //    emitter.ExistTime = EditorGUILayout.FloatField("存活时间", emitter.ExistTime);
        //}
        //else
        //{

        //}
        emitter.EffectApplyType = (EffectApplyType)EditorGUILayout.EnumPopup("应用效果", emitter.EffectApplyType);

        serializedObject.ApplyModifiedProperties();
        
    }
}
#endif
