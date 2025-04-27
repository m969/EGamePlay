using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class GameSystem : AEntitySystem<Game>,
IAwake<Game>,
IInit<Game>,
IUpdate<Game>
    {
        public void Awake(Game entity)
        {
            
        }

        public void Init(Game entity)
        {

        }

        public void Update(Game entity)
        {

        }

        public static Game Create(EcsNode ecsNode)
        {
            var game = ecsNode.AddChild<Game>();
            return game;
        }
    } 
}
