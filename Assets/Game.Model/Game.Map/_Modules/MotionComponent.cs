using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameUtils;
using Sirenix.OdinInspector;
using ECS;
using ECSGame;
#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 运动组件，在这里管理战斗实体的移动、跳跃、击飞等运动功能
    /// </summary>
    public sealed class MotionComponent : EcsComponent
    {
        public Vector3 Position
        {
            get { return Entity.GetComponent<TransformComponent>().Position; }
            set { Entity.GetComponent<TransformComponent>().Position = value; }
        }
        public Quaternion Rotation
        {
            get { return Entity.GetComponent<TransformComponent>().Rotation; }
            set { Entity.GetComponent<TransformComponent>().Rotation = value; }
        }
        public bool CanMove { get; set; }
        public GameTimer IdleTimer { get; set; }
        public GameTimer MoveTimer { get; set; }
        public Vector3 MoveVector { get; set; }
        public Vector3 originPos;
    }
}