using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;
using ET;
using Log = EGamePlay.Log;
using System;
#if EGAMEPLAY_ET
using Vector3 = Unity.Mathematics.float3;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 技能执行体，执行体就是控制角色表现和技能表现的，包括角色动作、移动、变身等表现的，以及技能生成碰撞体等表现
    /// </summary>
    [EnableUpdate]
    public sealed partial class SkillExecution : Entity, IAbilityExecute
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


        public override void Awake(object initData)
        {
            AbilityEntity = initData as SkillAbility;
            OwnerEntity = GetParent<CombatEntity>();
        }

        public void LoadExecutionEffects()
        {
            AddComponent<ExecuteClipComponent>();
        }

        public override void Update()
        {
            //if (SkillAbility.Spelling == false)
            //{
            //    return;
            //}

            if (ExecutionObject.TotalTime > 0)
            {
                var nowTicks = TimeHelper.ClientNow() - OriginTime;
                var nowSeconds = nowTicks / 1000f;
                if (nowSeconds >= ExecutionObject.TotalTime)
                {
                    EndExecute();
                }
            }
        }

        public void BeginExecute()
        {
            //Log.Debug("SkillExecution BeginExecute");
            OriginTime = TimeHelper.ClientNow();
            GetParent<CombatEntity>().SpellingExecution = this;
            if (SkillAbility != null)
            {
                SkillAbility.Spelling = true;
            }

            GetComponent<ExecuteClipComponent>().BeginExecute();

            if (ExecutionObject != null)
            {
                AddComponent<UpdateComponent>();
            }

            FireEvent(nameof(BeginExecute));
        }

        public void EndExecute()
        {
            //Log.Debug("SkillExecution EndExecute");
            GetParent<CombatEntity>().SpellingExecution = null;
            if (SkillAbility != null)
            {
                SkillAbility.Spelling = false;
            }
            SkillTargets.Clear();
            Entity.Destroy(this);
            //base.EndExecute();
        }
    }
}