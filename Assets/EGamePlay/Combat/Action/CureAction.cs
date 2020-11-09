using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 治疗行动
    /// </summary>
    public class CureAction : CombatAction
    {
        public CureEffect CureEffect { get; set; }
        //治疗数值
        public int CureValue { get; set; }


        private void BeforeCure()
        {
            CureValue = int.Parse(CureEffect.CureValueFormula);
        }

        public void ApplyCure()
        {
            BeforeCure();
            Target.ReceiveCure(this);
            AfterCure();
        }

        private void AfterCure()
        {

        }
    }
}