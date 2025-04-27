using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;
using static UnityEngine.GraphicsBuffer;

namespace EGamePlay
{
    public class AttackActionSystem : AEntitySystem<AttackAction>,
        IAwake<AttackAction>
    {
        public void Awake(AttackAction entity)
        {

        }

        public static void FinishAction(AttackAction entity)
        {
            EcsEntity.Destroy(entity);
        }

        //前置处理
        private static void PreProcess(AttackAction entity)
        {
            BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.PreExecuteAttack, entity);
            BehaviourPointSystem.TriggerActionPoint(entity.Target as CombatEntity, ActionPointType.PreSufferAttack, entity);
        }

        public static void ApplyAttackAwait(AttackAction entity)
        {
            PreProcess(entity);

            PostProcess(entity);

            FinishAction(entity);
        }

        //后置处理
        private static void PostProcess(AttackAction entity)
        {
            BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.PostExecuteAttack, entity);
            BehaviourPointSystem.TriggerActionPoint(entity.Target as CombatEntity, ActionPointType.PostSufferAttack, entity);
        }
    }
}