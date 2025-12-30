using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using GameUtils;
using System;
using static UnityEngine.GraphicsBuffer;
using ECS;
using ECSGame;



#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using AO;
using AO.EventType;
using ET.EventType;
#else
using float3 = UnityEngine.Vector3;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : EcsEntity, IPosition
    {
        public string Name { get; set; }
        public Ability AbilityEntity { get; set; }
        public AbilityExecution AbilityExecution { get; set; }
        public ExecuteTriggerType ExecuteTriggerType { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 Position
        {
            get
            {
                return GetComponent<TransformComponent>().Position;
            }
            set
            {
                GetComponent<TransformComponent>().Position = value;
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return GetComponent<TransformComponent>().Rotation;
            }
            set
            {
                GetComponent<TransformComponent>().Rotation = value;
            }
        }
        public CombatEntity TargetEntity { get; set; }
        public CombatEntity OwnerEntity => AbilityEntity.OwnerEntity;
    }
}