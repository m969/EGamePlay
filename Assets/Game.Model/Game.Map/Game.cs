using ECS;
using ECSGame;
using EGamePlay.Combat;
using System;

namespace ECSGame
{
    public class Game : EcsEntity
    {
        public CombatContext CombatContext { get; set; }
        public Actor MyActor { get; set; }
        public Actor OtherActor { get; set; }
    }
}
