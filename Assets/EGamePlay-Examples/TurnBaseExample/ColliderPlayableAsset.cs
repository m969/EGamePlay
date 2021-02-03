using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

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
    [LabelText("固定碰撞体")]
    Fixed,
    [LabelText("飞行碰撞体")]
    FlyMove,
}

[LabelText("飞行类型")]
public enum FlyType
{
    [LabelText("朝前方飞")]
    Forward,
    [LabelText("超目标飞")]
    Target,
}

[LabelText("应用效果")]
public enum EffectApplyType
{
    [LabelText("效果1")]
    Effect1,
    [LabelText("效果2")]
    Effect2,
    [LabelText("效果3")]
    Effect3,

    [LabelText("其他")]
    Other = 100,
}

[System.Serializable]
public class ColliderPlayableAsset : PlayableAsset
{
    public ColliderShape ColliderType;
    //[/*EnumToggleButtons, */HideLabel]
    //public ProjectileType ProjectileType;

    //[LabelText("碰撞体ID"), ShowIf("ProjectileType", ProjectileType.飞弹)]
    //public string ColliderID;

    //[OnInspectorGUI("DrawSeperate", append: false)]
    public EffectApplyType EffectApplyType;
    [LabelText("飞弹")]
    public bool IsProjectile;

    void DrawSeperate()
    {
        UnityEditor.EditorGUILayout.Space();
        //Sirenix.Utilities.Editor.SirenixEditorGUI.DrawThickHorizontalSeparator();
    }


    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        return Playable.Create(graph);
    }
}
