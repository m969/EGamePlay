using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ECSGame
{
    public class WorldSystem : AEntitySystem<World>,
        IInit<World>,
        IUpdate<World>
    {
        public static World Create(Assembly systemAssembly)
        {
            var world = EcsNodeSystem.Create<World>(EcsType.World, systemAssembly);
            // world.AddComponent<ActorListComponent>();
            return world;
        }

        public void Init(World world)
        {

        }

        public void Update(World entity)
        {
        }
    }
}
