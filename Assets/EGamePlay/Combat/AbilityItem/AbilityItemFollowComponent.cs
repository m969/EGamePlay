using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using ET;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemFollowComponent : Component
    {
        public long FollowInterval { get; set; } = 50;
        public long NextFollowTime { get; set; }


        public override void Awake()
        {
            //var abilityItem = GetEntity<AbilityItem>();
            //var moveComp = abilityItem.GetComponent<AbilityItemPathMoveComponent>();
            //moveComp.Speed = 5;
        }

        public override void Update()
        {
            var abilityItem = GetEntity<AbilityItem>();
            var moveComp = abilityItem.GetComponent<AbilityItemPathMoveComponent>();
            moveComp.FollowMove();
            //var now = TimeHelper.ClientNow();
            //if (now > NextFollowTime)
            //{
            //    NextFollowTime = now + FollowInterval;

            //    var abilityItem = GetEntity<AbilityItem>();
            //    var moveComp = abilityItem.GetComponent<AbilityItemPathMoveComponent>();
            //    moveComp.FollowMove();
            //}
        }
    }
}