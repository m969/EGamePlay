using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    public class CureActionAbility : ActionAbility<CureAction>
    {

    }

    /// <summary>
    /// 治疗行动
    /// </summary>
    public class CureAction : ActionExecution
    {
        public CureEffect CureEffect => AbilityEffect.EffectConfig as CureEffect;
        //治疗数值
        public int CureValue { get; set; }


        //前置处理
        private void PreProcess()
        {
            if (AbilityEffect != null)
            {
                CureValue = AbilityEffect.GetComponent<EffectCureComponent>().GetCureValue();
            }
        }

        public void ApplyCure()
        {
            //Log.Debug("CureAction ApplyCure");
            PreProcess();

            if (Target.CurrentHealth.IsFull() == false)
            {
                Target.ReceiveCure(this);
            }

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