using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    /// <summary>
    /// 生命周期组件
    /// </summary>
    public class LifeTimeSystem : AComponentSystem<EcsEntity, LifeTimeComponent>,
        IAwake<EcsEntity, LifeTimeComponent>
    {
        public void Awake(EcsEntity entity, LifeTimeComponent component)
        {
            component.LifeTimer = new GameTimer((float)component.Duration);
        }

        public static void Update(EcsEntity entity, LifeTimeComponent component)
        {
            if (component.LifeTimer.IsRunning)
            {
                component.LifeTimer.UpdateAsFinish(Time.deltaTime);
                if (component.LifeTimer.IsFinished)
                {
                    EcsObject.Destroy(component.Entity);
                }
            }
        }
    }
}