using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ColliderSpawnData
    {
        public bool HasStart;
        public ExecutionEventEmitter ColliderSpawnEmitter;
    }

    /// <summary>
    /// 
    /// </summary>
    public class EffectExecutionSpawnItemComponent : Component
    {
        public ColliderSpawnData ColliderSpawnData { get; set; }
    }
}