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
            Entity.OnNameChangedAction = OnNameChanged;
            Entity.OnAddComponentAction = OnAddComponent;
            Entity.OnRemoveComponentAction = OnRemoveComponent;
            Entity.Subscribe<SetChildEventAfter>(OnAddChild);
            Entity.Subscribe<RemoveChildEventAfter>(OnRemoveChild);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.GameObject.Destroy(GameObject);
        }
        
        private void OnNameChanged(string name)
        {
            GameObject.name = $"{Entity.GetType().Name}: {name}";
        }

        private void OnAddComponent(Component component)
        {
            var view = GameObject.AddComponent<ComponentView>();
            view.Type = component.GetType().Name;
            view.Component = component;
        }
        
        private void OnRemoveComponent(Component component)
        {
            UnityEngine.GameObject.Destroy(GameObject.GetComponent<ComponentView>());
        }
        
        private void OnAddChild(SetChildEventAfter evnt)
        {
            if (evnt.Entity.GetComponent<GameObjectComponent>() != null)
            {
                evnt.Entity.GetComponent<GameObjectComponent>().GameObject.transform.SetParent(GameObject.transform);
            }
        }
        
        private void OnRemoveChild(RemoveChildEventAfter evnt)
        {
            if (evnt.Entity.GetComponent<GameObjectComponent>() != null)
            {
                evnt.Entity.GetComponent<GameObjectComponent>().GameObject.transform.SetParent(null);
            }
        }
    }
}