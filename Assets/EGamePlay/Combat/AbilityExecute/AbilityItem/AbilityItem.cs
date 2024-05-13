using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using GameUtils;
using System;
using float3 = UnityEngine.Vector3;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : Entity, IPosition
    {
        public Entity AbilityEntity => AbilityExecution.AbilityEntity;
        public IAbilityExecute AbilityExecution { get; set; }
        //public ExecutionEffectComponent ItemExecutionEffectComponent { get; private set; }
        public EffectApplyType EffectApplyType { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public CombatEntity TargetEntity { get; set; }


        public override void Awake(object initData)
        {
            AbilityExecution = initData as IAbilityExecute;
            if (AbilityEntity == null)
            {
                return;
            }
            var abilityEffects = AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
            foreach (var abilityEffect in abilityEffects)
            {
                if (abilityEffect.EffectConfig.Decorators != null)
                {
                    foreach (var effectDecorator in abilityEffect.EffectConfig.Decorators)
                    {
                        if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                        {
                            AddComponent<AbilityItemTargetCounterComponent>();
                        }
                    }
                }
            }
        }

        /// 结束单元体
        public void DestroyItem()
        {
            Log.Debug("AbilityItem DestroyItem");
            Destroy(this);
        }

        public override void OnDestroy()
        {
            //Log.Debug("AbilityItem OnDestroy");
        }

        public void OnCollision(CombatEntity otherCombatEntity)
        {
            if (TargetEntity != null)
            {
                if (otherCombatEntity != TargetEntity)
                {
                    return;
                }
            }

            var collisionExecuteData = GetComponent<AbilityItemCollisionExecuteComponent>().CollisionExecuteData;

            if (AbilityEntity != null)
            {
                //Log.Debug($"AbilityItem OnCollision {collisionExecuteData.ActionData.ActionEventType}");
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
                                effect.TriggerObserver.OnTriggerWithAbilityItem(this, otherCombatEntity);
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
            var executionObject = AssetUtils.LoadObject<ExecutionObject>(ActionEventData.NewExecution);
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
            //if (executionObject != null)
            //{
            //    execution.AddComponent<UpdateComponent>();
            //}
        }

        /// <summary>   目标飞行碰撞体     </summary>
        public void TargetFlyProcess(CombatEntity InputTarget)
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.TargetEntity = InputTarget;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
            var moveComp = abilityItem.AddComponent<MoveWithDotweenComponent>();
            moveComp.DoMoveToWithTime(InputTarget, clipData.Duration);
        }

        /// <summary>   前向飞行碰撞体     </summary>
        public void ForwardFlyProcess(float InputDirection)
        {
            var abilityItem = this;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
            var x = Mathf.Sin(Mathf.Deg2Rad * InputDirection);
            var z = Mathf.Cos(Mathf.Deg2Rad * InputDirection);
            var destination = abilityItem.Position + new Vector3(x, 0, z) * 30;
            abilityItem.AddComponent<MoveWithDotweenComponent>().DoMoveTo(destination, 1f).OnMoveFinish(() => { Entity.Destroy(abilityItem); });
        }

        /// <summary>   路径飞行     </summary>
        public void PathFlyProcess()
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
            abilityItem.Position = AbilityExecution.OwnerEntity.Position + (float3)tempPoints[0].Position;
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.BezierCurve = new NaughtyBezierCurves.BezierCurve3D();
            moveComp.BezierCurve.Position = AbilityExecution.OwnerEntity.Position;
            moveComp.BezierCurve.Sampling = clipData.CollisionExecuteData.BezierCurve.Sampling;
            moveComp.BezierCurve.KeyPoints = tempPoints;
            foreach (var item in tempPoints)
            {
                item.Curve = moveComp.BezierCurve;
            }
            if (skillExecution.ExecutionObject.TargetInputType == ExecutionTargetInputType.Point)
            {
                abilityItem.Position = (float3)skillExecution.InputPoint + (float3)tempPoints[0].Position;
                //Log.Debug($"InputPoint={InputPoint} {tempPoints[0].Position}");
                moveComp.BezierCurve.Position = skillExecution.InputPoint;
            }
            moveComp.RotateAgree = 0;
            moveComp.Duration = clipData.Duration;
            moveComp.GetPathPoints();
            moveComp.DOMove();
        }

        /// <summary>   朝向路径飞行     </summary>
        public void DirectionPathFlyProcess()
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
            var tempPoints = clipData.CollisionExecuteData.GetCtrlPoints();

            var angle = AbilityExecution.OwnerEntity.Rotation.y - 90;
            var agree = angle * MathF.PI / 180;

            if (tempPoints.Count == 0)
            {
                return;
            }
            abilityItem.Position = AbilityExecution.OwnerEntity.Position + (float3)tempPoints[0].Position;
            var moveComp = abilityItem.AddComponent<AbilityItemPathMoveComponent>();
            moveComp.PositionEntity = abilityItem;
            moveComp.BezierCurve = new NaughtyBezierCurves.BezierCurve3D();
            moveComp.BezierCurve.Position = AbilityExecution.OwnerEntity.Position;
            moveComp.BezierCurve.Sampling = clipData.CollisionExecuteData.BezierCurve.Sampling;
            moveComp.BezierCurve.KeyPoints = tempPoints;
            foreach (var item in tempPoints)
            {
                item.Curve = moveComp.BezierCurve;
            }
            moveComp.RotateAgree = agree;
            moveComp.Duration = clipData.Duration;
            moveComp.GetPathPoints();
            moveComp.DOMove();
        }

        /// <summary>   固定位置碰撞体     </summary>
        public void FixedPositionProcess(Vector3 InputPoint)
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.Position = InputPoint;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
        }

        /// <summary>   固定方向碰撞体     </summary>
        public void FixedDirectionProcess()
        {
            var abilityItem = this;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            abilityItem.Position = AbilityExecution.OwnerEntity.Position;
            abilityItem.Rotation = AbilityExecution.OwnerEntity.Rotation;
            abilityItem.AddComponent<LifeTimeComponent>(clipData.Duration);
        }

        /// <summary>   创建技能碰撞体     </summary>
        public GameObject CreateAbilityItemProxyObj()
        {
            var abilityItem = this;
            var proxyObj = new GameObject("AbilityItemProxy");
            proxyObj.transform.position = abilityItem.Position;
            proxyObj.transform.rotation = abilityItem.Rotation;
            proxyObj.AddComponent<AbilityItemProxyObj>().AbilityItem = abilityItem;
            var clipData = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>().CollisionExecuteData;

            if (clipData.Shape == CollisionShape.Sphere)
            {
                proxyObj.AddComponent<SphereCollider>().enabled = false;
                proxyObj.GetComponent<SphereCollider>().radius = clipData.Radius;
            }
            if (clipData.Shape == CollisionShape.Box)
            {
                proxyObj.AddComponent<BoxCollider>().enabled = false;
                proxyObj.GetComponent<BoxCollider>().center = clipData.Center;
                proxyObj.GetComponent<BoxCollider>().size = clipData.Size;
            }

            proxyObj.AddComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) => {
                var combatEntity = CombatContext.Instance.Object2Entities[other.gameObject];
                abilityItem.OnCollision(combatEntity);
            };

            proxyObj.GetComponent<Collider>().enabled = true;

            if (clipData.ObjAsset != null)
            {
                abilityItem.Name = clipData.ObjAsset.name;
                var effectObj = GameObject.Instantiate(clipData.ObjAsset, proxyObj.transform);
                effectObj.transform.localPosition = Vector3.zero;
                effectObj.transform.localRotation = Quaternion.identity;
            }

            return proxyObj;
        }
    }
}