using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class GameObjectComponent : Component
    {
        public UnityEngine.GameObject GameObject { get; private set; }


        public override void Setup()
        {
            base.Setup();
            GameObject = new UnityEngine.GameObject(Entity.GetType().Name);
            var view = GameObject.AddComponent<ComponentView>();
            view.Type = GameObject.name;
            view.Component = this;
            //Entity.OnNameChangedAction = OnNameChanged;
            //Entity.OnAddComponentAction = OnAddComponent;
            //Entity.OnRemoveComponentAction = OnRemoveComponent;
            //Entity.Subscribe<SetChildEventAfter>(OnAddChild);
            //Entity.Subscribe<RemoveChildEventAfter>(OnRemoveChild);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.GameObject.Destroy(GameObject);
        }
        
        public void OnNameChanged(string name)
        {
            GameObject.name = $"{Entity.GetType().Name}: {name}";
        }

        public void OnAddComponent(Component component)
        {
            var view = GameObject.AddComponent<ComponentView>();
            view.Type = component.GetType().Name;
            view.Component = component;
        }

        public void OnRemoveComponent(Component component)
        {
            var comps = GameObject.GetComponents<ComponentView>();
            foreach (var item in comps)
            {
                if (item.Component == component)
                {
                    UnityEngine.GameObject.Destroy(item);
                }
            }
        }

        public void OnAddChild(Entity child)
        {
            if (child.GetComponent<GameObjectComponent>() != null)
            {
                child.GetComponent<GameObjectComponent>().GameObject.transform.SetParent(GameObject.transform);
            }
        }

        //public void OnRemoveChild(Entity child)
        //{
        //    if (child.GetComponent<GameObjectComponent>() != null)
        //    {
        //        child.GetComponent<GameObjectComponent>().GameObject.transform.SetParent(GameObject.transform);
        //    }
        //}
    }
}