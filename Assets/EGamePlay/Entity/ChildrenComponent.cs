using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class ChildrenComponent : Component
    {
        public List<Entity> Children { get; private set; } = new List<Entity>();
        public Dictionary<Type, List<Entity>> Type2Children { get; private set; } = new Dictionary<Type, List<Entity>>();
        
        
        public Entity[] GetChildren()
        {
            return Children.ToArray();
        }

        public Entity[] GetTypeChildren<T>() where T : Entity
        {
            return Type2Children[typeof(T)].ToArray();
        }
    }
}