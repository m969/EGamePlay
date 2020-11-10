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


        private void BeforeAssign()
        {

        }

        public void ApplyAssignEffect()
        {
            BeforeAssign();
            if (Effect is DamageEffect damageEffect)
            {

            }
            if (Effect is AddStatusEffect addStatusEffect)
            {
                StatusEntity status = EntityFactory.CreateWithParent<StatusEntity>(Target, addStatusEffect.AddStatus);
                status.Caster = Creator;
                status.Enable();
                status.AddComponent<StatusLifeTimeComponent>();
            }
            AfterAssign();
        }

        private void AfterAssign()
        {

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