using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using GameUtils;
using System;
#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.float3;
using AO;
using AO.EventType;
using ET.EventType;
#else
using float3 = UnityEngine.Vector3;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : Entity, IPosition
    {
        public Entity AbilityEntity { get; private set; }
        public IAbilityExecute AbilityExecution { get; private set; }
        public EffectApplyType EffectApplyType { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public CombatEntity TargetEntity { get; set; }
        public CombatEntity OwnerEntity => AbilityEntity.As<IAbilityEntity>().OwnerEntity;
#if UNITY
        public AbilityItemViewComponent ItemProxy { get; set; }
#endif


        public override void Awake(object initData)
        {
            AbilityExecution = initData as IAbilityExecute;
            AbilityEntity = AbilityExecution.AbilityEntity;
            if (AbilityEntity == null)
            {
                Log.Error("AbilityItem AbilityEntity == null");
                return;
            }

            AddComponent<ActionPointComponent>();
            AddComponent<AttributeComponent>().InitializeAbilityItem();
            var CurrentHealth = AddComponent<HealthPointComponent>();
            CurrentHealth.HealthPointNumeric = GetComponent<AttributeComponent>().HealthPoint;
            CurrentHealth.HealthPointMaxNumeric = GetComponent<AttributeComponent>().HealthPointMax;
            CurrentHealth.Reset();
            AddComponent<AbilityComponent>();
            AddComponent<StatusComponent>();

            var abilityEffects = AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
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
                        AddComponent<AbilityItemTargetCounterComponent>();
                    }
                }
            }
        }

        /// 结束单元体
        public void DestroyItem()
        {
            //Log.Debug("AbilityItem DestroyItem");
            Destroy(this);
        }

        public override void OnDestroy()
        {
            var clipData = GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            //Log.Debug($"AbilityItem OnDestroy {clipData.ExecuteClipType} {clipData.CollisionExecuteData.ActionData.FireType}");
            if (clipData.ExecuteClipType == ExecuteClipType.ItemExecute && clipData.CollisionExecuteData.ActionData.FireType == FireType.EndTrigger)
            {
                OnTriggerEvent(null);
            }

            if (clipData.CollisionExecuteData.ExecuteType == CollisionExecuteType.InHand)
            {
                AbilityExecution.EndExecute();
            }
        }

        public void OnTriggerEvent(Entity otherEntity)
        {
            if (IsDisposed)
            {
                return;
            }

            if (TargetEntity != null)
            {
                var otherCombatEntity = otherEntity as CombatEntity;
                if (otherCombatEntity != null)
                {
                    if (otherCombatEntity != TargetEntity)
                    {
                        return;
                    }
                }
                else
                {
                    var otherItem = otherEntity as AbilityItem;

                }
            }

            var collisionExecuteData = GetComponent<AbilityItemCollisionExecuteComponent>().CollisionExecuteData;
            if (AbilityEntity != null)
            {
                Log.Debug($"AbilityItem OnTriggerEvent {collisionExecuteData.ActionData.ActionEventType}");
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
                {
                    var effects = AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
                    for (int i = 0; i < effects.Count; i++)
                    {
                        if (i == (int)EffectApplyType - 1 || EffectApplyType == EffectApplyType.AllEffects)
                        {
                            var effect = effects[i];
                            if (effect.TriggerObserver != null)
                            {
                                effect.TriggerObserver.OnTriggerWithAbilityItem(this, otherEntity);
                            }
                        }
                    }
                }
            }

            if (AbilityExecution != null)
            {
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    OnTriggerNewExecution(collisionExecuteData.ActionData);
                }
            }

            if (TryGet(out AbilityItemTargetCounterComponent targetCounterComponent))
            {
                targetCounterComponent.TargetCounter++;
            }

            if (TargetEntity != null)
            {
                DestroyItem();
            }
        }

        public void OnTriggerNewExecution(ActionEventData ActionEventData)
        {
            Log.Debug($"AbilityItem OnTriggerNewExecution");
            var executionObject = AssetUtils.LoadObject<ExecutionObject>("SkillConfigs/ExecutionConfigs/" + ActionEventData.NewExecution);
            if (executionObject == null)
            {
                Log.Error($"Can not find {ActionEventData.NewExecution}");
                return;
            }
            var sourceExecution = AbilityExecution as SkillExecution;
            var execution = sourceExecution.OwnerEntity.AddChild<SkillExecution>(sourceExecution.SkillAbility);
            execution.ExecutionObject = executionObject;
            //execution.InputTarget = sourceExecution.InputTarget;
            execution.InputPoint = Position;
            execution.LoadExecutionEffects();
            execution.BeginExecute();
        }

        /// <summary>   目标飞行碰撞体     </summary>
        public void TargetFlyProcess(CombatEntity InputTarget)
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.TargetEntity = InputTarget;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
#if !EGAMEPLAY_ET
            var moveComp = abilityItem.AddComponent<MoveWithDotweenComponent>();
            moveComp.DoMoveToWithTime(InputTarget, clipData.Duration);
