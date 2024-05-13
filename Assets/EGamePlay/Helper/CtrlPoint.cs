using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;

namespace EGamePlay
{
    public enum BezierPointType
    {
        corner,
        smooth,
        bezierCorner,
    }

    [System.Serializable]
    public class CtrlPoint
    {
        public BezierPointType type;
        public Vector3 position;
        [SerializeField]
        Vector3 inTangent;
        [SerializeField]
        Vector3 outTangent;

        public Vector3 InTangent
        {
            get
            {
                if (type == BezierPointType.corner) return Vector3.zero;
                else return inTangent;
            }
            set
            {
                if (type != BezierPointType.corner) inTangent = value;
                if (value.sqrMagnitude > 0.001 && type == BezierPointType.smooth)
                {
                    outTangent = value.normalized * (-1) * outTangent.magnitude;
                }
            }
        }

        public Vector3 OutTangent
        {
            get
            {
                if (type == BezierPointType.corner) return Vector3.zero;
                if (type == BezierPointType.smooth)
                {
                    if (inTangent.sqrMagnitude > 0.001)
                    {
                        return inTangent.normalized * (-1) * outTangent.magnitude;
                    }
                }
                return outTangent;
            }
            set
            {
                if (type == BezierPointType.smooth)
                {
                    if (value.sqrMagnitude > 0.001)
                    {
                        inTangent = value.normalized * (-1) * inTangent.magnitude;
                    }
                    outTangent = value;
                }
                if (type == BezierPointType.bezierCorner) outTangent = value;
            }
        }
    }
}
