using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat.Skill;
using ET;

namespace EGamePlay.Combat
{
    public class AttackActionAbility : ActionAbility<AttackAction>
    {

    }

    /// <summary>
    /// 普攻行动
    /// </summary>
    public class AttackAction : ActionExecution<AttackActionAbility>
    {
        //前置处理
        private void PreProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PreGiveAttack, this);
            Target.TriggerActionPoint(ActionPointType.PreReceiveAttack, this);
        }

        public async ETTask ApplyAttackAwait()
        {
            PreProcess();
            
            await TimeHelper.WaitAsync(1000);

            ApplyAttack();

            await TimeHelper.WaitAsync(300);

            PostProcess();

            ApplyAction();
        }

        public void ApplyAttack()
        {
            var attackExecution = Creator.AttackAbility.CreateExecution();
            attackExecution.AttackAction = this;
            attackExecution.BeginExecute();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveAttack, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveAttack, this);
        }
    }
}