using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using EGamePlay;
using EGamePlay.Combat;

namespace ECSGame
{
    public class Handler_AfterCauseApplyCmd : ACommandHandler<AfterCauseApplyCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, AfterCauseApplyCmd cmd)
        {
            if (cmd.Action is DamageAction damageAction)
            {

            }
        }
    }
}
