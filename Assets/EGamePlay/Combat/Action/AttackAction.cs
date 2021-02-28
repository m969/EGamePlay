using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 普攻行动
    /// </summary>
    public class AttackAction : CombatAction
    {
        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyAttack()
        {
            PreProcess();
            //Hero.Instance.StopMove();

            var attackExecution = Creator.AttackAbility.CreateExecution() as AttackExecution;
            attackExecution.AttackAction = this;
            attackExecution.BeginExecute();

            PostProcess();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveAttack, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveAttack, this);
        }
    }
}