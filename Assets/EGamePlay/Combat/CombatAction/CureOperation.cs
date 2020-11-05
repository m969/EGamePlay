using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 治疗行动
    /// </summary>
    public class CureOperation : CombatOperation
    {
        //治疗数值
        public int CureValue { get; set; }


        private void BeforeCure()
        {

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