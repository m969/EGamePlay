using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public abstract class Entity
    {
        private static Dictionary<Type, List<Entity>> Entities = new Dictionary<Type, List<Entity>>();
        public Entity Parent { get; private set; }
        private Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        private List<Entity> Children { get; set; } = new List<Entity>();


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
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
        }
    }
}