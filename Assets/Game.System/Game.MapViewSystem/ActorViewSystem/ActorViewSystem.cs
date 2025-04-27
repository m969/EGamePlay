using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class ActorViewSystem : AEntitySystem<Actor>,
IAwake<Actor>,
IInit<Actor>,
IUpdate<Actor>
    {
        public void Awake(Actor entity)
        {
        }

        public void Init(Actor entity)
        {

        }

        public void Update(Actor entity)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } component)
            {
                ModelViewSystem.Update(entity, component);
            }
        }
    } 
}
