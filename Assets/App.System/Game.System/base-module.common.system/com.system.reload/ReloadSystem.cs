using ECS;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class ReloadSystem : AComponentSystem<EcsNode, ReloadComponent>
    {
        public static object CreateInstance(EcsNode entity, string typeName)
        {
            var instance = entity.GetComponent<ReloadComponent>().SystemAssembly.CreateInstance(typeName);
            return instance;
        }
    } 
}
