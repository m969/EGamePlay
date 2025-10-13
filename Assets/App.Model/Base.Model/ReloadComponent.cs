using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public class ReloadComponent : EcsComponent
    {
        public Assembly SystemAssembly { get; set; }
    }
}