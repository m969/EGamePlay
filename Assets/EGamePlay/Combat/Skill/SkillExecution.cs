using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Timeline;
using EGamePlay.Combat;
using ET;
using Log = EGamePlay.Log;

namespace EGamePlay.Combat
{
//    public class SkillExecutionData
//    {
//#if !NOT_UNITY
//        public GameObject SkillExecutionAsset { get; set; }
//        public TimelineAsset TimelineAsset { get; set; }
//        public ExecutionObject ExecutionObject { get; set; }
//#endif
//        public float SkillExecuteTime { get; set; }
//        public List<Effect> ExecutionEffects { get; set; } = new List<Effect>();
//    }

    /// <summary>
    /// 技能执行体，执行体就是控制角色表现和技能表现的，包括角色动作、移动、变身等表现的，以及技能生成碰撞体等表现
    /// </summary>
    [EnableUpdate]
    public sealed partial class SkillExecution : Entity, IAbilityExecution
    {
        public Entity AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }
        public SkillAbility SkillAbility { get { return AbilityEntity as SkillAbility; } }
        public ExecutionObject ExecutionObject { get; set; }
        public List<CombatEntity> SkillTargets { get; set; } = new List<CombatEntity>();
        public CombatEntity InputTarget { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }
        public long OriginTime { get; set; }
        /// 行为占用
        public bool ActionOccupy { get; set; } = true;
    }
}