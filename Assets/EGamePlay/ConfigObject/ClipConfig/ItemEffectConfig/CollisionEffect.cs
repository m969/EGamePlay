using Sirenix.OdinInspector;
using UnityEngine;

namespace EGamePlay.Combat
{
    [Effect("碰撞体效果", 20)]
    public class CollisionEffect : ItemEffect
    {
        public override string Label => "碰撞体效果";

        [ToggleGroup("Enabled")]
        public CollisionShape Shape;

        [ToggleGroup("Enabled")]
        [ShowIf("Shape", CollisionShape.Sphere), LabelText("半径")]
        public double Radius;

        [ToggleGroup("Enabled")]
        [ShowIf("Shape", CollisionShape.Box)]
        public Vector3 Center;

        [ToggleGroup("Enabled")]
        [ShowIf("Shape", CollisionShape.Box)]
        public Vector3 Size;
    }
}