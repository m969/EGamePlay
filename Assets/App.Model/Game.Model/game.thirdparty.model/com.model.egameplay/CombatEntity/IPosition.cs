using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using System;
#if EGAMEPLAY_ET
using Vector3 = Unity.Mathematics.float3;
#endif

namespace EGamePlay
{
    public interface IPosition
    {
        Vector3 Position { get; set; }
    }
}
#if EGAMEPLAY_ET
public static class RotationExtension
{
#if EGAMEPLAY_3D
        public static float3 GetForward(this quaternion rotation)
        {
            return math.mul(rotation, math.forward());
        }

        public static quaternion GetRotation(this float3 up)
        {
            return quaternion.LookRotation(math.forward(), up);
        }
#else
    public static float3 GetForward(this quaternion rotation)
    {
        return math.mul(rotation, math.right());
    }

    public static quaternion GetRotation(this float3 right)
    {
        var up = math.cross(math.forward(), right);
        return quaternion.LookRotation(math.forward(), up);
    }
#endif
}
#else
public static class RotationExtension
{
#if EGAMEPLAY_3D
        public static Vector3 GetForward(this Quaternion rotation)
        {
            return rotation * Vector3.forward;
        }

        public static Quaternion GetRotation(this Vector3 up)
        {
            return Quaternion.LookRotation(Vector3.forward, up);
        }
#else
    public static Vector3 GetForward(this Quaternion rotation)
    {
        return rotation * Vector3.right;
    }

    public static Quaternion GetRotation(this Vector3 right)
    {
        var up = math.cross(Vector3.forward, right);
        return Quaternion.LookRotation(Vector3.forward, up);
    }
#endif
}
#endif