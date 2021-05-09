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
    public class CureAction : ActionExecution<CureActionAbility>
    {
        public CureEffect CureEffect { get; set; }
        //治疗数值
        public int CureValue { get; set; }


        //前置处理
        private void PreProcess()
        {
            if (CureEffect != null)
            {
                var expression = ExpressionHelper.TryEvaluate(CureEffect.CureValueProperty);
                CureValue = (int)expression.Value;
            }
        }

        public void ApplyCure()
        {
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