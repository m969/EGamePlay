using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 治疗行为
    /// </summary>
    public class CureAction : CombatAction
    {
        //治疗数值
        public int CureValue { get; set; }


        private void BeforeCure()
        {

        }

        public void ApplyCure()
        {
            BeforeCure();

            AfterCure();
        }

        private void AfterCure()
        {

        }
    }
}