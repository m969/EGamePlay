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

namespace EGamePlay
{
    public class AbilityItemSystem : AEntitySystem<AbilityItem>,
        IAwake<AbilityItem>,
        IInit<AbilityItem>,
        IUpdate<AbilityItem>,
        IDestroy<AbilityItem>
    {
        public void Awake(AbilityItem entity)
        {
            entity.AbilityEntity = entity.AbilityExecution.AbilityEntity;
            if (entity.AbilityEntity == null)
            {
                Log.Error("AbilityItem AbilityEntity == null");
                return;
            }

            var abilityEffects = entity.AbilityEntity.AbilityEffects;
            foreach (var abilityEffect in abilityEffects)
            {
                if (abilityEffect.EffectConfig.Decorators == null)
                {
                    continue;
                }
                foreach (var effectDecorator in abilityEffect.EffectConfig.Decorators)
                {
                    if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                    {
                        entity.AddComponent<AbilityItemTargetCounterComponent>();
                    }
                }
            }
        }

        public void Init(AbilityItem entity)
        {
            AttributeSystem.InitializeAbilityItem(entity);
            HealthSystem.Reset(entity);

            var execution = entity.AbilityExecution;
            var clipData = entity.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            var moveType = clipData.ItemData.MoveType;
            if (moveType == CollisionMoveType.PathFly) PathFlyProcess(entity, execution.InputPoint);
            if (moveType == CollisionMoveType.SelectedDirectionPathFly) DirectionPathFlyProcess(entity, execution.InputPoint, execution.InputRadian);
            if (moveType == CollisionMoveType.TargetFly) TargetFlyProcess(entity, execution.InputTarget);
            if (moveType == CollisionMoveType.ForwardFly) ForwardFlyProcess(entity, execution.InputRadian);
            if (moveType == CollisionMoveType.SelectedPosition) SelectedPositionProcess(entity, execution.InputPoint);
            if (moveType == CollisionMoveType.FixedPosition) FixedPositionProcess(entity);
            if (moveType == CollisionMoveType.SelectedDirection) SelectedDirectionProcess(entity);
        }

        public void Update(AbilityItem entity)
        {
            if (entity.GetComponent<LifeTimeComponent>() is { } component)
            {
                LifeTimeSystem.Update(entity, component);
            }
            if (entity.GetComponent<AbilityItemFollowComponent>() is { } component2)
            {
                AbilityItemFollowSystem.Update(entity, component2);
            }
            if (entity.GetComponent<AbilityItemMoveComponent>() is { } component3)
            {
                AbilityItemMoveSystem.Update(entity, component3);
            }
        }

        public void Destroy(AbilityItem entity)
        {
            if (entity.GetComponent<AbilityItemCollisionExecuteComponent>()  is { } component)
            {
                var clipData = component.ExecuteClipData;
                var actionEvenData = component.CollisionExecuteData.ActionData;
                if (clipData.ExecuteClipType == ExecuteClipType.ItemExecute && actionEvenData.FireType == FireType.EndTrigger)
                {
                    OnTriggerEvent(entity, null);
                }

                if (clipData.ItemData.ExecuteType == CollisionExecuteType.InHand)
                {
                    AbilityExecutionSystem.EndExecute(entity.AbilityExecution);
                }
            }
        }

        public static AbilityItem Create(EcsNode parent, AbilityExecution execution)
        {
            var abilityItem = parent.AddChild<AbilityItem>(beforeAwake: x => x.AbilityExecution = execution);
            abilityItem.AddComponent<TransformComponent>();
            //abilityItem.AddComponent<BehaviourPointComponent>();
            abilityItem.AddComponent<AttributeComponent>();
            abilityItem.AddComponent<HealthPointComponent>();
            //abilityItem.AddComponent<AbilityComponent>();
            //abilityItem.AddComponent<BuffComponent>();
            abilityItem.AddComponent<CollisionComponent>();
            return abilityItem;
        }

        public static void OnTriggerEvent(AbilityItem abilityItem, EcsEntity otherEntity)
        {
            if (abilityItem.IsDisposed)
            {
                return;
            }

            if (abilityItem.TargetEntity != null)
            {
                var otherCombatEntity = otherEntity as CombatEntity;
                if (otherCombatEntity != null)
                {
                    if (otherCombatEntity != abilityItem.TargetEntity)
                    {
                        return;
                    }
                }
                else
                {
                    var otherItem = otherEntity as AbilityItem;
                }
            }

            var collisionExecuteData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().CollisionExecuteData;
            if (abilityItem.AbilityEntity != null)
            {
                //Log.Debug($"AbilityItem OnTriggerEvent {collisionExecuteData.ActionData.ActionEventType}");
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
                {
                    var effects = abilityItem.AbilityEntity.AbilityTriggers;
                    for (int i = 0; i < effects.Count; i++)
                    {
                        if (i == (int)abilityItem.ExecuteTriggerType - 1 || abilityItem.ExecuteTriggerType == ExecuteTriggerType.AllTriggers)
                        {
                            var effect = effects[i];
                            var context = new TriggerContext()
                            {
                                AbilityTrigger = effect,
                                TriggerSource = abilityItem,
                                AbilityItem = abilityItem,
                                Target = otherEntity,
                            };
                            AbilityTriggerSystem.OnTrigger(effect, context);
                        }
                    }
                }
            }

            if (abilityItem.AbilityExecution != null)
            {
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    OnTriggerNewExecution(abilityItem, collisionExecuteData.ActionData);
                }
            }

            if (abilityItem.GetComponent<AbilityItemTargetCounterComponent>() is { } targetCounterComponent)
            {
                targetCounterComponent.TargetCounter++;
            }

            if (abilityItem.TargetEntity != null)
            {
                EcsObject.Destroy(abilityItem);
            }
        }

