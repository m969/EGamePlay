using ECS;
using ECSGame;
using ECSUnity;
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

    public static class EcsDomain
    {
        public static Dictionary<ushort, EcsNode> EcsNodes { get; set; } = new();
        public static Dictionary<Type, List<IEventRun>> EventHandlers { get; set; } = new();

        public static Game Game { get; set; }
        public static World World { get; set; }
        // public static TrueWorld TrueWorld { get; set; }
        // public static UIStage UIStage { get; set; }
        // public static SoundMaster SoundMaster { get; set; }
        // public static PlayerInput PlayerInput { get; set; }
        // public static Player Player { get; set; }


        public static void AddNode(EcsNode node)
        {
            EcsNodes.Add(node.EcsTypeId, node);
        }

        public static EcsNode GetNode(ushort typeId)
        {
            EcsNodes.TryGetValue(typeId, out var ecsNode);
            return ecsNode;
        }
    }
}