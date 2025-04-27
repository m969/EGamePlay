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
    public class CollisionActionSystem : AEntitySystem<CollisionAction>,
        IAwake<CollisionAction>
    {
        public void Awake(CollisionAction entity)
        {

        }

        public static void FinishAction(CollisionAction entity)
        {
            EcsEntity.Destroy(entity);
        }

        //前置处理
        private static void ActionProcess(CollisionAction entity)
        {

        }

        public static void ApplyCollision(CollisionAction entity)
        {
            ConsoleLog.Debug($"CollisionActionSystem ApplyCollision");
            ActionProcess(entity);

            if (entity.Target != null)
            {
                if (entity.Target is CombatEntity combatEntity)
                {
                    AbilityItemSystem.OnTriggerEvent(entity.CauseItem, combatEntity);
                }
                if (entity.Target is AbilityItem abilityItem)
                {
                    var collisionComp = entity.Target.GetComponent<AbilityItemCollisionExecuteComponent>();
                    var causeCollisionComp = entity.CauseItem.GetComponent<AbilityItemCollisionExecuteComponent>();
                    var actionEvent = collisionComp.CollisionExecuteData.ActionData.ActionEventType;
                    if (entity.Target.GetComponent<AbilityItemShieldComponent>() != null)
                    {
#if !EGAMEPLAY_ET
                        if (entity.CauseItem.OwnerEntity.IsHero != abilityItem.OwnerEntity.IsHero)
#endif
                        {
                            AbilityItemSystem.OnTriggerEvent(entity.CauseItem, entity.Target);

                            if (causeCollisionComp.CollisionExecuteData.ActionData.FireType == FireType.CollisionTrigger)
                            {
#if EGAMEPLAY_ET
                                var itemUnit = CauseItem.GetComponent<CombatUnitComponent>().Unit as ItemUnit;
                                itemUnit.DestroyType = UnitDestroyType.DestroyWithExplosion;
#endif
                                HealthPointSystem.SetDie(entity.CauseItem);
                            }
                        }
                    }
                }
            }

            AfterActionProcess(entity);

            FinishAction(entity);
        }

        //后置处理
        private static void AfterActionProcess(CollisionAction entity)
        {
            //BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.PostExecuteCure, entity);
            //BehaviourPointSystem.TriggerActionPoint(entity.Target as CombatEntity, ActionPointType.PostSufferCure, entity);
        }
    }
}