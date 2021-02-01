using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

[LabelText("碰撞体形状")]
public enum ColliderShapeType
{
    [LabelText("矩形")]
    Box,
    [LabelText("圆形")]
    Circle,
    [LabelText("扇形")]
    Sector,
}

//[System.Flags]
public enum ProjectileType
{
    [LabelText("飞弹")]
    飞弹,

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
    public ColliderShapeType ColliderType;
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