#endif
        }

        /// <summary>   前向飞行碰撞体     </summary>
        public void ForwardFlyProcess(float InputDirection)
        {
            var abilityItem = this;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
            //moveComp.InputPoint = AbilityExecution.OwnerEntity.Position;

            var x = MathF.Sin(MathF.PI / 180 * InputDirection);
            var z = MathF.Cos(MathF.PI / 180 * InputDirection);
            var destination = abilityItem.Position + new Vector3(x, 0, z) * 30;
#if !EGAMEPLAY_ET
            abilityItem.AddComponent<MoveWithDotweenComponent>().DoMoveTo(destination, 1f).OnMoveFinish(() => { Entity.Destroy(abilityItem); });
#endif
        }

        /// <summary>   路径飞行     </summary>
        public void PathFlyProcess(Vector3 InputPoint)
        {
            var skillExecution = (AbilityExecution as SkillExecution);
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            var tempPoints = clipData.CollisionExecuteData.GetCtrlPoints();

            if (tempPoints.Count == 0)
            {
                return;
            }
            abilityItem.LocalPosition = tempPoints[0].Position;
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.InputPoint = InputPoint;
            abilityItem.Position = moveComp.OriginPoint + abilityItem.LocalPosition;

            moveComp.BezierCurve = new NaughtyBezierCurves.BezierCurve3D();
            moveComp.BezierCurve.Sampling = clipData.CollisionExecuteData.BezierCurve.Sampling;
            moveComp.BezierCurve.KeyPoints = tempPoints;
            foreach (var item in tempPoints)
            {
                item.Curve = moveComp.BezierCurve;
            }
            if (skillExecution.ExecutionObject.TargetInputType == ExecutionTargetInputType.Point)
            {
                abilityItem.Position = moveComp.OriginPoint + abilityItem.LocalPosition;
            }
            moveComp.RotateAgree = 0;
            moveComp.Rotate = false;
            moveComp.Duration = clipData.Duration;
            moveComp.GetPathLocalPoints();
            moveComp.DOMove();
        }

        /// <summary>   朝向路径飞行     </summary>
        public void DirectionPathFlyProcess(float InputDirection)
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            var tempPoints = clipData.CollisionExecuteData.GetCtrlPoints();

            //var angle = AbilityExecution.OwnerEntity.Rotation.y - 90;
            //var agree = angle * MathF.PI / 180;
            var agree = (InputDirection) * MathF.PI / 180f;

            if (tempPoints.Count == 0)
            {
                return;
            }
            abilityItem.Position = AbilityExecution.OwnerEntity.Position + (float3)tempPoints[0].Position;
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.OriginEntity = AbilityExecution.OwnerEntity;

            moveComp.BezierCurve = new NaughtyBezierCurves.BezierCurve3D();
            moveComp.BezierCurve.Sampling = clipData.CollisionExecuteData.BezierCurve.Sampling;
            moveComp.BezierCurve.KeyPoints = tempPoints;
            foreach (var item in tempPoints)
            {
                item.Curve = moveComp.BezierCurve;
            }
            moveComp.RotateAgree = agree;
            moveComp.Rotate = true;
            moveComp.Duration = clipData.Duration;
            moveComp.GetPathLocalPoints();
            moveComp.DOMove();
        }

        /// <summary>   固定位置碰撞体     </summary>
        public void FixedPositionProcess()
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.LocalPosition = clipData.CollisionExecuteData.FixedPoint;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position + clipData.CollisionExecuteData.FixedPoint;
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.OriginEntity = AbilityExecution.OwnerEntity;
            abilityItem.AddComponent<AbilityItemFollowComponent>();

            if (clipData.Duration > 0)
            {
                abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            }
        }

        /// <summary>   输入位置碰撞体     </summary>
        public void SelectedPositionProcess(Vector3 InputPoint)
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.Position = InputPoint;
            if (clipData.Duration > 0)
            {
                abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            }
        }

        /// <summary>   输入方向碰撞体     </summary>
        public void SelectedDirectionProcess()
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
            abilityItem.Rotation = AbilityExecution.OwnerEntity.Rotation;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
        }

