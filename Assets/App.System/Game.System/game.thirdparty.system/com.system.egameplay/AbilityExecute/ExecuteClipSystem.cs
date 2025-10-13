using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using ECSGame;

namespace EGamePlay
{
    public interface IOnTriggerClip
    {
        void OnTriggerClip(ExecuteClip entity);
    }

    public interface IOnEndClip
    {
        void OnEndClip(ExecuteClip entity);
    }

    public class ExecuteClipSystem : AEntitySystem<ExecuteClip>,
        IAwake<ExecuteClip>,
        IEnable<ExecuteClip>,
        IDisable<ExecuteClip>,
        IUpdate<ExecuteClip>
    {
        public void Awake(ExecuteClip entity)
        {
            entity.Name = entity.ClipData.GetType().Name;

            var clipType = entity.ClipData.ExecuteClipType;

            /// 时间到触发执行效果
            if (clipType == ExecuteClipType.ActionEvent)
            {
                entity.AddComponent<ExecuteTimeTriggerComponent>(x =>
                {
                    x.StartTime = (float)entity.ClipData.StartTime;
                });
            }
            else if (entity.ClipData.Duration > 0)
            {
                entity.AddComponent<ExecuteTimeTriggerComponent>(x =>
                {
                    x.StartTime = (float)entity.ClipData.StartTime;
                    x.EndTime = (float)entity.ClipData.EndTime;
                });
            }
        }

        public void Enable(ExecuteClip entity)
        {
            
        }

        public void Disable(ExecuteClip entity)
        {

        }

        public void Update(ExecuteClip entity)
        {
            if (entity.GetComponent<ExecuteTimeTriggerComponent>() is { } component)
            {
                ExecuteTimeTriggerSystem.Update(entity, component);
            }
        }

        public static void TriggerClip(ExecuteClip entity)
        {
            var clipType = entity.ClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.ActionEvent)
            {
                var spawnItemEffect = entity.ClipData.ActionEventData;
                if (spawnItemEffect.ActionEventType == FireEventType.AssignEffect)
                {
                    Trigger_AssignEffect(entity);
                }
                if (spawnItemEffect.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    Trigger_NewExecution(entity);
                }
            }
            if (clipType == ExecuteClipType.ItemExecute)
            {
                Trigger_CollisionItem(entity);
            }

            EventSystem.Dispatch<IOnTriggerClip>(entity, x => x.OnTriggerClip(entity));
        }

        public static void EndClip(ExecuteClip entity)
        {
            EventSystem.Dispatch<IOnEndClip>(entity, x => x.OnEndClip(entity));
        }

        private static void Trigger_NewExecution(ExecuteClip entity)
        {
            var executionObject = AssetUtils.LoadObject<ExecutionObject>($"{AbilityManagerObject.ExecutionResFolder}/" + entity.ClipData.ActionEventData.NewExecution);
            if (executionObject == null)
            {
                return;
            }
            var sourceExecution = entity.GetParent<AbilityExecution>();
            var execution = sourceExecution.OwnerEntity.AddChild<AbilityExecution>(x => x.AbilityEntity = sourceExecution.SkillAbility);
            execution.ExecutionObject = executionObject;
            execution.InputTarget = sourceExecution.InputTarget;
            execution.InputPoint = sourceExecution.InputPoint;
            AbilityExecutionSystem.LoadExecutionEffects(execution);
            AbilityExecutionSystem.BeginExecute(execution);
            if (executionObject != null)
            {
                execution.Enable = true;
            }
        }

        /// <summary>   技能碰撞体生成   </summary>
        private static void Trigger_CollisionItem(ExecuteClip entity)
        {
            var execution = entity.GetParent<AbilityExecution>();
            var abilityItem = AbilityItemSystem.Create(entity.EcsNode, execution);
            abilityItem.AddComponent<AbilityItemCollisionExecuteComponent>(x => x.ExecuteClipData = entity.ClipData);
            abilityItem.Init();
        }

        private static void Trigger_AssignEffect(ExecuteClip entity)
        {
            var skillExecution = entity.GetParent<AbilityExecution>();
            var clipType = entity.ClipData.ExecuteClipType;
            var assignEffect = entity.ClipData.ActionEventData;
            if (skillExecution.InputTarget != null)
            {
                var OwnerEntity = skillExecution.OwnerEntity;
                var effects = skillExecution.AbilityEntity.AbilityTriggers;
                for (int i = 0; i < effects.Count; i++)
                {
                    if (i == (int)assignEffect.ExecuteTrigger - 1 || assignEffect.ExecuteTrigger == ExecuteTriggerType.AllTriggers)
                    {
                        var effect = effects[i];
                        var context = new TriggerContext()
                        {
                            AbilityTrigger = effect,
                            TriggerSource = skillExecution,
                            Target = skillExecution.InputTarget,
                        };
                        AbilityTriggerSystem.OnTrigger(effect, context);
                    }
                }
            }
        }
    }
}