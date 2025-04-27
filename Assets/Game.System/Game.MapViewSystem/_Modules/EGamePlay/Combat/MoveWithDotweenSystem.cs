using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using DG.Tweening;
using System;

namespace EGamePlay
{
    //public class MoveWithDotweenSystem : AComponentSystem<AbilityItem, MoveWithDotweenComponent>,
    //    IAwake<AbilityItem, MoveWithDotweenComponent>
    //{
    //    public void Awake(AbilityItem entity, MoveWithDotweenComponent component)
    //    {
    //        component.PositionEntity = (IPosition)entity;
    //        component.ElapsedTime = 0;
    //    }

    //    public static void Update(AbilityItem entity)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        if (component.TargetPositionEntity != null)
    //        {
    //            if (component.TargetEntity.IsDisposed)
    //            {
    //                component.TargetEntity = null;
    //                component.TargetPositionEntity = null;
    //                EcsObject.Destroy(entity);
    //                return;
    //            }
    //            if (component.SpeedType == SpeedType.Speed) DoMoveToWithSpeed(entity, component.TargetPositionEntity, component.Speed);
    //            if (component.SpeedType == SpeedType.Duration) DoTimeMove(entity, MathF.Max(0, component.Duration - component.ElapsedTime));
    //            component.ElapsedTime += Time.deltaTime;
    //        }
    //    }

    //    public static MoveWithDotweenComponent DoMoveTo(AbilityItem entity, Vector3 destination, float duration)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        component.Destination = destination;
    //        DOTween.To(() => { return component.PositionEntity.Position; }, (x) => component.PositionEntity.Position = x, component.Destination, duration).SetEase(Ease.Linear).OnComplete(() => { OnMoveFinish(entity); });
    //        return component;
    //    }

    //    public static void DoMoveToWithSpeed(AbilityItem entity, IPosition targetPositionEntity, float speed = 1f)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        component.Speed = speed;
    //        component.SpeedType = SpeedType.Speed;
    //        component.TargetPositionEntity = targetPositionEntity;
    //        component.TargetEntity = targetPositionEntity as EcsEntity;
    //        component.MoveTweener?.Kill();
    //        var dist = Vector3.Distance(component.PositionEntity.Position, component.TargetPositionEntity.Position);
    //        var duration = dist / speed;
    //        component.MoveTweener = DOTween.To(() => { return component.PositionEntity.Position; }, (x) => component.PositionEntity.Position = x, component.TargetPositionEntity.Position, duration);
    //    }

    //    public static void DoMoveToWithTime(AbilityItem entity, IPosition targetPositionEntity, float time = 1f)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        component.Duration = time;
    //        component.SpeedType = SpeedType.Duration;
    //        component.TargetPositionEntity = targetPositionEntity;
    //        component.TargetEntity = targetPositionEntity as EcsEntity;
    //        DoTimeMove(entity, time);
    //    }

    //    private static void DoTimeMove(AbilityItem entity, float time)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        component.MoveTweener?.Kill();
    //        component.MoveTweener = DOTween.To(() => { return component.PositionEntity.Position; }, (x) => component.PositionEntity.Position = x, component.TargetPositionEntity.Position, time);
    //        component.MoveTweener.SetEase(Ease.Linear);
    //    }

    //    public static void OnMoveFinish(AbilityItem entity, System.Action action)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        component.MoveFinishAction = action;
    //    }

    //    private static void OnMoveFinish(AbilityItem entity)
    //    {
    //        var component = entity.GetComponent<MoveWithDotweenComponent>();
    //        component.MoveFinishAction?.Invoke();
    //    }
    //}
}