#if EGAMEPLAY_ET
        /// <summary>   创建技能碰撞体     </summary>
        public ItemUnit AddCollisionComponent()
        {
            var abilityItem = this;
            var scene = AbilityExecution.OwnerEntity.GetComponent<CombatUnitComponent>().Unit.GetParent<Scene>();
            var itemUnit = scene.AddChild<ItemUnit>();
            itemUnit.OwnerUnit = AbilityExecution.OwnerEntity.GetComponent<CombatUnitComponent>().Unit;
            itemUnit.AbilityItem = abilityItem;
            itemUnit.Position = abilityItem.Position;
            itemUnit.Name = (abilityItem.AbilityExecution as SkillExecution).ExecutionObject.Name;
            //ET.Log.Console($"AddCollisionComponent itemUnit={itemUnit.Name}");
            itemUnit.AddComponent<UnitLifeTimeComponent, float>(abilityItem.GetComponent<LifeTimeComponent>().LifeTimer.MaxTime);
            abilityItem.AddComponent<CombatUnitComponent>().Unit = itemUnit;
            if (AbilityEntity != null && AbilityEntity.As<SkillAbility>().SkillConfig != null)
            {
                itemUnit.ConfigId = AbilityEntity.As<SkillAbility>().SkillConfig.Id;
            }
            itemUnit.AddComponent<UnitCollisionComponent>().Radius = 2;
            var moveComp = abilityItem.GetComponent<AbilityItemPathMoveComponent>();
            if (moveComp != null)
            {
                itemUnit.AddComponent<UnitTranslateComponent>();
                itemUnit.AddComponent<UnitPathMoveComponent>();
                var points = moveComp.GetPathPoints();
                itemUnit.GetComponent<UnitPathMoveComponent>().Speed = moveComp.Speed;
                itemUnit.GetComponent<UnitPathMoveComponent>().PathPoints = points.ToList();
                var lifeTime = abilityItem.GetComponent<LifeTimeComponent>().LifeTimer.MaxTime * 1000;
                AOGame.PublishServer(new PublishNewUnitEvent() { Unit = itemUnit.MapUnit() });
#if UNITY
                AOGame.Publish(new CreateUnit() { MapUnit = itemUnit, IsMainAvatar = false });
#endif
                AOGame.Publish(new UnitPathMoveEvent() { Unit = itemUnit.MapUnit(), PathPoints = points, ArriveTime = (long)(TimeHelper.ServerNow() + lifeTime) });
            }
            else
            {
#if UNITY
                AOGame.Publish(new CreateUnit() { MapUnit = itemUnit, IsMainAvatar = false });
#endif
                AOGame.PublishServer(new PublishNewUnitEvent() { Unit = itemUnit.MapUnit() });
            }
            return itemUnit;
        }
#endif

#if UNITY
        /// <summary>   创建技能碰撞体     </summary>
        public GameObject CreateAbilityItemProxyObj()
        {
            var abilityItem = this;
            var proxyObj = new GameObject("AbilityItemProxy");
            proxyObj.transform.position = abilityItem.Position;
            proxyObj.transform.rotation = abilityItem.Rotation;
            abilityItem.AddComponent<AbilityItemViewComponent>().AbilityItem = abilityItem;
            abilityItem.GetComponent<AbilityItemViewComponent>().AbilityItemTrans = proxyObj.transform;
            var executeComp = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>();
            var clipData = executeComp.CollisionExecuteData;
            ItemProxy = abilityItem.GetComponent<AbilityItemViewComponent>();
            CombatContext.Instance.Object2Items[proxyObj.gameObject] = abilityItem;

            if (clipData.Shape == CollisionShape.Sphere)
            {
                proxyObj.AddComponent<SphereCollider>().enabled = false;
                proxyObj.GetComponent<SphereCollider>().isTrigger = true;
                proxyObj.GetComponent<SphereCollider>().radius = (float)clipData.Radius;
            }
            if (clipData.Shape == CollisionShape.Box)
            {
                proxyObj.AddComponent<BoxCollider>().enabled = false;
                proxyObj.GetComponent<BoxCollider>().isTrigger = true;
                proxyObj.GetComponent<BoxCollider>().center = clipData.Center;
                proxyObj.GetComponent<BoxCollider>().size = clipData.Size;
            }

            //Log.Debug($"CreateAbilityItemProxyObj ActionEventType {clipData.ActionData.ActionEventType}");
            if (abilityItem.GetComponent<AbilityItemShieldComponent>() != null)
            {
                var rigid = proxyObj.AddComponent<Rigidbody>();
                rigid.isKinematic = true;
                rigid.useGravity = false;
                abilityItem.AddComponent<AbilityItemShieldViewComponent>();
            }
            else
            {
                proxyObj.AddComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) =>
                {
                    if (abilityItem.IsDisposed)
                    {
                        return;
                    }
                    var owner = abilityItem.AbilityEntity.As<IAbilityEntity>().OwnerEntity;
                    if (owner.CollisionAbility.TryMakeAction(out var collisionAction))
                    {
                        collisionAction.Creator = owner;
                        collisionAction.CauseItem = abilityItem;
                        if (CombatContext.Instance.Object2Entities.TryGetValue(other.gameObject, out var combatEntity))
                        {
                            collisionAction.Target = combatEntity;
                            collisionAction.ApplyCollision();
                        }
                        else
                        {
                            if (CombatContext.Instance.Object2Items.TryGetValue(other.gameObject, out var otherItem))
                            {
                                collisionAction.Target = otherItem;
                                collisionAction.ApplyCollision();
                            }
                        }
                    }
                };
            }

            var collider = proxyObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (clipData.ObjAsset != null)
            {
                abilityItem.Name = clipData.ObjAsset.name;
                var effectObj = GameObject.Instantiate(clipData.ObjAsset, proxyObj.transform);
                effectObj.transform.localPosition = Vector3.zero;
                effectObj.transform.localRotation = UnityEngine.Quaternion.identity;
            }

            return proxyObj;
        }
#endif
    }
}