using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class ExecuteTimeTriggerSystem : AComponentSystem<ExecuteClip, ExecuteTimeTriggerComponent>,
        IAwake<ExecuteClip, ExecuteTimeTriggerComponent>,
        IEnable<ExecuteClip, ExecuteTimeTriggerComponent>
    {
        public void Awake(ExecuteClip entity, ExecuteTimeTriggerComponent component)
        {
        }

        public void Enable(ExecuteClip entity, ExecuteTimeTriggerComponent component)
        {
            if (!string.IsNullOrEmpty(component.TimeValueExpression))
            {
                var expression = ExpressionHelper.TryEvaluate(component.TimeValueExpression);
                component.StartTime = (int)expression.Value / 1000f;
                component.StartTimer = new GameTimer(component.StartTime);
            }
            else if (component.StartTime > 0)
            {
                component.StartTimer = new GameTimer(component.StartTime);
            }
            else
            {
                ExecuteClipSystem.TriggerClip(entity);
            }

            if (component.EndTime > 0)
            {
                component.EndTimer = new GameTimer(component.EndTime);
            }
        }

        public static void Update(ExecuteClip entity, ExecuteTimeTriggerComponent component)
        {
            if (component.StartTimer != null && !component.StartTimer.IsFinished)
            {
                component.StartTimer.UpdateAsFinish(Time.deltaTime, () => ExecuteClipSystem.TriggerClip(entity));
            }
            if (component.EndTimer != null && !component.EndTimer.IsFinished)
            {
                component.EndTimer.UpdateAsFinish(Time.deltaTime, () => ExecuteClipSystem.EndClip(entity));
            }
        }
    }
}