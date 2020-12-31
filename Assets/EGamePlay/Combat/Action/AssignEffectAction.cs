using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat.Status;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 赋给效果行动
    /// </summary>
    public class AssignEffectAction : CombatAction
    {
        public Effect Effect { get; set; }
        //效果类型
        public EffectType EffectType { get; set; }
        //效果数值
        public string EffectValue { get; set; }
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
                Status = Target.AttachStatus<StatusAbility>(addStatusEffect.AddStatus);
                Status.Caster = Creator;
                Status.AddComponent<StatusLifeTimeComponent>();
                Status.TryActivateAbility();
            }
            PostProcess();
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