        public static void OnTriggerNewExecution(AbilityItem abilityItem, ActionEventData actionEventData)
        {
            Log.Debug($"AbilityItem OnTriggerNewExecution");
            var executionObject = AssetUtils.LoadObject<ExecutionObject>($"{AbilityManagerObject.ExecutionResFolder}/" + actionEventData.NewExecution);
            if (executionObject == null)
            {
                Log.Error($"Can not find {actionEventData.NewExecution}");
                return;
            }
            var sourceExecution = abilityItem.AbilityExecution;
            var execution = sourceExecution.OwnerEntity.AddChild<AbilityExecution>(x => x.AbilityEntity = sourceExecution.SkillAbility);
            execution.ExecutionObject = executionObject;
            execution.InputPoint = abilityItem.Position;
            AbilityExecutionSystem.LoadExecutionEffects(execution);
            AbilityExecutionSystem.BeginExecute(execution);
        }

        /// <summary>   目标飞行碰撞体     </summary>
        public static void TargetFlyProcess(AbilityItem abilityItem, CombatEntity inputTarget)
        {
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.TargetEntity = inputTarget;
            abilityItem.Position = abilityItem.AbilityExecution.OwnerEntity.Position;
            abilityItem.AddComponent<AbilityItemMoveComponent>();
            AbilityItemMoveSystem.MoveToTarget(abilityItem, inputTarget, 0.02f);
        }

        /// <summary>   前向飞行碰撞体     </summary>
        public static void ForwardFlyProcess(AbilityItem abilityItem, float inputRadian)
        {
            abilityItem.Position = abilityItem.AbilityExecution.OwnerEntity.Position;

            var z = MathF.Sin(inputRadian);
            var x = MathF.Cos(inputRadian);
            var destination = abilityItem.Position + new Vector3(x, 0, z) * 30;
            abilityItem.AddComponent<AbilityItemMoveComponent>();
            AbilityItemMoveSystem.MoveToPoint(abilityItem, destination, 0.02f);
        }

