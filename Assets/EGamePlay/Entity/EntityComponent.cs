using System;

namespace EGamePlay
{
    public class EntityComponent<TEntity> : Component where TEntity : Entity
    {
        public TEntity OwnerEntity { get => Entity as TEntity; }
    }
}