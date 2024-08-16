using Sirenix.OdinInspector;
using UnityEngine;

namespace EGamePlay.Combat
{
    [Effect("碰撞体", 10)]
    public class CollisionEffect : ItemEffect
    {
        public override string Label => "碰撞体";

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

        [ToggleGroup("Enabled")]
        [LabelText("碰撞对象")]
        public CollisionExecuteTargetType ExecuteTargetType;
    }
}