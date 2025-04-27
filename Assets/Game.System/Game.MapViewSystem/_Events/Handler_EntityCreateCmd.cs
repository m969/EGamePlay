using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using EGamePlay.Combat;
using EGamePlay;

namespace ECSGame
{
    public class Handler_EntityCreateCmd : ACommandHandler<EntityCreateCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, EntityCreateCmd cmd)
        {
            var entity = cmd.Entity;
            //entity.AddComponent<EntityViewComponent>();
            if (entity is Item)
            {
                //EntityViewSystem.SetScale(cmd.Entity, TSVector.one * 0.2f);
            }
            if (entity is Ability { Config:{ Type: "Buff" } } ability)
            {
                var abilityConfig = StaticConfig.Config.Get<AbilityConfig>(ability.Config.Id);
                var keyName = abilityConfig.KeyName;

                if (keyName == "Vertigo")
                {
                    //AnimationComponent.Play(AnimationComponent.StunAnimation);
                    //if (vertigoParticle == null)
                    //{
                    //    vertigoParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Vertigo"));
                    //    vertigoParticle.transform.parent = transform;
                    //    vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
                    //}
                }
            }
        }
    }
}
