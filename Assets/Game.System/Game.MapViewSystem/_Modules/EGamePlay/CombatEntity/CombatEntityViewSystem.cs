using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System.Net;
using System;
using UnityEngine.UIElements;
using ECSGame;
using ECSUnity;
using ET;

namespace EGamePlay
{
    public class CombatEntityViewSystem : AEntitySystem<CombatEntity>,
    IAwake<CombatEntity>,
    IInit<CombatEntity>,
    IUpdate<CombatEntity>,
    IBeforeExecuteAction,
    IAfterCauseApply,
    IAfterReceiveApply
    {
        public void Awake(CombatEntity entity)
        {
        }

        public void Init(CombatEntity entity)
        {
            entity.AddComponent<ModelViewComponent>();
            entity.AddComponent<ECSGame.AnimationComponent>();
        }

        public void BeforeExecuteAction(CombatEntity entity, EcsEntity combatAction)
        {
            //ConsoleLog.Debug("BeforeExecuteAction " + combatAction.GetType().Name);
            if (combatAction is SpellAction spellAction)
            {
                if (entity.IsHero)
                {
                    if (spellAction.InputPoint != TransformSystem.GetPosition(entity.Actor))
                    {
                        TransformSystem.ChangeForward(entity.Actor, spellAction.InputPoint - TransformSystem.GetPosition(entity.Actor));
                    }
                    PlayerInputSystem.DisableMove(StaticClient.Game);
                }
            }
        }

        public void AfterCauseApply(CombatEntity entity, EcsEntity combatAction)
        {
            //ConsoleLog.Debug("AfterCauseApply " + combatAction.GetType().Name);
        }

        public void AfterReceiveApply(EcsEntity entity, EcsEntity combatAction)
        {
            //ConsoleLog.Debug("AfterReceiveApply " + combatAction.GetType().Name);
            if (combatAction is DamageAction)
            {
                HealthViewSystem.OnReceiveDamage(entity, combatAction);
            }
            if (combatAction is CureAction)
            {
                HealthViewSystem.OnReceiveCure(entity, combatAction);
            }
        }

        public void Update(CombatEntity entity)
        {
            EntityViewSystem.Update(entity);
        }
    }

    public class CombatEntityModelViewSystem : AComponentSystem<CombatEntity, ModelViewComponent>,
        IAwake<CombatEntity, ModelViewComponent>
    {
        public void Awake(CombatEntity entity, ModelViewComponent component)
        {

        }
    }
}