using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

[LabelText("碰撞体类型")]
public enum ColliderType
{
    [LabelText("矩形")]
    Box,
    [LabelText("圆形")]
    Circle,
    [LabelText("扇形")]
    Sector,
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
    public ColliderType ColliderType;
    public EffectApplyType EffectApplyType;


    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        return Playable.Create(graph);
    }
}
