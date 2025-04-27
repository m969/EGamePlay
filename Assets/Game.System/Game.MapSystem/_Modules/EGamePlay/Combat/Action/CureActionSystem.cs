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

namespace EGamePlay
{
    public class CureActionSystem : AEntitySystem<CureAction>,
        IAwake<CureAction>
    {
        public void Awake(CureAction entity)
        {

        }

        public static void FinishAction(CureAction entity)
        {
            EcsEntity.Destroy(entity);
        }

        //前置处理
        private static void ActionProcess(CureAction entity)
        {
            if (entity.SourceAssignAction != null && entity.SourceAssignAction.AbilityEffect != null)
            {
                entity.CureValue = EffectCureSystem.GetCureValue(entity.SourceAssignAction.AbilityEffect);
                var healthComp = entity.Target.GetComponent<HealthPointComponent>();
                if (entity.CureValue + healthComp.Value > healthComp.MaxValue)
                {
                    entity.CureValue = healthComp.MaxValue - healthComp.Value;
                }
            }
        }

        public static void Execute(CureAction entity)
        {
            ActionProcess(entity);

            if (HealthSystem.IsFull(entity.Target as CombatEntity) == false)
            {
                HealthSystem.ReceiveCure(entity.Target as CombatEntity, entity);
            }

            AfterActionProcess(entity);

            FinishAction(entity);
        }

        //后置处理
        private static void AfterActionProcess(CureAction entity)
        {
            BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.PostExecuteCure, entity);
            BehaviourPointSystem.TriggerActionPoint(entity.Target as CombatEntity, ActionPointType.PostSufferCure, entity);
        }
    }
}