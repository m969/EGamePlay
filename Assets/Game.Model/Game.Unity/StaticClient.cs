using ECS;
using ECSGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EGamePlay.Combat;

namespace ECSUnity
{
    public static class StaticClient
    {
        public static EcsNode EcsNode { get; set; }
        public static Game Game { get; set; }
        public static CombatContext Context { get; set; }
        public static ReferenceCollector ConfigsCollector { get; set; }
        public static ReferenceCollector PrefabsCollector { get; set; }
    }
}