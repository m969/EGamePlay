using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class Handler_EntityDestroyCmd : ACommandHandler<EntityDestroyCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, EntityDestroyCmd cmd)
        {
            var entity = cmd.Entity;
        }
    }
}
