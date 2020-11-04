using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 赋给效果行动
    /// </summary>
    public class AssignEffectOperation : CombatOperation
    {
        //效果类型
        public EffectType EffectType { get; set; }
        //效果数值
        public string EffectValue { get; set; }


        private void BeforeAssign()
        {

        }

        public void ApplyAssignOperation()
        {
            BeforeAssign();

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