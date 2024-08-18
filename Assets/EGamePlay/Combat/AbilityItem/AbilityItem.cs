using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using GameUtils;
using System;
using static UnityEngine.GraphicsBuffer;


#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
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
        public Ability AbilityEntity { get; private set; }
        public IAbilityExecute AbilityExecution { get; private set; }
        public ExecuteTriggerType ExecuteTriggerType { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public CombatEntity TargetEntity { get; set; }
        public CombatEntity OwnerEntity => AbilityEntity.OwnerEntity;
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
            var actionEvenData = GetComponent<AbilityItemCollisionExecuteComponent>().GetItemEffect<ActionEventEffect>();
            if (clipData.ExecuteClipType == ExecuteClipType.ItemExecute && actionEvenData.FireType == FireType.EndTrigger)
            {
                OnTriggerEvent(null);
            }

            if (clipData.ItemData.ExecuteType == CollisionExecuteType.InHand)
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
                //Log.Debug($"AbilityItem OnTriggerEvent {collisionExecuteData.ActionData.ActionEventType}");
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
                {
                    var effects = AbilityEntity.GetComponent<AbilityTriggerComponent>().AbilityTriggers;
                    for (int i = 0; i < effects.Count; i++)
                    {
                        if (i == (int)ExecuteTriggerType - 1 || ExecuteTriggerType == ExecuteTriggerType.AllTriggers)
                        {
                            var effect = effects[i];
                            var context = new TriggerContext()
                            {
                                AbilityTrigger = effect,
                                TriggerSource = this,
                                AbilityItem = this,
                                Target = otherEntity,
                            };
                            effect.OnTrigger(context);
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
            var executionObject = AssetUtils.LoadObject<ExecutionObject>($"{AbilityManagerObject.ExecutionResFolder}/" + ActionEventData.NewExecution);
            if (executionObject == null)
            {
                Log.Error($"Can not find {ActionEventData.NewExecution}");
                return;
            }
            var sourceExecution = AbilityExecution as AbilityExecution;
            var execution = sourceExecution.OwnerEntity.AddChild<AbilityExecution>(sourceExecution.SkillAbility);
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
        public void ForwardFlyProcess(float inputRadian)
        {
            var abilityItem = this;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
            //moveComp.InputPoint = AbilityExecution.OwnerEntity.Position;

            var x = MathF.Sin(inputRadian);
            var z = MathF.Cos(inputRadian);
            var destination = abilityItem.Position + new Vector3(x, 0, z) * 30;
#if !EGAMEPLAY_ET
            abilityItem.AddComponent<MoveWithDotweenComponent>().DoMoveTo(destination, 1f).OnMoveFinish(() => { Entity.Destroy(abilityItem); });
#endif
        }

        /// <summary>   路径飞行     </summary>
        public void PathFlyProcess(Vector3 inputPoint)
        {
            var skillExecution = (AbilityExecution as AbilityExecution);
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            var tempPoints = clipData.ItemData.GetCtrlPoints();

            if (tempPoints.Count == 0)
            {
                return;
            }
            abilityItem.LocalPosition = tempPoints[0].Position;
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
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
            if (skillExecution.ExecutionObject.TargetInputType == ExecutionTargetInputType.Point)
            {
                abilityItem.Position = moveComp.OriginPoint + abilityItem.LocalPosition;
            }
            moveComp.RotateRadian = 0;
            moveComp.Rotate = false;
            moveComp.Duration = clipData.Duration;
            moveComp.GetPathLocalPoints();
            moveComp.DOMove();
        }

        /// <summary>   朝向路径飞行     </summary>
        public void DirectionPathFlyProcess(Vector3 inputPoint, float inputRadian)
        {
            var abilityItem = this;
            var skillExecution = AbilityExecution as AbilityExecution;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            var tempPoints = clipData.ItemData.GetCtrlPoints();

            if (tempPoints.Count == 0)
            {
                return;
            }
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
            var executePoint = AbilityExecution.Position;
            if (clipData.ItemData.PathExecutePoint == PathExecutePoint.EntityOffset)
            {
                moveComp.ExecutePoint = AbilityExecution.Position + clipData.ItemData.Offset;
            }
            if (clipData.ItemData.PathExecutePoint == PathExecutePoint.InputPoint)
            {
                moveComp.ExecutePoint = inputPoint + clipData.ItemData.Offset;
            }
            abilityItem.Position = executePoint + tempPoints[0].Position;
            abilityItem.Rotation = skillExecution.InputDirection.GetRotation();
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
            moveComp.GetPathLocalPoints();
            moveComp.DOMove();
        }

        /// <summary>   固定位置碰撞体     </summary>
        public void FixedPositionProcess()
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.LocalPosition = clipData.ItemData.FixedPoint;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position + clipData.ItemData.FixedPoint;
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
            var itemUnit = scene.AddChild<ItemUnit, Action<ItemUnit>>((x) => { x.AbilityItem = abilityItem; });
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
            var itemData = executeComp.CollisionExecuteData;
            CollisionEffect collisionData = executeComp.GetItemEffect<CollisionEffect>();
            ItemProxy = abilityItem.GetComponent<AbilityItemViewComponent>();
            CombatContext.Instance.Object2Items[proxyObj.gameObject] = abilityItem;

            if (collisionData.Shape == CollisionShape.Sphere)
            {
                proxyObj.AddComponent<SphereCollider>().enabled = false;
                proxyObj.GetComponent<SphereCollider>().isTrigger = true;
                proxyObj.GetComponent<SphereCollider>().radius = (float)collisionData.Radius;
            }
            if (collisionData.Shape == CollisionShape.Box)
            {
                proxyObj.AddComponent<BoxCollider>().enabled = false;
                proxyObj.GetComponent<BoxCollider>().isTrigger = true;
                proxyObj.GetComponent<BoxCollider>().center = collisionData.Center;
                proxyObj.GetComponent<BoxCollider>().size = collisionData.Size;
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
                    var owner = abilityItem.AbilityEntity.OwnerEntity;
                    Entity target = null;
                    if (CombatContext.Instance.Object2Entities.TryGetValue(other.gameObject, out var otherEntity))
                    {
                        target = otherEntity;
                    }
                    else
                    {
                        if (CombatContext.Instance.Object2Items.TryGetValue(other.gameObject, out var otherItem))
                        {
                            target = otherItem;
                        }
                    }

                    if (collisionData.ExecuteTargetType == CollisionExecuteTargetType.SelfGroup)
                    {
                        if (otherEntity.IsHero != owner.IsHero)
                        {
                            return;
                        }
                    }
                    if (collisionData.ExecuteTargetType == CollisionExecuteTargetType.EnemyGroup)
                    {
                        if (otherEntity.IsHero == owner.IsHero)
                        {
                            return;
                        }
                    }

                    if (owner.CollisionAbility.TryMakeAction(out var collisionAction))
                    {
                        collisionAction.Creator = owner;
                        collisionAction.CauseItem = abilityItem;
                        collisionAction.Target = target;
                        collisionAction.ApplyCollision();
                    }
                };
            }

            var collider = proxyObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (itemData.ObjAsset != null)
            {
                abilityItem.Name = itemData.ObjAsset.name;
                var effectObj = GameObject.Instantiate(itemData.ObjAsset, proxyObj.transform);
                effectObj.transform.localPosition = Vector3.zero;
                effectObj.transform.localRotation = UnityEngine.Quaternion.identity;
            }

            return proxyObj;
        }
#endif
    }
}