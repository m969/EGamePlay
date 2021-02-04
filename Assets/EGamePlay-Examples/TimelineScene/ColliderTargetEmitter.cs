using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using UnityEngine;

public class ColliderTargetEmitter : SignalEmitter
{
    [LabelText("碰撞体名称")]
    public string ColliderName;


    public override void OnInitialize(TrackAsset aPent)
    {
        base.OnInitialize(aPent);
        retroactive = true;
        emitOnce = true;
    }
}
