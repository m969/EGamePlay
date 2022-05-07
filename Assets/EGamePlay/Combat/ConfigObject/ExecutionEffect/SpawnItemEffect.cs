using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    public class ColliderSpawnData
    {
        public bool HasStart;
        public ExecutionEventEmitter ColliderSpawnEmitter;
    }

    public class SpawnItemEffect : Effect
    {
        public override string Label => "生成碰撞体";

        public ColliderSpawnData ColliderSpawnData { get; set; }
    }
}