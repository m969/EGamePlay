using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
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