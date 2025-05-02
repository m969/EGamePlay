using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using ECS;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemTargetCounterComponent : EcsComponent<AbilityItem>
    {
        public int TargetCounter { get; set; }
    }
}