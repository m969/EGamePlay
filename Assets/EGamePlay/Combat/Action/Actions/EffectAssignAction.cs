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
        public StatusAbility Status { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyEffectAssign()
        {
            PreProcess();
            if (EffectConfig is DamageEffect damageEffect)
            {
                //if (string.IsNullOrEmpty(damageEffect.DamageValueProperty)) damageEffect.DamageValueProperty = damageEffect.DamageValueFormula;
                if (OwnerEntity.DamageActionAbility.TryCreateAction(out var action))
                {
                    action.Target = Target;
                    action.DamageSource = DamageSource.Skill;
                    action.AbilityEffect = AbilityEffect;
                    action.ApplyDamage();
                }
            }
            if (EffectConfig is CureEffect cureEffect && Target.CurrentHealth.IsFull() == false)
            {
                //if (string.IsNullOrEmpty(cureEffect.CureValueProperty)) cureEffect.CureValueProperty = cureEffect.CureValueFormula;
                if (OwnerEntity.CureActionAbility.TryCreateAction(out var action))
                {
                    action.Target = Target;
                    action.AbilityEffect = AbilityEffect;
                    action.ApplyCure();
                }
            }
            if (EffectConfig is AddStatusEffect addStatusEffect)
            {
                var statusConfig = addStatusEffect.AddStatus;
                if (statusConfig.CanStack == false)
                {
                    if (Target.HasStatus(statusConfig.ID))
                    {
                        var status = Target.GetStatus(statusConfig.ID);
                        var statusLifeTimer = status.GetComponent<StatusLifeTimeComponent>().LifeTimer;
                        statusLifeTimer.MaxTime = addStatusEffect.Duration / 1000f;
                        statusLifeTimer.Reset();
                        return;
                    }
                }

                Status = Target.AttachStatus<StatusAbility>(statusConfig);
                Status.OwnerEntity = Creator;
                Status.Level = SourceAbility.Level;
                //Log.Debug($"ApplyEffectAssign AddStatusEffect {Status}");

                if (statusConfig.EnabledLogicTrigger)
                {
                    Status.ProccessInputKVParams(addStatusEffect.Params);
                }

                Status.AddComponent<StatusLifeTimeComponent>();
                Status.TryActivateAbility();
            }
            PostProcess();

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            if (EffectConfig is AddStatusEffect addStatusEffect)
            {
                Creator.TriggerActionPoint(ActionPointType.PostGiveStatus, this);
                Target.TriggerActionPoint(ActionPointType.PostReceiveStatus, this);
            }
        }
    }

    public enum EffectType
    {
        DamageAffect = 1,
        NumericModify = 2,
        StatusAttach = 3,
        BuffAttach = 4,
    }
}