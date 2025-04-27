using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public enum EcsNodeType
    {
        TrueAuthority = 1,
        LocalPrePlay = 2,
    }

    public class ConfigComponent : EcsComponent
    {
        public EcsNodeType NodeType { get; set; }
    }
}