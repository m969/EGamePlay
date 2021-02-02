using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using UnityEngine;

public class ColliderProjectileEmitter : SignalEmitter
{
    [LabelText("碰撞体名称")]
    public string ColliderName;
    [LabelText("存活时间")]
    public float ExistTime;


    public override void OnInitialize(TrackAsset aPent)
    {
        base.OnInitialize(aPent);
        retroactive = true;
        emitOnce = true;
    }
}