        /// <summary>   路径飞行     </summary>
        public static void PathFlyProcess(AbilityItem abilityItem, Vector3 inputPoint)
        {
            var execution = abilityItem.AbilityExecution;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(beforeAwake: x => x.Duration = clipData.Duration);
            var tempPoints = clipData.ItemData.GetCtrlPoints();

            if (tempPoints.Count == 0)
            {
                return;
            }
            abilityItem.LocalPosition = tempPoints[0].Position;
            var moveComp = abilityItem.AddComponent<AbilityItemMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.ExecutePoint = inputPoint;
            abilityItem.Position = moveComp.OriginPoint + abilityItem.LocalPosition;

            moveComp.BezierCurve = new NaughtyBezierCurves.BezierCurve3D();
            moveComp.BezierCurve.Sampling = clipData.ItemData.BezierCurve.Sampling;
            moveComp.BezierCurve.KeyPoints = tempPoints;
            foreach (var item in tempPoints)
            {
                item.Curve = moveComp.BezierCurve;
            }
            if (execution.ExecutionObject.TargetInputType == ExecutionTargetInputType.Point)
            {
                abilityItem.Position = moveComp.OriginPoint + abilityItem.LocalPosition;
            }
            moveComp.RotateRadian = 0;
            moveComp.Rotate = false;
            moveComp.Duration = clipData.Duration;
            AbilityItemMoveSystem.GetPathLocalPoints(abilityItem);
            AbilityItemMoveSystem.DOMove(abilityItem);
        }

        /// <summary>   朝向路径飞行     </summary>
        public static void DirectionPathFlyProcess(AbilityItem abilityItem, Vector3 inputPoint, float inputRadian)
        {
            var execution = abilityItem.AbilityExecution;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(beforeAwake: x => x.Duration = clipData.Duration);
            var tempPoints = clipData.ItemData.GetCtrlPoints();

            if (tempPoints.Count == 0)
            {
                return;
            }
            var moveComp = abilityItem.AddComponent<AbilityItemMoveComponent>();
            var executePoint = abilityItem.AbilityExecution.Position;
            if (clipData.ItemData.PathExecutePoint == PathExecutePoint.EntityOffset)
            {
                executePoint = abilityItem.AbilityExecution.Position + clipData.ItemData.Offset;
            }
            if (clipData.ItemData.PathExecutePoint == PathExecutePoint.InputPoint)
            {
                executePoint = inputPoint + clipData.ItemData.Offset;
            }
            abilityItem.Position = executePoint + tempPoints[0].LocalPosition;
            abilityItem.Rotation = execution.InputDirection.GetRotation();
            moveComp.PositionEntity = abilityItem;
            moveComp.ExecutePoint = executePoint;

            moveComp.BezierCurve = new NaughtyBezierCurves.BezierCurve3D();
            moveComp.BezierCurve.Sampling = clipData.ItemData.BezierCurve.Sampling;
            moveComp.BezierCurve.KeyPoints = tempPoints;
            foreach (var item in tempPoints)
            {
                item.Curve = moveComp.BezierCurve;
            }
            moveComp.RotateRadian = inputRadian;
            moveComp.Rotate = true;
            moveComp.Duration = clipData.Duration;
            AbilityItemMoveSystem.GetPathLocalPoints(abilityItem);
            AbilityItemMoveSystem.DOMove(abilityItem);
        }

        /// <summary>   固定位置碰撞体     </summary>
        public static void FixedPositionProcess(AbilityItem abilityItem)
        {
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.LocalPosition = clipData.ItemData.FixedPoint;
            abilityItem.Position = abilityItem.AbilityExecution.OwnerEntity.Position + clipData.ItemData.FixedPoint;
            var moveComp = abilityItem.AddComponent<AbilityItemMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.OriginEntity = abilityItem.AbilityExecution.OwnerEntity;
            abilityItem.AddComponent<AbilityItemFollowComponent>();

            if (clipData.Duration > 0)
            {
                abilityItem.AddComponent<LifeTimeComponent>(beforeAwake: x => x.Duration = clipData.Duration);
            }
        }

        /// <summary>   输入位置碰撞体     </summary>
        public static void SelectedPositionProcess(AbilityItem abilityItem, Vector3 InputPoint)
        {
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.Position = InputPoint;
            if (clipData.Duration > 0)
            {
                abilityItem.AddComponent<LifeTimeComponent>(beforeAwake: x => x.Duration = clipData.Duration);
            }
        }

        /// <summary>   输入方向碰撞体     </summary>
        public static void SelectedDirectionProcess(AbilityItem abilityItem)
        {
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.Position = abilityItem.AbilityExecution.OwnerEntity.Position;
            abilityItem.Rotation = abilityItem.AbilityExecution.OwnerEntity.Rotation;
            abilityItem.AddComponent<LifeTimeComponent>(beforeAwake: x => x.Duration = clipData.Duration);
        }
    }
}