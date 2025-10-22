using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class CollisionEvent : AEventRun<CollisionEvent, EcsEntity, EcsEntity>
    {
        protected override async ETTask Run(EcsEntity entity1, EcsEntity entity2)
        {
            if (entity1 is Item)
            {
                EcsObject.Destroy(entity1);
            }
            if (entity2 is Item)
            {
                EcsObject.Destroy(entity2);
            }
        }
    }
}
