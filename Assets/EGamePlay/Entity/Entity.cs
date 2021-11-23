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

    public class SetChildEventAfter { public Entity Entity; }
    public class RemoveChildEventAfter { public Entity Entity; }

    public abstract partial class Entity
    {
        public static MasterEntity Master => MasterEntity.Instance;
        public static bool EnableLog { get; set; } = false;

        private static Entity NewEntity(Type entityType)
        {
            var entity = Activator.CreateInstance(entityType) as Entity;
            entity.InstanceId = IdFactory.NewInstanceId();
            entity.Id = entity.InstanceId;
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

        private static T CreateWithParent<T>(Entity parent) where T : Entity
        {
            return CreateWithParent(typeof(T), parent) as T;
        }

        private static T CreateWithParent<T>(Entity parent, object initData) where T : Entity
        {
            return CreateWithParent(typeof(T), parent, initData) as T;
        }

        private static T CreateWithParentAndId<T>(Entity parent, long id) where T : Entity
        {
            return CreateWithParentAndId(typeof(T), parent, id) as T;
        }

        private static T CreateWithParentAndId<T>(Entity parent, long id, object initData) where T : Entity
        {
            return CreateWithParentAndId(typeof(T), parent, id, initData) as T;
        }

        private static void SetupEntity(Entity entity, Entity parent)
        {
            //var preParent = entity.Parent;
            parent.SetChild(entity);
            //if (preParent == null)
            {
                entity.Awake();
            }
            entity.Start();
        }

        private static void SetupEntity(Entity entity, Entity parent, object initData)
        {
            //var preParent = entity.Parent;
            parent.SetChild(entity);
            //if (preParent == null)
            {
                entity.Awake(initData);
            }
            entity.Start(initData);
        }

        public static Entity Create(Type entityType)
        {
            var entity = NewEntity(entityType);
            if (EnableLog) Log.Debug($"Create {entityType.Name}={entity.Id}");
            SetupEntity(entity, Master);
            return entity;
        }

        public static Entity Create(Type entityType, object initData)
        {
            var entity = NewEntity(entityType);
            if (EnableLog) Log.Debug($"Create {entityType.Name}={entity.Id}, {initData}");
            SetupEntity(entity, Master, initData);
            return entity;
        }

        public static Entity CreateWithId(Type entityType, long id)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"Create {entityType.Name}={entity.Id}");
            SetupEntity(entity, Master);
            return entity;
        }

        public static Entity CreateWithId(Type entityType, long id, object initData)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"Create {entityType.Name}={entity.Id}, {initData}");
            SetupEntity(entity, Master, initData);
            return entity;
        }

        private static Entity CreateWithParent(Type entityType, Entity parent)
        {
            var entity = NewEntity(entityType);
            if (EnableLog) Log.Debug($"CreateWithParent {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, parent);
            return entity;
        }

        private static Entity CreateWithParentAndId(Type entityType, Entity parent, long id)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"CreateWithParent {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, parent);
            return entity;
        }

        private static Entity CreateWithParent(Type entityType, Entity parent, object initData)
        {
            var entity = NewEntity(entityType);
            if (EnableLog) Log.Debug($"CreateWithParent {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, parent, initData);
            return entity;
        }

        private static Entity CreateWithParentAndId(Type entityType, Entity parent, long id, object initData)
        {
            var entity = NewEntity(entityType);
            entity.Id = id;
            if (EnableLog) Log.Debug($"CreateWithParent {parent.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, parent, initData);
            return entity;
        }

        public static void Destroy(Entity entity)
        {
            try
            {
                entity.OnDestroy();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            entity.Dispose();
        }

        public static void Destroy(Component entity)
        {
            try
            {
                entity.OnDestroy();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            entity.Dispose();
        }
    }

    public abstract partial class Entity : IDisposable
    {
        public long Id { get; set; }
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
#if !NOT_UNITY
                GetComponent<GameObjectComponent>().OnNameChanged(name);
#endif
            }
        }
        public long InstanceId { get; set; }
        private Entity parent;
        public Entity Parent { get { return parent; } }
        public bool IsDisposed { get { return InstanceId == 0; } }
        public List<Entity> Children { get; private set; } = new List<Entity>();
        public Dictionary<long, Entity> Id2Children { get; private set; } = new Dictionary<long, Entity>();
        public Dictionary<Type, List<Entity>> Type2Children { get; private set; } = new Dictionary<Type, List<Entity>>();
        public Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        //public Action<Component> OnAddComponentAction { get; set; }
        //public Action<Component> OnRemoveComponentAction { get; set; }
        //public Action<string> OnNameChangedAction { get; set; }
        //public Action<Entity> OnAddChildAction { get; set; }
        //public Action<Entity> OnRemoveChildAction { get; set; }


        public Entity()
        {
#if !NOT_UNITY
            if (this is MasterEntity) { }
            else if (this.GetType().Name.Contains("OnceWaitTimer")) { }
            else AddComponent<GameObjectComponent>();
#endif
        }

        public virtual void Awake()
        {

        }

        public virtual void Awake(object initData)
        {

        }

        public virtual void Start()
        {

        }

        public virtual void Start(object initData)
        {

        }

        public virtual void OnSetParent(Entity preParent, Entity nowParent)
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
            if (EnableLog) Log.Debug($"{GetType().Name}->Dispose");
            //var childrenComponent = GetComponent<ChildrenComponent>();
            //if (childrenComponent != null)
            //{
            //    var Children = childrenComponent.Children;
            //    var Type2Children = childrenComponent.Type2Children;
                if (Children.Count > 0)
                {
                    for (int i = Children.Count - 1; i >= 0; i--)
                    {
                        Destroy(Children[i]);
                    }
                    Children.Clear();
                    Type2Children.Clear();
                }
            //}

            Parent?.RemoveChild(this);
            foreach (Component component in Components.Values)
            {
                Destroy(component);
            }
            Components.Clear();
            InstanceId = 0;
            if (Master.Entities.ContainsKey(GetType()))
            {
                Master.Entities[GetType()].Remove(this);
            }
        }

        #region 组件
        public T GetParent<T>() where T : Entity
        {
            return parent as T;
        }

        public T AddComponent<T>() where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.Entity = this;
            component.IsDisposed = false;
            Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name}");
            component.Awake();
            component.Setup();
