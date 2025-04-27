using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using System;
using NaughtyBezierCurves;
using ET;
#if EGAMEPLAY_ET
using AO;
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
#else
using float3 = UnityEngine.Vector3;
using UnityEngine.UIElements;
using ECS;


#endif
#if UNITY
using DG.Tweening;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemMoveComponent : EcsComponent<AbilityItem>
    {
        public IPosition PositionEntity { get; set; }
        public IPosition TargetEntity { get; set; }
        public MoveType MoveType { get; set; }
        public Vector3 TargetPoint { get; set; }
        public bool Rotate { get; set; }
        public float RotateRadian { get; set; }
        public float Duration { get; set; }
        public float Speed { get; set; } = 0.05f;
        public float Progress { get; set; }
        public BezierCurve3D BezierCurve { get; set; }
        public IPosition OriginEntity { get; set; }
        public Vector3 ExecutePoint { get; set; }
        public Vector3 OriginPoint
        {
            get
            {
                if (OriginEntity != null) return OriginEntity.Position;
                else return ExecutePoint;
            }
        }
    }
}