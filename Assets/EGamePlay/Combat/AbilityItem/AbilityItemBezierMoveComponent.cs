using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using DG.Tweening;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemBezierMoveComponent : Component
    {
        public IPosition PositionEntity { get; set; }
        public List<CtrlPoint> ctrlPoints { get; set; }
        public float Speed { get; set; } = 0.05f;
        float SegmentCount = 20;
        float Progress;


        public void DOMove()
        {
            Progress = 0.1f;
            var endValue = Evaluate(Progress);
            DOTween.To(() => PositionEntity.Position, (x) => PositionEntity.Position = x, endValue, Speed).SetEase(Ease.Linear).OnComplete(DOMoveNext);
        }

        private void DOMoveNext()
        {
            if (Progress >= 1f)
            {
                return;
            }
            Progress += 0.1f;
            var endValue = Evaluate(Progress);
            DOTween.To(() => PositionEntity.Position, (x) => PositionEntity.Position = x, endValue, Speed).SetEase(Ease.Linear).OnComplete(DOMoveNext);
        }

        public Vector3 Evaluate(float t, int derivativeOrder = 0)
        {
            if (ctrlPoints.Count == 0) return PositionEntity.Position;
            if (ctrlPoints.Count == 1) return ctrlPoints[0].position;
            t = Mathf.Clamp(t, 0, SegmentCount);
            int segment_index = (int)t;
            if (segment_index == SegmentCount) segment_index -= 1;
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
                return v;
            }
            else if (derivativeOrder > 0)
            {
                Vector3[] q = new Vector3[3];
                q[0] = 3 * (p[1] - p[0]);
                q[1] = 3 * (p[2] - p[1]);
                q[2] = 3 * (p[3] - p[2]);
                //一阶导
                if (derivativeOrder == 1)
                {
                    return q[0] * u * u + 2 * q[1] * t * u + q[2] * t * t;
                }
                else if (derivativeOrder > 1)
                {
                    Vector3[] r = new Vector3[2];
                    r[0] = 2 * (q[1] - q[0]);
                    r[1] = 2 * (q[2] - q[1]);
                    //二阶导
                    if (derivativeOrder == 2)
                    {
                        return r[0] * u + r[1] * t;
                    }
                    else if (derivativeOrder > 2)
                    {
                        //三阶导
                        if (derivativeOrder == 3)
                        {
                            return r[1] - r[0];
                        }
                        //其他阶导
                        else if (derivativeOrder > 3)
                        {
                            return Vector3.zero;
                        }
                    }
                }
            }
            return Vector3.zero;
        }
    }
}