#if !NOT_UNITY
            GetComponent<GameObjectComponent>().OnAddComponent(component);
#endif
            //OnAddComponentAction?.Invoke((component));
            component.Enable = component.DefaultEnable;
            return component;
        }

        public T AddComponent<T>(object initData) where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.Entity = this;
            component.IsDisposed = false;
            Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name} initData={initData}");
            component.Awake(initData);
            component.Setup(initData);
#if !NOT_UNITY
            GetComponent<GameObjectComponent>().OnAddComponent(component);
#endif
            //OnAddComponentAction?.Invoke((component));
            component.Enable = component.DefaultEnable;
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            var component = Components[typeof(T)];
            if (component.Enable) component.Enable = false;
            Destroy(component);
            Components.Remove(typeof(T));
#if !NOT_UNITY
            GetComponent<GameObjectComponent>().OnRemoveComponent(component);
#endif
            //OnRemoveComponentAction?.Invoke((component));
        }

        public T GetComponent<T>() where T : Component
        {
            if (Components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return null;
        }
        #endregion

        #region 子实体
        private void SetParent(Entity parent)
        {
            var preParent = Parent;
            preParent?.RemoveChild(this);
            this.parent = parent;
#if !NOT_UNITY
            parent.GetComponent<GameObjectComponent>().OnAddChild(this);
#endif
            OnSetParent(preParent, parent);
            //parent?.SetChild(this);
        }

        public void SetChild(Entity child)
        {
            //var childrenComponent = GetComponent<ChildrenComponent>();
            //if (childrenComponent == null) childrenComponent = AddComponent<ChildrenComponent>();
            //childrenComponent.SetChild(child);

            Children.Add(child);
            Id2Children.Add(child.Id, child);
            if (!Type2Children.ContainsKey(child.GetType())) Type2Children.Add(child.GetType(), new List<Entity>());
            Type2Children[child.GetType()].Add(child);
            child.SetParent(this);
            //GetComponent<GameObjectComponent>().OnAddChild(child);
            //Fire("OnSetChildEvent", child);
        }

        public void RemoveChild(Entity child)
        {
            //var childrenComponent = GetComponent<ChildrenComponent>();
            //childrenComponent.RemoveChild(child);

            Children.Remove(child);
            if (Type2Children.ContainsKey(child.GetType())) Type2Children[child.GetType()].Remove(child);
            //child.SetParent(Master);
            //Fire("OnRemoveChildEvent", child);
        }

        public Entity AddChild(Type entityType)
        {
            return CreateWithParent(entityType, this);
        }

        public T AddChild<T>() where T : Entity
        {
            return CreateWithParent<T>(this);
        }

        public T AddIdChild<T>(long id) where T : Entity
        {
            return CreateWithParent<T>(this);
        }

        public T AddChild<T>(object initData) where T : Entity
        {
            return CreateWithParent<T>(this, initData);
        }

        public Entity GetIdChild(long id)
        {
            Id2Children.TryGetValue(id, out var entity);
            return entity;
        }

        public T GetIdChild<T>(long id) where T : Entity
        {
            Id2Children.TryGetValue(id, out var entity);
            return entity as T;
        }

        public T GetChild<T>(int index = 0) where T : Entity
        {
            //var childrenComponent = GetComponent<ChildrenComponent>();
            //if (childrenComponent == null)
            //{
            //    return null;
            //}
            if (Type2Children.ContainsKey(typeof(T)) == false)
            {
                return null;
            }
            if (Type2Children[typeof(T)].Count <= index)
            {
                return null;
            }
            return Type2Children[typeof(T)][index] as T;
        }

        //public Entity[] GetChildren()
        //{
        //    var childrenComponent = GetComponent<ChildrenComponent>();
        //    if (childrenComponent == null)
        //    {
        //        return new Entity[0];
        //    }
        //    return childrenComponent.GetChildren();
        //}

        //public T[] GetTypeChildren<T>() where T : Entity
        //{
        //    var childrenComponent = GetComponent<ChildrenComponent>();
        //    if (childrenComponent == null)
        //    {
        //        return new T[0];
        //    }
        //    return childrenComponent.GetTypeChildren<T>();
        //}

        public Entity Find(string name)
        {
            foreach (var item in Children)
            {
                if (item.name == name) return item;
            }
            return null;
        }

        public T Find<T>(string name) where T : Entity
        {
            if (Type2Children.TryGetValue(typeof(T), out var chidren))
            {
                foreach (var item in chidren)
                {
                    if (item.name == name) return item as T;
                }
            }
            return null;
        }
        #endregion

        #region 事件
        //public T ExecuteEvent<T>(T TEvent) where T : class
        //{
        //    var eventComponent = GetComponent<EventComponent>();
        //    if (eventComponent == null)
        //    {
        //        return TEvent;
        //    }
        //    eventComponent.Publish(TEvent);
        //    return TEvent;
        //}

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

        //public TEvent Publish<TEvent, TParam>(TEvent evnt, TParam param) where TEvent : class
        //{
        //    var eventComponent = GetComponent<EventComponent>();
        //    if (eventComponent == null)
        //    {
        //        return evnt;
        //    }
        //    eventComponent.Publish(evnt);
        //    return evnt;
        //}

        public SubscribeSubject Subscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            return eventComponent.Subscribe(action);
        }

        public SubscribeSubject Subscribe<T>(Action<T> action, Entity disposeWith) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            return eventComponent.Subscribe(action).DisposeWith(disposeWith);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.UnSubscribe(action);
            }
        }

        public void SendSignal(int signal)
        {

        }

        public void Fire(string eventType)
        {
            Fire(eventType, this);
        }

        public void Fire(string eventType, Entity entity)
        {

        }

        public void OnFire(string eventType, Action<Entity> action)
        {

        }

        public EventStream<T> OnEvent<T>() where T : class
        {
            return new EventStream<T>();
        }
        #endregion
    }
}