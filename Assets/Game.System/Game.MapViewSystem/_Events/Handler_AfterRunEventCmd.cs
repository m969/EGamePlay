using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class Handler_AfterRunEventCmd : ACommandHandler<AfterRunEventCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, AfterRunEventCmd cmd)
        {
            //if (cmd.EventRun is FireEvent)
            //{
            //    SoundSystem.PlayClip(SoundType.OnceFire);
            //}
            //if (cmd.EventRun is CollisionEvent)
            //{
            //    var entity1 = cmd.EventEntity;
            //    var entity2 = (EcsEntity)cmd.EventArgs2;
            //    if (entity1 is Item)
            //    {
            //        SoundSystem.PlayClip(SoundType.Explosion);
            //    }
            //    if (entity2 is Item)
            //    {
            //        SoundSystem.PlayClip(SoundType.Explosion);
            //    }
            //}
        }
    }
}
