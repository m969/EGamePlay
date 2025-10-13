using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECSGame
{
    public class CollisionSystem : AComponentSystem<EcsEntity, CollisionComponent>,
IAwake<EcsEntity, CollisionComponent>
    {
        public void Awake(EcsEntity entity, CollisionComponent component)
        {
        }
    }
}
