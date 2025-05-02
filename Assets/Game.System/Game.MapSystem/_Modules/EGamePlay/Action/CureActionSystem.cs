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
            EcsObject.Destroy(entity);
        }

        //«∞÷√¥¶¿Ì
        private static bool ActionCheckProcess(CureAction entity)
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
            if (HealthSystem.IsFull(entity.Target as CombatEntity))
            {
                return false;
            }
            return true;
        }

        public static void Execute(CureAction entity)
        {
            ActionSystem.ExecuteAction(entity, _ =>
            {
                return ActionCheckProcess(entity);
            }, _ =>
            {
                HealthSystem.AddHealth(entity.Target as CombatEntity, entity);
                return true;
            });

            FinishAction(entity);
        }
    }
}