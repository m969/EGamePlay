using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS
{
    /// <summary>
    /// </summary>
    public abstract class EcsObject
    {
        public static void Destroy(EcsObject ecsObject)
        {
            if (ecsObject is EcsEntity entity)
            {
                entity.InstanceId = 0;

                var children = entity.Id2Children.Values.ToArray();
                foreach (var item in children)
                {
                    Destroy(item);
                }

                var components = entity.Components.Values.ToArray();
                foreach (var item in components)
                {
                    entity.RemoveComponent(item.GetType());
                }

                entity.Parent.RemoveChild(entity);
            }

            if (ecsObject is EcsComponent component)
            {
                component.Entity.RemoveComponent(component.GetType());
            }
        }
    }
}