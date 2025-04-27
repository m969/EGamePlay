using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using DG.Tweening;
using System;
using ECS;

namespace EGamePlay.Combat
{
    public enum MoveType
    {
        TargetMove,
        PathMove,
        DestinationMove,
    }

    public enum SpeedType
    {
        Speed,
        Duration,
    }

    //public class MoveWithDotweenComponent : EcsComponent
    //{
    //    public SpeedType SpeedType { get; set; }
    //    public float Speed { get; set; }
    //    public float Duration { get; set; }
    //    public float ElapsedTime { get; set; }
    //    public IPosition PositionEntity { get; set; }
    //    public IPosition TargetPositionEntity { get; set; }
    //    public EcsEntity TargetEntity { get; set; }
    //    public Vector3 Destination { get; set; }
    //    public Tweener MoveTweener { get; set; }
    //    public System.Action MoveFinishAction { get; set; }
    //}
}