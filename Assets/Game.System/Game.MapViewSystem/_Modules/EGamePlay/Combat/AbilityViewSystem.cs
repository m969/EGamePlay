using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System.Net;
using System;
using UnityEngine.UIElements;
using ECSGame;
using ECSUnity;
using ET;

namespace EGamePlay
{
    public class AbilityViewControlSystem : AEntitySystem<Ability>,
    IAwake<Ability>,
    IInit<Ability>,
    IUpdate<Ability>
    {
        public void Awake(Ability entity)
        {
        }

        public void Init(Ability entity)
        {
            entity.AddComponent<ModelViewComponent>();
        }

        public void Update(Ability entity)
        {
            //if (entity.GetComponent<EntityViewComponent>() is { } component)
            //{
            //    EntityViewSystem.Update(entity, component);
            //}
        }
    }

    public class AbilityViewSystem : AComponentSystem<Ability, ModelViewComponent>,
        IAwake<Ability, ModelViewComponent>
    {
        public void Awake(Ability entity, ModelViewComponent component)
        {
            //ConsoleLog.Debug($"AbilityViewSystem Awake {entity.Config.KeyName}");
            if (entity.Config.Type == "Buff")
            {
                var abilityConfig = StaticConfig.Config.Get<AbilityConfig>(entity.Config.Id);
                var keyName = abilityConfig.KeyName;

                var combatEntity = entity.GetParent<CombatEntity>();

                if (keyName == "Vertigo")
                {
                    AnimationSystem.Play(combatEntity.Actor, combatEntity.GetComponent<ECSGame.AnimationComponent>().StunAnimation);

                    var vertigoParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Vertigo"));
                    vertigoParticle.transform.parent = combatEntity.GetComponent<ModelViewComponent>().ModelTrans.transform;
                    vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
                    entity.GetComponent<ModelViewComponent>().ModelTrans = vertigoParticle.transform;
                }

                if (keyName == "Weak")
                {
                    var weakParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Weak"));
                    weakParticle.transform.parent = combatEntity.GetComponent<ModelViewComponent>().ModelTrans.transform;
                    weakParticle.transform.localPosition = new Vector3(0, 0, 0);
                    entity.GetComponent<ModelViewComponent>().ModelTrans = weakParticle.transform;
                }
            }
        }
    }
}