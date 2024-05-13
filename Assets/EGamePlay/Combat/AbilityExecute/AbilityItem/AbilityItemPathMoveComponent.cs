using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using System;
using NaughtyBezierCurves;
using ET;
using float3 = UnityEngine.Vector3;
#if UNITY
using DG.Tweening;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemPathMoveComponent : Component
    {
        public IPosition PositionEntity { get; set; }
        public float RotateAgree { get; set; }
        public float Duration { get; set; }
        public float Speed { get; set; } = 0.05f;
        public float Progress { get; set; }
        public BezierCurve3D BezierCurve { get; set; }


        public override void Update()
        {
            var abilityItem = GetEntity<AbilityItem>();
            //if (abilityItem.ItemUnit != null && !PositionEntity.Position.Equals(abilityItem.ItemUnit.MapUnit().Position))
            //{
            //    PositionEntity.Position = abilityItem.ItemUnit.MapUnit().Position;
            //}
        }

        public float3[] GetPathPoints()
        {
            var pathPoints = new float3[BezierCurve.Sampling];
            var perc = 1f / BezierCurve.Sampling;
            for (int i = 1; i <= BezierCurve.Sampling; i++)
            {
                var progress = perc * i;
                var endValue = BezierCurve.GetPoint(progress);
                pathPoints[i - 1] = endValue;
                //ET.Log.Console($"{progress} {endValue}");
            }

            var duration = Duration;
            var length = math.distance(pathPoints[0], PositionEntity.Position);
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
            Speed = speed;

            return pathPoints;
        }

        public void DOMove()
        {
            Progress = 1f / BezierCurve.Sampling;

            var endValue = BezierCurve.GetPoint(Progress);
            //var endValue = Evaluate(Progress);
            var startPos = PositionEntity.Position;
            //Log.Debug($"{startPos} {endValue} {Speed} {Progress}");
#if UNITY
            var duration = math.distance(endValue, startPos) / Speed;
            DOTween.To(() => startPos, (x) => PositionEntity.Position = x, endValue, duration).SetEase(Ease.Linear).OnComplete(DOMoveNext);
#else
                    //EventSystem.Instance.Publish()
#endif
        }

        private void DOMoveNext()
        {
            if (Progress >= 1f)
            {
                return;
            }
            Progress += 1f / BezierCurve.Sampling;
            Progress = System.Math.Min(1f, Progress);
            var endValue = BezierCurve.GetPoint(Progress);
            //var endValue = Evaluate(Progress);
            var startPos = PositionEntity.Position;
            //Log.Debug($"{startPos} {endValue} {Speed} {Progress}");
#if UNITY
            var duration = math.distance(endValue, startPos) / Speed;
            DOTween.To(() => startPos, (x) => PositionEntity.Position = x, endValue, duration).SetEase(Ease.Linear).OnComplete(DOMoveNext);
#endif
        }
    }
}