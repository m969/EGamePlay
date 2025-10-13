using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public abstract class AComponentSystem<TEntity, TComponent> : IEcsComponentSystem where TEntity : EcsEntity where TComponent : EcsComponent
    {
        public Type EntityType { get => typeof(TEntity); }
        public Type ComponentType { get => typeof(TComponent); }
    }
}
