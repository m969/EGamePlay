using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;
using ET;
using Log = EGamePlay.Log;
using System;
using ECS;


#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.float3;
using AO;
using AO.EventType;
using ET.EventType;
#else
using float3 = UnityEngine.Vector3;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 技能执行体，执行体就是控制角色表现和技能表现的，包括角色动作、移动、变身等表现的，以及技能生成碰撞体等表现
    /// </summary>
    public sealed partial class AbilityExecution : EcsEntity
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Ability AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }
        public Ability SkillAbility { get { return AbilityEntity as Ability; } }
        public ExecutionObject ExecutionObject { get; set; }
        public List<CombatEntity> SkillTargets { get; set; } = new List<CombatEntity>();
        public CombatEntity InputTarget { get; set; }
        public Vector3 InputPoint { get; set; }
        public Vector3 InputDirection { get; set; }
        public float InputRadian { get; set; }
        public long OriginTime { get; set; }
        /// 行为占用
        public bool ActionOccupy { get; set; } = true;
        public List<ExecuteClip> ExecuteClips { get; private set; } = new();
    }
}