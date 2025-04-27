using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class Handler_EntityUpdateCmd : ACommandHandler<EntityUpdateCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, EntityUpdateCmd cmd)
        {
            var entity = cmd.Entity;
            if (entity.GetComponent<ModelViewComponent>() is { } viewComp && viewComp.ModelTrans != null)
            {
                if (cmd.ChangeComponent is TransformComponent transformComponent)
                {
                    if (entity is Actor)
                    {
                        viewComp.ModelTrans.GetChild(0).transform.rotation = transformComponent.Rotation;
                    }
                    else
                    {
                        viewComp.ModelTrans.transform.rotation = transformComponent.Rotation;
                    }
                }

                //if (cmd.ChangeComponent is MoveComponent moveComponent)
                //{
                //    viewComp.ViewObj.transform.position = cmd.Entity.GetComponent<TransformComponent>().Position.ToVector();
                //}
            }
        }
    }
}
