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
#if UNITY
            /// 播放特效效果
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                var animationEffect = entity.ClipData.ParticleEffectData;
                entity.AddComponent<ExecuteParticleEffectComponent>().ParticleEffectPrefab = animationEffect.ParticleEffect;
            }
#endif

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
            var clipType = entity.ClipData.ExecuteClipType;
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

        public static void BeginExecute(ExecuteClip entity)
        {
            //if (entity.GetComponent<ExecuteTimeTriggerComponent>() == null)
            //{
            //    TriggerEffect(entity);
            //}
            //foreach (var item in entity.Components.Values)
            //{
            //    item.Enable = true;
            //}
        }

        public static void TriggerEffect(ExecuteClip entity)
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
#if UNITY
            if (clipType == ExecuteClipType.Animation)
            {
                Trigger_Animation(entity);
            }
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                Trigger_ParticleEffect(entity);
            }
#endif
        }

        public static void EndEffect(ExecuteClip entity)
        {
            var clipType = entity.ClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                TriggerEnd_ParticleEffect(entity);
            }
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

        private static void Trigger_ParticleEffect(ExecuteClip entity)
        {
//            var component = entity.GetComponent<ExecuteParticleEffectComponent>();
//#if EGAMEPLAY_ET
//            ParticleEffectObj = GameObject.Instantiate(ParticleEffectPrefab, Entity.GetParent<SkillExecution>().OwnerEntity.Position, Entity.GetParent<SkillExecution>().OwnerEntity.Rotation);
//#else
//            component.ParticleEffectObj = GameObject.Instantiate(component.ParticleEffectPrefab, entity.GetParent<AbilityExecution>().OwnerEntity.Position, entity.GetParent<AbilityExecution>().OwnerEntity.Rotation);
//#endif
        }

        private static void TriggerEnd_ParticleEffect(ExecuteClip entity)
        {
            //var component = entity.GetComponent<ExecuteParticleEffectComponent>();
            //GameObject.Destroy(component.ParticleEffectObj);
        }

        private static void Trigger_Animation(ExecuteClip entity)
        {
            //entity.GetParent<AbilityExecution>().OwnerEntity.Publish(entity.ClipData.AnimationData.AnimationClip);
        }

        /// <summary>   技能碰撞体生成   </summary>
        private static void Trigger_CollisionItem(ExecuteClip entity)
        {
            var execution = entity.GetParent<AbilityExecution>();
            var abilityItem = AbilityItemSystem.Create(entity.EcsNode, execution);
            abilityItem.AddComponent<AbilityItemCollisionExecuteComponent>(x => x.ExecuteClipData = entity.ClipData);
            abilityItem.Init();
            var moveType = entity.ClipData.ItemData.MoveType;
            //if (moveType == CollisionMoveType.PathFly) abilityItem.PathFlyProcess(skillExecution.InputPoint);
            //if (moveType == CollisionMoveType.SelectedDirectionPathFly) abilityItem.DirectionPathFlyProcess(skillExecution.InputPoint, skillExecution.InputRadian);
            if (moveType == CollisionMoveType.TargetFly) AbilityItemSystem.TargetFlyProcess(abilityItem, execution.InputTarget);
            //if (moveType == CollisionMoveType.ForwardFly) abilityItem.ForwardFlyProcess(skillExecution.InputRadian);
            //if (moveType == CollisionMoveType.SelectedPosition) abilityItem.SelectedPositionProcess(skillExecution.InputPoint);
            //if (moveType == CollisionMoveType.FixedPosition) abilityItem.FixedPositionProcess();
            //if (moveType == CollisionMoveType.SelectedDirection) abilityItem.SelectedDirectionProcess();

            //EventSystem.Dispatch(new EntityCreateCmd()
            //{
            //    Entity = abilityItem,
            //});

//#if EGAMEPLAY_ET
//            AbilityItemSystem.AddCollisionComponent(abilityItem);
//#else
//#if UNITY
//            AbilityItemSystem.CreateAbilityItemProxyObj(abilityItem);
//#endif
//#endif
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