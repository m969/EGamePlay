using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vector3 = UnityEngine.Vector3;

namespace ECSGame
{
    public class MoveSystem : AComponentSystem<EcsEntity, MoveComponent>,
IAwake<EcsEntity, MoveComponent>
    {
        public void Awake(EcsEntity actor, MoveComponent moveComponent)
        {
        }

        public static void SetSpeed(EcsEntity entity, int value)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Speed = value;
        }
    } 
}
