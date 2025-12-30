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
            EcsObject.Destroy(entity);
        }

        public static void ApplyCollision(CollisionAction entity)
        {
            ActionSystem.ExecuteAction(entity, _ =>
            {
                return true;
            }, _ =>
            {
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
                            if (entity.CauseItem.OwnerEntity.IsHero != abilityItem.OwnerEntity.IsHero)
                            {
                                AbilityItemSystem.OnTriggerEvent(entity.CauseItem, entity.Target);

                                if (causeCollisionComp.CollisionExecuteData.ActionData.FireType == FireType.CollisionTrigger)
                                {
                                    HealthSystem.SetDie(entity.CauseItem);
                                }
                            }
                        }
                    }
                }
                return true;
            });

            FinishAction(entity);
        }
    }
}