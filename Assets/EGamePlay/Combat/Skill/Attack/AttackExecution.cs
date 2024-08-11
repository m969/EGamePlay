using EGamePlay.Combat;
using UnityEngine;

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
    /// 普攻执行体
    /// </summary>
    public class AttackExecution : Entity, IAbilityExecute
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public AttackAction AttackAction { get; set; }
        public Ability AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }

        private bool BeBlocked;/// 是否被格挡
        private bool HasTriggerDamage;/// 是否触发了伤害


        public override void Update()
        {
            if (!IsDisposed)
            {
                if (!HasTriggerDamage)
                {
                    TryTriggerAttackEffect();
                }
                else
                {
                    this.EndExecute();
                }
            }
        }

        public void SetBlocked()
        {
            BeBlocked = true;
        }

        public void BeginExecute()
        {
            AddComponent<UpdateComponent>();
            //Log.Debug("AttackExecution BeginExecute");
        }

        /// 前置处理
        private void PreProcess()
        {
            AttackAction.Creator.TriggerActionPoint(ActionPointType.PreGiveAttackEffect, AttackAction);
            AttackAction.Target.GetComponent<ActionPointComponent>().TriggerActionPoint(ActionPointType.PreReceiveAttackEffect, AttackAction);
        }

        /// <summary>   尝试触发普攻效果   </summary>
        private void TryTriggerAttackEffect()
        {
            HasTriggerDamage = true;

            PreProcess();

            if (BeBlocked)
            {
                Log.Debug("被格挡了");
            }
            else
            {
                var abilityTriggerComp = AbilityEntity.GetComponent<AbilityTriggerComponent>();
                var effects = abilityTriggerComp.AbilityTriggers;
                for (int i = 0; i < effects.Count; i++)
                {
                    var effect = effects[i];
                    effect.OnTrigger(new TriggerContext() { Target = AttackAction.Target });
                }
            }
        }

        public void EndExecute()
        {
            AttackAction.FinishAction();
            AttackAction = null;
            Entity.Destroy(this);
        }
    }
}