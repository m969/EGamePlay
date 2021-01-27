﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameUtils;

namespace EGamePlay
{
    public abstract partial class Entity
    {
        public static MasterEntity Master => MasterEntity.Instance;
        public static bool DebugLog { get; set; } = false;

        private static T New<T>() where T : Entity, new()
        {
            var entity = new T();
            entity.InstanceId = IdFactory.NewInstanceId();
            if (!Master.Entities.ContainsKey(typeof(T)))
            {
                Master.Entities.Add(typeof(T), new List<Entity>());
            }
            Master.Entities[typeof(T)].Add(entity);
            return entity;
        }

        public static T Create<T>() where T : Entity, new()
        {
            var entity = New<T>();
            entity.Id = entity.InstanceId;
            Master.AddChild(entity);
            entity.Awake();
            if (DebugLog) Log.Debug($"EntityFactory->Create, {typeof(T).Name}={entity.InstanceId}");
            return entity;
        }

        public static T Create<T>(object initData) where T : Entity, new()
        {
            var entity = New<T>();
            entity.Id = entity.InstanceId;
            Master.AddChild(entity);
            entity.Awake(initData);
            if (DebugLog) Log.Debug($"EntityFactory->Create, {typeof(T).Name}={entity.InstanceId}, {initData}");
            return entity;
        }

        public static T CreateWithParent<T>(Entity parent) where T : Entity, new()
        {
            var entity = New<T>();
            entity.Id = entity.InstanceId;
            parent.AddChild(entity);
            entity.Awake();
            if (DebugLog) Log.Debug($"EntityFactory->CreateWithParent, {parent.GetType().Name}, {typeof(T).Name}={entity.InstanceId}");
            return entity;
        }

        public static T CreateWithParent<T>(Entity parent, object initData) where T : Entity, new()
        {
            var entity = New<T>();
            entity.Id = entity.InstanceId;
            parent.AddChild(entity);
            entity.Awake(initData);
            if (DebugLog) Log.Debug($"EntityFactory->CreateWithParent, {parent.GetType().Name}, {typeof(T).Name}={entity.InstanceId}");
            return entity;
        }

        public static void Destroy(Entity entity)
        {
            entity.OnDestroy();
            entity.Dispose();
        }
    }
    public abstract partial class Entity : IDisposable
    {
#if !SERVER
        public UnityEngine.GameObject GameObject { get; set; }
#endif
        public long Id { get; set; }
        private string name;
        public string Name
        {
            get => name;
            set
            {
#if !SERVER
                GameObject.name = $"{GetType().Name}: {value}";
#endif
                name = value;
            }
        }
        public long InstanceId { get; set; }
        private Entity parent;
        public Entity Parent { get { return parent; } private set { parent = value; OnSetParent(value); } }
        public bool IsDisposed { get { return InstanceId == 0; } }
        public Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        private List<Entity> Children { get; set; } = new List<Entity>();
        private Dictionary<Type, List<Entity>> Type2Children { get; set; } = new Dictionary<Type, List<Entity>>();


        public Entity()
        {
#if !SERVER
            GameObject = new UnityEngine.GameObject(GetType().Name);
            var view = GameObject.AddComponent<ET.ComponentView>();
            view.Type = GameObject.name;
            view.Component = this;
#endif
        }

        public virtual void Awake()
        {

        }

        public virtual void Awake(object initData)
        {

        }

        public virtual void OnDestroy()
        {

        }

        public void Dispose()
        {
            if (Entity.DebugLog) Log.Debug($"{GetType().Name}->Dispose");
            if (Children.Count > 0)
            {
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    Entity.Destroy(Children[i]);
                }
                Children.Clear();
                Type2Children.Clear();
            }

            foreach (Component component in this.Components.Values)
            {
                component.OnDestroy();
                component.Dispose();
            }
            this.Components.Clear();
            Parent?.RemoveChild(this);
            InstanceId = 0;
            if (Master.Entities.ContainsKey(GetType()))
            {
                Master.Entities[GetType()].Remove(this);
            }
#if !SERVER
            UnityEngine.GameObject.Destroy(GameObject);
#endif
        }

        public virtual void OnSetParent(Entity parent)
        {

        }

        public T GetParent<T>() where T : Entity
        {
            return parent as T;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            component.Entity = this;
            component.IsDisposed = false;
            //component.Enable = true;
            this.Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (Entity.DebugLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name}");
            component.Setup();
#if !SERVER
            var view = GameObject.AddComponent<ET.ComponentView>();
            view.Type = typeof(T).Name;
            view.Component = component;
#endif
            return component;
        }

        public T AddComponent<T>(object initData) where T : Component, new()
        {
            var component = new T();
            component.Entity = this;
            component.IsDisposed = false;
            //component.Enable = true;
            this.Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (Entity.DebugLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name} initData={initData}");
            component.Setup(initData);
#if !SERVER
            var view = GameObject.AddComponent<ET.ComponentView>();
            view.Type = typeof(T).Name;
            view.Component = component;
#endif
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            this.Components[typeof(T)].OnDestroy();
            this.Components[typeof(T)].Dispose();
            this.Components.Remove(typeof(T));
        }

        public T GetComponent<T>() where T : Component
        {
            if (this.Components.TryGetValue(typeof(T),  out var component))
            {
                return component as T;
            }
            return null;
        }

        public void SetParent(Entity parent)
        {
            Parent?.RemoveChild(this);
            parent?.AddChild(this);
        }

        public void AddChild(Entity child)
        {
            Children.Add(child);
            if (!Type2Children.ContainsKey(child.GetType()))
            {
                Type2Children.Add(child.GetType(), new List<Entity>());
            }
            Type2Children[child.GetType()].Add(child);
            child.Parent = this;
#if !SERVER
            child.GameObject.transform.SetParent(GameObject.transform);
#endif
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
            if (Type2Children.ContainsKey(child.GetType()))
            {
                Type2Children[child.GetType()].Remove(child);
            }
            child.Parent = null;
#if !SERVER
            child.GameObject.transform.SetParent(null);
#endif
        }

        public Entity[] GetChildren()
        {
            return Children.ToArray();
        }

        public Entity[] GetTypeChildren<T>() where T : Entity
        {
            return Type2Children[typeof(T)].ToArray();
        }

        public T Publish<T>(T TEvent) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                return TEvent;
            }
            eventComponent.Publish(TEvent);
            return TEvent;
        }

        public EventSubscribe<T> Subscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            return eventComponent.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.UnSubscribe(action);
            }
        }
    }
}