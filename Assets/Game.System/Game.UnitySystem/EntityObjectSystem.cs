using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using Animancer;
using System.ComponentModel;

namespace ECSGame
{
    public class EntityObjectSystem : AComponentSystem<EcsEntity, EntityObjectComponent>,
IAwake<EcsEntity, EntityObjectComponent>,
IDestroy<EcsEntity, EntityObjectComponent>
    {
        public void Awake(EcsEntity entity, EntityObjectComponent component)
        {
            var go = new GameObject($"{entity.GetType().Name}(Id:{entity.Id})");
            if (entity.Parent != null)
            {
                var parentObj = entity.Parent.GetComponent<EntityObjectComponent>().EntityObject as GameObject;
                go.transform.SetParent(parentObj.transform);
            }
            component.EntityObject = go;
            var componentView = go.AddComponent<ComponentView>();
            componentView.Type = entity.GetType().FullName;
            componentView.Component = entity;
        }

        public void Destroy(EcsEntity entity, EntityObjectComponent component)
        {
            var go = component.EntityObject as GameObject;
            GameObject.Destroy(go);
        }
    }

    public class ComponentObjectSystem : AEntitySystem<EcsEntity>,
IOnAddComponent<EcsEntity>,
IOnRemoveComponent<EcsEntity>
    {
        public void OnAddComponent(EcsEntity entity, EcsComponent component)
        {
            if (entity.GetComponent<EntityObjectComponent>() == null || entity.GetComponent<EntityObjectComponent>().EntityObject == null)
            {
                return;
            }
            var go = entity.GetComponent<EntityObjectComponent>().EntityObject as GameObject;
            var componentView = go.AddComponent<ComponentView>();
            componentView.Type = component.GetType().FullName;
            componentView.Component = component;
        }

        public void OnRemoveComponent(EcsEntity entity, EcsComponent component)
        {
            if (entity.GetComponent<EntityObjectComponent>() == null || entity.GetComponent<EntityObjectComponent>().EntityObject == null)
            {
                return;
            }
            var go = entity.GetComponent<EntityObjectComponent>().EntityObject as GameObject;
            GameObject.Destroy(go);
        }
    }
}
