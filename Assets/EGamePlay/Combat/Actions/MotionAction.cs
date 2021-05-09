using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    public class MotionActionAbility : ActionAbility<MotionAction>
    {

    }

    /// <summary>
    /// 动作行动
    /// </summary>
    public class MotionAction : ActionExecution<MotionActionAbility>
    {
        public int MotionType { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyMotion()
        {
            PreProcess();

            PostProcess();
        }

        //后置处理
        private void PostProcess()
        {
            //Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            //Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}