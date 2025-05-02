using ECS;
using ECSGame;
using EGamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class Game : EcsEntity
    {
        //public CombatContext CombatContext { get; set; }
        public Actor MyActor { get; set; }
        public Actor OtherActor { get; set; }
#if !SERVER
        public Dictionary<GameObject, CombatEntity> Object2Entities { get; set; } = new Dictionary<GameObject, CombatEntity>();
        public Dictionary<GameObject, AbilityItem> Object2Items { get; set; } = new Dictionary<GameObject, AbilityItem>();
#endif
    }
}
