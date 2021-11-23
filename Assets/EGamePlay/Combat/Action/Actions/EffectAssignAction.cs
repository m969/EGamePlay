using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    public class EffectAssignAbility : ActionAbility<EffectAssignAction>
    {

    }

    /// <summary>
    /// 赋给效果行动
    /// </summary>
    public class EffectAssignAction : ActionExecution
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
            //Log.Debug($"ApplyEffectAssign {EffectConfig}");
            PreProcess();

            if (EffectConfig is DamageEffect)
            {
                if (OwnerEntity.DamageAbility.TryMakeAction(out var damageAction))
                {
                    damageAction.Target = Target;
                    damageAction.AbilityEffect = AbilityEffect;
                    damageAction.ExecutionEffect = ExecutionEffect;
                    damageAction.DamageSource = DamageSource.Skill;
                    damageAction.ApplyDamage();
                }
            }

            if (EffectConfig is CureEffect && Target.CurrentHealth.IsFull() == false)
            {
                if (OwnerEntity.CureAbility.TryMakeAction(out var cureAction))
                {
                    cureAction.Target = Target;
                    cureAction.AbilityEffect = AbilityEffect;
                    cureAction.ExecutionEffect = ExecutionEffect;
                    cureAction.ApplyCure();
                }
            }

            if (EffectConfig is AddStatusEffect)
            {
                if (OwnerEntity.AddStatusAbility.TryMakeAction(out var addStatusAction))
                {
                    addStatusAction.SourceAbility = SourceAbility;
                    addStatusAction.Target = Target;
                    addStatusAction.AbilityEffect = AbilityEffect;
                    addStatusAction.ExecutionEffect = ExecutionEffect;
                    addStatusAction.ApplyAddStatus();
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