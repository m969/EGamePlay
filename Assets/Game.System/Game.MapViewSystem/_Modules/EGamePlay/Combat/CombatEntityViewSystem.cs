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
    public class CombatEntityViewControlSystem : AEntitySystem<CombatEntity>,
    IAwake<CombatEntity>,
    IInit<CombatEntity>,
    IUpdate<CombatEntity>
    {
        public void Awake(CombatEntity entity)
        {
        }

        public void Init(CombatEntity entity)
        {
            entity.AddComponent<ModelViewComponent>();
            entity.AddComponent<ECSGame.AnimationComponent>();
        }

        public void Update(CombatEntity entity)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } component)
            {
                ModelViewSystem.Update(entity, component);
            }
        }
    }

    public class CombatEntityViewSystem : AComponentSystem<CombatEntity, ModelViewComponent>,
        IAwake<CombatEntity, ModelViewComponent>
    {
        public void Awake(CombatEntity entity, ModelViewComponent component)
        {

        }
    }
}