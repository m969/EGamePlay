using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using DG.Tweening;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemBezierMoveComponent : Component
    {
        public IPosition PositionEntity { get; set; }
        public Vector3 OriginPosition { get; set; }
        public float RotateAgree { get; set; }
        public List<CtrlPoint> ctrlPoints { get; set; }
        public float Duration { get; set; }
        public float Speed { get; set; } = 0.05f;
        float SegmentCount = 20;
        float Progress;


        public void DOMove()
        {
            Progress = 0.1f;
            var endValue = Evaluate(Progress);
            var startPos = PositionEntity.Position;
            DOTween.To(() => startPos, (x) => PositionEntity.Position = x, endValue, Speed).SetEase(Ease.Linear).OnComplete(DOMoveNext);
        }

        private void DOMoveNext()
        {
            if (Progress >= 1f)
            {
                return;
            }
            Progress += 0.1f;
            Progress = Mathf.Min(1f, Progress);
            var endValue = Evaluate(Progress);
            //Log.Debug($"{Progress} {endValue} {Speed}");
            var startPos = PositionEntity.Position;
            DOTween.To(() => startPos, (x) => PositionEntity.Position = x, endValue, Speed).SetEase(Ease.Linear).OnComplete(DOMoveNext);
        }

        public Vector3 Evaluate(float t, int derivativeOrder = 0)
        {
            if (ctrlPoints.Count == 0) return PositionEntity.Position;
            if (ctrlPoints.Count == 1) return ctrlPoints[0].position;
            //var tempT = t;
            t = Mathf.Clamp(t, 0, 1f);
            t = t * ctrlPoints.Count;
            int segment_index = (int)t;
            //Log.Debug($"{Progress} {segment_index} {tempT}");
            if (segment_index + 1 >= ctrlPoints.Count)
            {
                var v = ctrlPoints[segment_index].position;
                var a = RotateAgree;
                var x = v.x;
                var y = v.z;
                var x1 = x * Mathf.Cos(a) - y * Mathf.Sin(a);
                var y1 = -(y * Mathf.Cos(a) + x * Mathf.Sin(a));
                //Log.Debug($"x={x},y={y} x1={x1},y={y1}");
                v = OriginPosition + new Vector3(x1, v.y, y1);
                return v;//segment_index -= 1;
            }
            Vector3[] p = new Vector3[4];
            p[0] = ctrlPoints[segment_index].position;
            p[1] = ctrlPoints[segment_index].OutTangent + p[0];
            p[3] = ctrlPoints[segment_index + 1].position;
            p[2] = ctrlPoints[segment_index + 1].InTangent + p[3];

            t = t - segment_index;
            float u = 1 - t;
            if (derivativeOrder < 0) derivativeOrder = 0;
            //原函数
            if (derivativeOrder == 0)
            {
                var v = p[0] * u * u * u + 3 * p[1] * u * u * t + 3 * p[2] * u * t * t + p[3] * t * t * t;
                //Debug.Log($"Evaluate {t} {v}");
                var a = RotateAgree;
                var x = v.x;
                var y = v.z;
                var x1 = x * Mathf.Cos(a) - y * Mathf.Sin(a);
                var y1 = -(y * Mathf.Cos(a) + x * Mathf.Sin(a));
                //Log.Debug($"x={x},y={y} x1={x1},y={y1}");
                v = OriginPosition + new Vector3(x1, v.y, y1);
                return v;
            }
            //else if (derivativeOrder > 0)
            //{
            //    Vector3[] q = new Vector3[3];
            //    q[0] = 3 * (p[1] - p[0]);
            //    q[1] = 3 * (p[2] - p[1]);
            //    q[2] = 3 * (p[3] - p[2]);
            //    //一阶导
            //    if (derivativeOrder == 1)
            //    {
            //        return q[0] * u * u + 2 * q[1] * t * u + q[2] * t * t;
            //    }
            //    else if (derivativeOrder > 1)
            //    {
            //        Vector3[] r = new Vector3[2];
            //        r[0] = 2 * (q[1] - q[0]);
            //        r[1] = 2 * (q[2] - q[1]);
            //        //二阶导
            //        if (derivativeOrder == 2)
            //        {
            //            return r[0] * u + r[1] * t;
            //        }
            //        else if (derivativeOrder > 2)
            //        {
            //            //三阶导
            //            if (derivativeOrder == 3)
            //            {
            //                return r[1] - r[0];
            //            }
            //            //其他阶导
            //            else if (derivativeOrder > 3)
            //            {
            //                return Vector3.zero;
            //            }
            //        }
            //    }
            //}
            return Vector3.zero;
        }
    }
}