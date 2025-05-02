using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class ItemViewSystem : AEntitySystem<Item>,
IAwake<Item>,
IInit<Item>,
IUpdate<Item>
    {
        public void Awake(Item entity)
        {
        }

        public void Init(Item entity)
        {

        }

        public void Update(Item entity)
        {
            EntityViewSystem.Update(entity);
        }
    } 
}
