using DG.Tweening;
using NaughtyBezierCurves;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace EGamePlay
{
    public class BezierComponent : MonoBehaviour
    {
        //[ShowInInspector]
        public List<BezierPoint3D> ctrlPoints { get => CollisionExecuteData.BezierCurve.KeyPoints; }
        public BezierCurve3D BezierCurve { get => CollisionExecuteData.BezierCurve; }
        public ItemExecute CollisionExecuteData;

        //这两个画Beizer线段的时候要用
        private Vector3 lastPosition;
        private Vector3 lastOutTangent;
        //正在操作哪个控制点
        int pickedIndex = -1;
        //正在操作控制点的哪一部分
        enum CtrlPointPickedType
        {
            position,
            inTangent,
            outTangent
        }

        CtrlPointPickedType pickedType = CtrlPointPickedType.position;

        float SegmentCount = 100;

        public float Progress;
        //public void DOMove()
        //{
        //    Progress = 0.1f;
        //    transform.DOMove(Evaluate(Progress), 0.3f).OnComplete(DOMoveNext);
        //}

        //public void DOMoveNext()
        //{
        //    if (Progress >= 1f)
        //    {
        //        return;
        //    }
        //    Progress += 0.1f;
        //    transform.DOMove(Evaluate(Progress), 0.3f).OnComplete(DOMoveNext);
        //}

        //public Vector3 Evaluate(float t, int derivativeOrder = 0)
        //{
        //    if (ctrlPoints.Count == 0) return transform.position;
        //    if (ctrlPoints.Count == 1) return ctrlPoints[0].position;
        //    t = Mathf.Clamp(t, 0, SegmentCount);
        //    int segment_index = (int)t;
        //    if (segment_index == SegmentCount) segment_index -= 1;
        //    Vector3[] p = new Vector3[4];
        //    p[0] = ctrlPoints[segment_index].position;
        //    p[1] = ctrlPoints[segment_index].OutTangent + p[0];
        //    p[3] = ctrlPoints[segment_index + 1].position;
        //    p[2] = ctrlPoints[segment_index + 1].InTangent + p[3];

        //    t = t - segment_index;
        //    float u = 1 - t;
        //    if (derivativeOrder < 0) derivativeOrder = 0;
        //    //原函数
        //    if (derivativeOrder == 0)
        //    {
        //        var v = p[0] * u * u * u + 3 * p[1] * u * u * t + 3 * p[2] * u * t * t + p[3] * t * t * t;
        //        //Debug.Log($"Evaluate {t} {v}");
        //        return v;
        //    }
        //    else if (derivativeOrder > 0)
        //    {
        //        Vector3[] q = new Vector3[3];
        //        q[0] = 3 * (p[1] - p[0]);
        //        q[1] = 3 * (p[2] - p[1]);
        //        q[2] = 3 * (p[3] - p[2]);
        //        //一阶导
        //        if (derivativeOrder == 1)
        //        {
        //            return q[0] * u * u + 2 * q[1] * t * u + q[2] * t * t;
        //        }
        //        else if (derivativeOrder > 1)
        //        {
        //            Vector3[] r = new Vector3[2];
        //            r[0] = 2 * (q[1] - q[0]);
        //            r[1] = 2 * (q[2] - q[1]);
        //            //二阶导
        //            if (derivativeOrder == 2)
        //            {
        //                return r[0] * u + r[1] * t;
        //            }
        //            else if (derivativeOrder > 2)
        //            {
        //                //三阶导
        //                if (derivativeOrder == 3)
        //                {
        //                    return r[1] - r[0];
        //                }
        //                //其他阶导
        //                else if (derivativeOrder > 3)
        //                {
        //                    return Vector3.zero;
        //                }
        //            }
        //        }
        //    }
        //    return Vector3.zero;
        //}

        private void OnDrawGizmos()
        {
            var bezierComponent = this;
            if (bezierComponent.ctrlPoints == null)
            {
                return;
            }
            for (int i = 0; i < bezierComponent.ctrlPoints.Count; i++)
            {
                //一个个地把控制点渲染出来
                var ctrlPoint = bezierComponent.ctrlPoints[i];
                var type = ctrlPoint.HandleStyle;
                var position = ctrlPoint.Position;
                var inTangentPoint = ctrlPoint.LeftHandleLocalPosition + position;
                var outTangentPoint = ctrlPoint.RightHandleLocalPosition + position;
                if (type == BezierPoint3D.HandleType.Broken)
                {
                    inTangentPoint = position;
                    outTangentPoint = position;
                }
                //从第二个控制点开始画Bezier线段
                if (i > 0)
                {
                    Handles.DrawBezier(lastPosition, position, lastOutTangent, inTangentPoint, Color.green, null, 2f);
                }
                //所以每次先暂存下控制点位置和OutTangent，留给下一个控制点画线用
                lastPosition = position;
                lastOutTangent = outTangentPoint;
            }
        }
    }
}
