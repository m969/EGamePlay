using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class TransformComponent : EcsComponent
    {
        public UnityEngine.Vector3 Position
        {
            get;
            set;
        }

        public UnityEngine.Vector3 Forward
        {
            get => this.Rotation * UnityEngine.Vector3.forward;
            set => this.Rotation = UnityEngine.Quaternion.LookRotation(value, UnityEngine.Vector3.up);
        }

        public UnityEngine.Quaternion Rotation
        {
            get;
            set;
        }
    }
}
