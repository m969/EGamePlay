using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using ET;
using ECS;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemFollowComponent : EcsComponent<AbilityItem>
    {
        public long FollowInterval { get; set; } = 50;
        public long NextFollowTime { get; set; }
    }
}