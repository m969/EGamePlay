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
    public class EcsType
    {
        /// <summary>
        /// 游戏主流程
        /// </summary>
        public const ushort Game = 1;

        /// <summary>
        /// UI
        /// </summary>
        public const ushort UI = 2;

        /// <summary>
        /// 声音
        /// </summary>
        public const ushort Sound = 3;

        /// <summary>
        /// 玩家输入
        /// </summary>
        public const ushort PlayerInput = 4;

        /// <summary>
        /// 帧同步地图世界
        /// </summary>
        public const ushort TrueWorld = 5;

        /// <summary>
        /// 游戏地图世界
        /// </summary>
        public const ushort World = 6;

        /// <summary>
        /// 玩家实体
        /// </summary>
        public const ushort Player = 7;
    }

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