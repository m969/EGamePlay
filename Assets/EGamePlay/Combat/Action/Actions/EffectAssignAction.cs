using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    public class EffectAssignAbility : EffectActionAbility<EffectAssignAction>
    {

    }

    /// <summary>
    /// 赋给效果行动
    /// </summary>
    public class EffectAssignAction : ActionExecution<EffectAssignAbility>
    {
        //创建这个效果赋给行动的源能力
        public AbilityEntity SourceAbility { get; set; }
        public Effect EffectConfig => AbilityEffect.EffectConfig;


        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyEffectAssign()
        {
            PreProcess();
            if (EffectConfig is DamageEffect)
            {
                if (OwnerEntity.DamageActionAbility.TryCreateAction(out var action))
                {
                    action.Target = Target;
                    action.AbilityEffect = AbilityEffect;
                    action.ExecutionEffect = ExecutionEffect;
                    action.DamageSource = DamageSource.Skill;
                    action.ApplyDamage();
                }
            }

            if (EffectConfig is CureEffect && Target.CurrentHealth.IsFull() == false)
            {
                if (OwnerEntity.CureActionAbility.TryCreateAction(out var action))
                {
                    action.Target = Target;
                    action.AbilityEffect = AbilityEffect;
                    action.ExecutionEffect = ExecutionEffect;
                    action.ApplyCure();
                }
            }

            if (EffectConfig is AddStatusEffect)
            {
                if (OwnerEntity.AddStatusActionAbility.TryCreateAction(out var action))
                {
                    action.SourceAbility = SourceAbility;
                    action.Target = Target;
                    action.AbilityEffect = AbilityEffect;
                    action.ExecutionEffect = ExecutionEffect;
                    action.ApplyAddStatus();
                }
            }
            PostProcess();

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.AssignEffect, this);
            Target.TriggerActionPoint(ActionPointType.ReceiveEffect, this);
        }
    }
}