using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public abstract class AEntitySystem<TEntity> : IEcsEntitySystem where TEntity : EcsEntity
    {
        public Type EntityType { get => typeof(TEntity); }
    }
}
