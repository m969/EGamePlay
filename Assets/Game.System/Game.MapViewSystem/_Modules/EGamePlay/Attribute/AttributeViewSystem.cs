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
    public class AttributeViewSystem : AComponentSystem<CombatEntity, AttributeComponent>,
        IAwake<CombatEntity, AttributeComponent>,
        IOnAttributeUpdate
    {
        public void Awake(CombatEntity entity, AttributeComponent component)
        {
        }

        public void OnAttributeUpdate(EcsEntity entity, FloatNumeric numeric)
        {
            //ConsoleLog.Debug("OnAttributeUpdate " + numeric.AttributeType);
        }
    }
}