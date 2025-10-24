using ECS;
using ECSGame;
using ECSUnity;
using EGamePlay.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public enum GameType
    {
        ECSGame,
        TrueGameDemo,
        SimulationGameDemo,
    }

    public static class AppStatic
    {
        public static long NowMilliseconds { get; set; }
        public static float NowSeconds { get; set; }
        public static long DeltaTimeMilliseconds { get; set; }
        public static float DeltaTimeSeconds { get; set; }
        public static GameType GameType { get; set; } = GameType.ECSGame;
        public static Actor MyActor { get; set; }
        public static Actor OtherActor { get; set; }
        public static ConfigManageComponent Config { get; set; }
    }
}