using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 赋给效果行动
    /// </summary>
    public class AssignEffectAction : CombatAction
    {
        //创建这个赋给效果行动的源能力
        public AbilityEntity SourceAbility { get; set; }
        public Effect Effect { get; set; }
        public StatusAbility Status { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyAssignEffect()
        {
            PreProcess();
            if (Effect is DamageEffect damageEffect)
            {

            }
            if (Effect is AddStatusEffect addStatusEffect)
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
                Status.Caster = Creator;
                Status.Level = SourceAbility.Level;
                Status.AddComponent<StatusLifeTimeComponent>();
                Status.TryActivateAbility();
            }
            PostProcess();

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            if (Effect is AddStatusEffect addStatusEffect)
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