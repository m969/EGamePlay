using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class AbilityItemFollowSystem : AComponentSystem<AbilityItem, AbilityItemFollowComponent>,
        IAwake<AbilityItem, AbilityItemFollowComponent>
    {
        public void Awake(AbilityItem entity, AbilityItemFollowComponent component)
        {

        }

        public static void Update(AbilityItem abilityItem, AbilityItemFollowComponent component)
        {
            //AbilityItemPathMoveSystem.FollowMove(abilityItem);
        }
    }
}