using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using DG.Tweening;
using float3 = UnityEngine.Vector3;

namespace EGamePlay
{
    public class AbilityItemMoveSystem : AComponentSystem<AbilityItem, AbilityItemMoveComponent>,
        IAwake<AbilityItem, AbilityItemMoveComponent>
    {
        public void Awake(AbilityItem entity, AbilityItemMoveComponent component)
        {
            component.PositionEntity = entity;
        }

        public static void Update(AbilityItem abilityItem, AbilityItemMoveComponent component)
        {
#if EGAMEPLAY_ET
            var itemUnit = abilityItem.GetComponent<CombatUnitComponent>().Unit;
            if (itemUnit != null && !PositionEntity.Position.Equals(itemUnit.MapUnit().Position))
            {
                PositionEntity.Position = itemUnit.MapUnit().Position;
            }
#endif

            if (component.MoveType == MoveType.TargetMove)
            {
                if (component.TargetEntity != null && component.PositionEntity != null)
                {
                    if (math.distance(component.TargetEntity.Position, component.PositionEntity.Position) < 0.01f)
                    {
                        component.TargetEntity = null;
                        return;
                    }

                    var step = component.TargetEntity.Position - component.PositionEntity.Position;
                    step = step.normalized * component.Speed;
                    component.PositionEntity.Position += step;
                }
            }

            if (component.MoveType == MoveType.DestinationMove)
            {
                if (math.distance(component.TargetPoint, component.PositionEntity.Position) < 0.01f)
                {
                    return;
                }

                var step = component.TargetPoint - component.PositionEntity.Position;
                step = step.normalized * component.Speed;
                component.PositionEntity.Position += step;
            }
        }

        public static float3[] GetPathPoints(AbilityItem abilityItem)
        {
            var component = abilityItem.GetComponent<AbilityItemMoveComponent>();
            var points = GetPathLocalPoints(abilityItem);
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                points[i] = point + component.OriginPoint;
            }
            return points;
        }

        public static float3[] GetPathLocalPoints(AbilityItem abilityItem)
        {
            var component = abilityItem.GetComponent<AbilityItemMoveComponent>();
            var pathPoints = new float3[component.BezierCurve.Sampling];
            var perc = 1f / component.BezierCurve.Sampling;
            for (int i = 1; i <= component.BezierCurve.Sampling; i++)
            {
                var progress = perc * i;
                var endValue = component.BezierCurve.GetPoint(progress);
                var v = endValue;

                if (component.Rotate)
                {
                    var a = component.RotateRadian;

                    var x = v.x;
                    var y = v.y;

                    var x1 = x * math.cos(a) - y * math.sin(a);
                    var y1 = (y * math.cos(a) + x * math.sin(a));

                    v = new float3(x1, y1, v.z);
                    //Log.Console($"PositionEntity.Position={PositionEntity.Position}, v.y={v.y}, v={v}");
                }

                pathPoints[i - 1] = v;
                //ET.Log.Console($"{progress} {endValue}");
            }

            var duration = component.Duration;
            var length = math.distance(pathPoints[0], abilityItem.LocalPosition);
            for (int i = 0; i < pathPoints.Length; i++)
            {
                if (i == pathPoints.Length - 1)
                {
                    break;
                }
                var dist = math.distance(pathPoints[i + 1], pathPoints[i]);
                length += dist;
            }
            var speed = length / duration;
            component.Speed = speed;

            return pathPoints;
        }

        public static void MoveToTarget(AbilityItem abilityItem, IPosition target, float speed)
        {
            var component = abilityItem.GetComponent<AbilityItemMoveComponent>();
            component.MoveType = MoveType.TargetMove;
            component.Speed = speed;
            component.TargetEntity = target;
        }

        public static void MoveToPoint(AbilityItem abilityItem, Vector3 point, float speed)
        {
            var component = abilityItem.GetComponent<AbilityItemMoveComponent>();
            component.MoveType = MoveType.DestinationMove;
            component.Speed = speed;
            component.TargetPoint = point;
        }

        //        public static void FollowMove(AbilityItem abilityItem)
        //        {
        //            var component = abilityItem.GetComponent<AbilityItemPathMoveComponent>();
        //#if !EGAMEPLAY_ET
        //            var localPos = abilityItem.LocalPosition;
        //            var endValue = component.OriginPoint + localPos;
        //            component.PositionEntity.Position = endValue;
        //#endif
        //        }

        public static void DOMove(AbilityItem abilityItem)
        {
            var component = abilityItem.GetComponent<AbilityItemMoveComponent>();
            component.MoveType = MoveType.PathMove;
            component.Progress = 0;
            DOMoveNext(abilityItem);
        }

        private static void DOMoveNext(AbilityItem abilityItem)
        {
            var component = abilityItem.GetComponent<AbilityItemMoveComponent>();
            if (component.Progress >= 1f)
            {
                return;
            }
            component.Progress += 1f / component.BezierCurve.Sampling;
            component.Progress = System.Math.Min(1f, component.Progress);
            var localPos = component.BezierCurve.GetPoint(component.Progress);

            var v = localPos;
            var a = component.RotateRadian;
            var x = v.x;
            var y = v.z;
            var x1 = x * math.cos(a) - y * math.sin(a);
            var y1 = (y * math.cos(a) + x * math.sin(a));
            v = new float3(x1, v.y, y1);

            abilityItem.LocalPosition = v;
            var endValue = component.OriginPoint + v;
            var startPos = component.PositionEntity.Position;
#if UNITY
            var duration = math.distance(endValue, startPos) / component.Speed;
            DOTween.To(() => startPos, (x) => component.PositionEntity.Position = x, endValue, duration).SetEase(DG.Tweening.Ease.Linear).OnComplete(() => DOMoveNext(abilityItem));
#endif
        }
    }
}