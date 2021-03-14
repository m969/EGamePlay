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


        //前置处理
        private void PreProcess()
        {
            if (CureEffect != null)
            {
                CureValue = int.Parse(CureEffect.CureValueFormula);
            }
        }

        public void ApplyCure()
        {
            PreProcess();
            Target.ReceiveCure(this);
            PostProcess();

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}