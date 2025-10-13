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
    public class AbilityItemCollisionExecuteComponent : EcsComponent<AbilityItem>
    {
        public ExecuteClipData ExecuteClipData { get; set; }
        public ItemExecute CollisionExecuteData => ExecuteClipData.ItemData;
    }
}