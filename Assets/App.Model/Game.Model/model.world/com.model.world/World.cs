using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ECSGame
{
    public class World : EcsNode
    {
        public World(ushort ecsTypeId) : base(ecsTypeId)
        {
        }
    }
}
