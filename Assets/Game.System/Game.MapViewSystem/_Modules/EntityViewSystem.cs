using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class EntityViewSystem : AEntitySystem<EcsEntity>,
        IAwake<EcsEntity>,
        IInit<EcsEntity>
    {
        public void Awake(EcsEntity entity)
        {
        }

        public void Init(EcsEntity entity)
        {

        }

        public static void Update(EcsEntity entity)
        {

        }
    } 
}
