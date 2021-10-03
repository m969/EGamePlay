using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ColliderSpawnData
    {
        public bool HasStart;
        public ColliderSpawnEmitter ColliderSpawnEmitter;
    }

    /// <summary>
    /// 
    /// </summary>
    public class EffectSpawnItemComponent : Component
    {
        public ColliderSpawnData ColliderSpawnData { get; set; }
    }
}