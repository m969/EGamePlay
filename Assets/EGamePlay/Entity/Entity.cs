using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameUtils;

namespace EGamePlay
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class EnableUpdateAttribute : Attribute
    {
        public EnableUpdateAttribute()
        {
        }
    }

    public abstract partial class Entity
    {
        public static MasterEntity Master => MasterEntity.Instance;
        public static bool EnableLog { get; set; } = false;

        private static Entity NewEntity(Type entityType)
        {
            var entity = Activator.CreateInstance(entityType) as Entity;
            entity.InstanceId = IdFactory.NewInstanceId();
            if (!Master.Entities.ContainsKey(entityType))
            {
                Master.Entities.Add(entityType, new List<Entity>());
            }
            Master.Entities[entityType].Add(entity);
            return entity;
        }

        public static T Create<T>() where T : Entity
        {
            return Create(typeof(T)) as T;
        }

        public static T Create<T>(object initData) where T : Entity
        {
            return Create(typeof(T), initData) as T;
        }

        public static T CreateWithId<T>(long id) where T : Entity
        {
            return CreateWithId(typeof(T), id) as T;
        }

        public static T CreateWithId<T>(long id, object initData) where T : Entity
        {
            return CreateWithId(typeof(T), id, initData) as T;
        }

        public static T CreateWithParent<T>(Entity parent) where T : Entity
        {
            return CreateWithParent(typeof(T), parent) as T;
        }

        public static T CreateWithParent<T>(Entity parent, object initData) where T : Entity
        {
            return CreateWithParent(typeof(T), parent, initData) as T;
        }

        public static T CreateWithParentAndId<T>(Entity parent, long id) where T : Entity
        {
            return CreateWithParentAndId(typeof(T), parent, id) as T;
        }

        public static T CreateWithParentAndId<T>(Entity parent, long id, object initData) where T : Entity
        {
            return CreateWithParentAndId(typeof(T), parent, id, initData) as T;
        }

        public static Entity Create(Type entityType)
        {
            var entity = NewEntity(entityType);
            entity.Id = entity.InstanceId;
            if (EnableLog) Log.Debug($"Entity->Create, {entityType.Name}={entity.Id}");
            Master.SetChild(entity);
            entity.Awake();
            return entity;
        }

        public static Entity Create(Type entityType, object initData)
        {
            var entity = NewEntity(entityType);
            entity.Id = entity.InstanceId;
            if (EnableLog) Log.Debug($"Entity->Create, {entityType.Name}={entity.Id}, {initData}");
            Master.SetChild(entity);
            entity.Awake(initData);
            return entity;
        }

        public static Entity CreateWithId(Type entityType, long id)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"Entity->Create, {entityType.Name}={entity.Id}");
            Master.SetChild(entity);
            entity.Awake();
            return entity;
        }

        public static Entity CreateWithId(Type entityType, long id, object initData)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"Entity->Create, {entityType.Name}={entity.Id}, {initData}");
            Master.SetChild(entity);
            entity.Awake(initData);
            return entity;
        }

        public static Entity CreateWithParent(Type entityType, Entity parent)
        {
            var entity = NewEntity(entityType);
            entity.Id = entity.InstanceId;
            if (EnableLog) Log.Debug($"Entity->CreateWithParent, {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            parent.SetChild(entity);
            entity.Awake();
            return entity;
        }

        public static Entity CreateWithParentAndId(Type entityType, Entity parent, long id)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"Entity->CreateWithParent, {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            parent.SetChild(entity);
            entity.Awake();
            return entity;
        }

        public static Entity CreateWithParent(Type entityType, Entity parent, object initData)
        {
            var entity = NewEntity(entityType);
            entity.Id = entity.InstanceId;
            if (EnableLog) Log.Debug($"Entity->CreateWithParent, {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            parent.SetChild(entity);
            entity.Awake(initData);
            return entity;
        }

        public static Entity CreateWithParentAndId(Type entityType, Entity parent, long id, object initData)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"Entity->CreateWithParent, {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            parent.SetChild(entity);
            entity.Awake(initData);
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
        public long Id { get; set; }
        private string name;
        public virtual string Name
        {
            get => name;
            set
            {
                name = value;
                OnNameChangedAction?.Invoke((name));
            }
        }
        public long InstanceId { get; set; }
        private Entity parent;
        public Entity Parent { get { return parent; } private set { parent = value; OnSetParent(value); } }
        public bool IsDisposed { get { return InstanceId == 0; } }
        public Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        public Action<string> OnNameChangedAction { get; set; }
        public Action<Component> OnAddComponentAction { get; set; }
        public Action<Component> OnRemoveComponentAction { get; set; }
        public Action<Entity> OnAddChildAction { get; set; }
        public Action<Entity> OnRemoveChildAction { get; set; }


        public Entity()
        {
#if !NOT_UNITY
            if (this is MasterEntity) { }
            else AddComponent<GameObjectComponent>();
#endif
        }

        public virtual void Awake()
        {

        }

        public virtual void Awake(object initData)
        {

        }

        public virtual void Update()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public void Dispose()
        {
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->Dispose");
            var childrenComponent = GetComponent<ChildrenComponent>();
            if (childrenComponent != null)
            {
                var Children = childrenComponent.Children;
                var Type2Children = childrenComponent.Type2Children;
                if (Children.Count > 0)
                {
                    for (int i = Children.Count - 1; i >= 0; i--)
                    {
                        Entity.Destroy(Children[i]);
                    }
                    Children.Clear();
                    Type2Children.Clear();
                }
            }

            Parent?.RemoveChild(this);
            foreach (Component component in this.Components.Values)
            {
                component.OnDestroy();
                component.Dispose();
            }
            this.Components.Clear();
            InstanceId = 0;
            if (Master.Entities.ContainsKey(GetType()))
            {
                Master.Entities[GetType()].Remove(this);
            }
        }

        public virtual void OnSetParent(Entity parent)
        {

        }

        public T GetParent<T>() where T : Entity
        {
            return parent as T;
        }

        public T AddComponent<T>() where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.Entity = this;
            component.IsDisposed = false;
            component.Enable = true;
            this.Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name}");
            component.Setup();
            OnAddComponentAction?.Invoke((component));
            return component;
        }

        public T AddComponent<T>(object initData) where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.Entity = this;
            component.IsDisposed = false;
            component.Enable = true;
            this.Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name} initData={initData}");
            component.Setup(initData);
            OnAddComponentAction?.Invoke((component));
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            var component = this.Components[typeof(T)];
            component.OnDestroy();
            component.Dispose();
            this.Components.Remove(typeof(T));
            OnRemoveComponentAction?.Invoke((component));
        }

        public T GetComponent<T>() where T : Component
        {
            if (this.Components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return null;
        }

        public void SetParent(Entity parent)
        {
            Parent?.RemoveChild(this);
            parent?.SetChild(this);
        }

        public void SetChild(Entity child)
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
            if (childrenComponent == null) childrenComponent = AddComponent<ChildrenComponent>();

            var Children = childrenComponent.Children;
            var Type2Children = childrenComponent.Type2Children;
            Children.Add(child);
            if (!Type2Children.ContainsKey(child.GetType())) Type2Children.Add(child.GetType(), new List<Entity>());
            Type2Children[child.GetType()].Add(child);
            child.Parent = this;
            OnAddChildAction?.Invoke(child);
        }

        public void RemoveChild(Entity child)
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
            var Children = childrenComponent.Children;
            var Type2Children = childrenComponent.Type2Children;
            Children.Remove(child);
            if (Type2Children.ContainsKey(child.GetType())) Type2Children[child.GetType()].Remove(child);
            child.Parent = Master;
            OnRemoveChildAction?.Invoke(child);
        }

        public Entity AddChild(Type entityType)
        {
            return CreateWithParent(entityType, this);
        }

        public T AddChild<T>() where T : Entity
        {
            return CreateWithParent<T>(this);
        }

        public T AddChildWithId<T>(long id) where T : Entity
        {
            return CreateWithParent<T>(this);
        }

        public T AddChild<T>(object initData) where T : Entity
        {
            return CreateWithParent<T>(this, initData);
        }

        public Entity[] GetChildren()
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                return new Entity[0];
            }
            return childrenComponent.GetChildren();
        }

        public Entity[] GetTypeChildren<T>() where T : Entity
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                return new Entity[0];
            }
            return childrenComponent.GetTypeChildren<T>();
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

        public TEvent Publish<TEvent, TParam>(TEvent evnt, TParam param) where TEvent : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                return evnt;
            }
            eventComponent.Publish(evnt);
            return evnt;
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            eventComponent.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.UnSubscribe(action);
            }
        }

        public void Fire(string signal)
        {

        }
    }
}