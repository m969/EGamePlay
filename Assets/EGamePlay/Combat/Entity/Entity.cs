using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public class Entity
    {
        public Entity Parent { get; private set; }
        private Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        private List<Entity> Children { get; set; } = new List<Entity>();

        public void AddComponent<T>() where T : Component, new()
        {
            var c = new T();
            c.SetParent(this);
            this.Components.Add(typeof(T), c);
        }

        public void RemoveComponent<T>() where T : Component
        {
            this.Components.Remove(typeof(T));
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