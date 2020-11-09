using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public abstract class Entity
    {
        public static Dictionary<Type, List<Entity>> Entities = new Dictionary<Type, List<Entity>>();
        public Entity Parent { get; private set; }
        public Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        private List<Entity> Children { get; set; } = new List<Entity>();
        private Dictionary<Type, List<Entity>> Type2Children { get; set; } = new Dictionary<Type, List<Entity>>();


        public static T Create<T>()where T : Entity, new()
        {
            var entity = new T();
            if (!Entities.ContainsKey(typeof(T)))
            {
                Entities.Add(typeof(T), new List<Entity>());
            }
            Entities[typeof(T)].Add(entity);
            return entity;
        }

        public static void Destroy(Entity entity)
        {

        }

        public virtual void Awake()
        {

        }

        public virtual void Destroy()
        {

        }

        public T AddComponent<T>() where T : Component, new()
        {
            var c = new T();
            c.SetParent(this);
            this.Components.Add(typeof(T), c);
            return c;
        }

        public void RemoveComponent<T>() where T : Component
        {
            this.Components.Remove(typeof(T));
        }

        public T GetComponent<T>()where T : Component
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
            Parent = parent;
            Parent?.AddChild(this);
        }

        public void AddChild(Entity child)
        {
            Children.Add(child);
            if (!Type2Children.ContainsKey(child.GetType()))
            {
                Type2Children.Add(child.GetType(), new List<Entity>());
            }
            Type2Children[child.GetType()].Add(child);
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
        }

        public Entity[] GetChildren()
        {
            return Children.ToArray();
        }

        public Entity[] GetTypeChildren<T>() where T : Entity
        {
            return Type2Children[typeof(T)].ToArray();
        }
    }
}