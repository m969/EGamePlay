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

namespace EGamePlay
{
    public class AbilityItemViewSystem : AEntitySystem<AbilityItem>,
    IAwake<AbilityItem>,
    IInit<AbilityItem>,
    IUpdate<AbilityItem>
    {
        public void Awake(AbilityItem entity)
        {
        }

        public void Init(AbilityItem entity)
        {
            entity.AddComponent<ModelViewComponent>();
            AbilityItemModelViewSystem.CreateAbilityItemProxyObj(entity);
        }

        public void Update(AbilityItem entity)
        {
            EntityViewSystem.Update(entity);
        }
    }

    public class AbilityItemModelViewSystem : AComponentSystem<AbilityItem, ModelViewComponent>,
        IAwake<AbilityItem, ModelViewComponent>,
        IDestroy<AbilityItem, ModelViewComponent>
    {
        public void Awake(AbilityItem entity, ModelViewComponent component)
        {

        }

        public void Destroy(AbilityItem entity, ModelViewComponent component)
        {

        }

        /// <summary>   创建技能碰撞体     </summary>
        public static GameObject CreateAbilityItemProxyObj(AbilityItem abilityItem)
        {
            var proxyObj = new GameObject("AbilityItemProxy");
            abilityItem.GetComponent<ModelViewComponent>().ModelTrans = proxyObj.transform;
            proxyObj.transform.position = abilityItem.Position;
            proxyObj.transform.rotation = abilityItem.Rotation;
            var executeComp = abilityItem.GetComponent<AbilityItemCollisionExecuteComponent>();
            var itemData = executeComp.CollisionExecuteData;
            var collisionData = AbilityItemCollisionExecuteSystem.GetItemEffect<CollisionEffect>(abilityItem);
            StaticClient.Game.Object2Items[proxyObj.gameObject] = abilityItem;

            if (collisionData.Shape == CollisionShape.Sphere)
            {
                proxyObj.AddComponent<SphereCollider>().enabled = false;
                proxyObj.GetComponent<SphereCollider>().isTrigger = true;
                proxyObj.GetComponent<SphereCollider>().radius = (float)collisionData.Radius;
            }
            if (collisionData.Shape == CollisionShape.Box)
            {
                proxyObj.AddComponent<BoxCollider>().enabled = false;
                proxyObj.GetComponent<BoxCollider>().isTrigger = true;
                proxyObj.GetComponent<BoxCollider>().center = collisionData.Center;
                proxyObj.GetComponent<BoxCollider>().size = collisionData.Size;
            }

            if (abilityItem.GetComponent<AbilityItemShieldComponent>() != null)
            {
                //var rigid = proxyObj.AddComponent<Rigidbody>();
                //rigid.isKinematic = true;
                //rigid.useGravity = false;
                //abilityItem.AddComponent<AbilityItemShieldViewComponent>();
            }
            else
            {
                proxyObj.AddComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) =>
                {
                    if (abilityItem.IsDisposed)
                    {
                        return;
                    }
                    var owner = abilityItem.AbilityEntity.OwnerEntity;
                    EcsEntity target = null;
                    if (StaticClient.Game.Object2Entities.TryGetValue(other.gameObject, out var otherEntity))
                    {
                        target = otherEntity;
                    }
                    else
                    {
                        if (StaticClient.Game.Object2Items.TryGetValue(other.gameObject, out var otherItem))
                        {
                            target = otherItem;
                        }
                    }

                    if (collisionData.ExecuteTargetType == CollisionExecuteTargetType.SelfGroup)
                    {
                        if (otherEntity.IsHero != owner.IsHero)
                        {
                            return;
                        }
                    }
                    if (collisionData.ExecuteTargetType == CollisionExecuteTargetType.EnemyGroup)
                    {
                        if (otherEntity.IsHero == owner.IsHero)
                        {
                            return;
                        }
                    }

                    if (owner.CollisionAbility.TryMakeAction(out var collisionAction))
                    {
                        collisionAction.Creator = owner;
                        collisionAction.CauseItem = abilityItem;
                        collisionAction.Target = target;
                        CollisionActionSystem.ApplyCollision(collisionAction);
                    }
                };
            }

            var collider = proxyObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            if (itemData.ObjAsset != null)
            {
                abilityItem.Name = itemData.ObjAsset.name;
                var effectObj = GameObject.Instantiate(itemData.ObjAsset, proxyObj.transform);
                effectObj.transform.localPosition = Vector3.zero;
                effectObj.transform.localRotation = UnityEngine.Quaternion.identity;
            }

            return proxyObj;
        }
    }
}