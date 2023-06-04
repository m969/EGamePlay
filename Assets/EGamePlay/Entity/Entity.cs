using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameUtils;

namespace EGamePlay
{
    public abstract partial class Entity
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

        private void Dispose()
        {
            if (EnableLog) Log.Debug($"{GetType().Name}->Dispose");
            if (Children.Count > 0)
            {
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    Destroy(Children[i]);
                }
                Children.Clear();
                Type2Children.Clear();
            }

            Parent?.RemoveChild(this);
            foreach (var component in Components.Values)
            {
                component.Enable = false;
                Component.Destroy(component);
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

        public T As<T>() where T : class
        {
            return this as T;
        }

        public bool As<T>(out T entity) where T : Entity
        {
            entity = this as T;
            return entity != null;
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

        public T Add<T>() where T : Component
        {
            return AddComponent<T>();
        }

        public T Add<T>(object initData) where T : Component
        {
            return AddComponent<T>(initData);
        }

        public void RemoveComponent<T>() where T : Component
        {
            var component = Components[typeof(T)];
            if (component.Enable) component.Enable = false;
            Component.Destroy(component);
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

        public bool HasComponent<T>() where T : Component
        {
            return Components.TryGetValue(typeof(T), out var component);
        }

        public Component GetComponent(Type componentType)
        {
            if (this.Components.TryGetValue(componentType, out var component))
            {
                return component;
            }
            return null;
        }

        //public T GetComponent<T>() where T : Component
        //{
        //    if (Components.TryGetValue(typeof(T), out var component))
        //    {
        //        return component as T;
        //    }
        //    return null;
        //}

        //public Component Get(Type componentType)
        //{
        //    if (this.Components.TryGetValue(componentType, out var component))
        //    {
        //        return component;
        //    }
        //    return null;
        //}

        public bool TryGet<T>(out T component) where T : Component
        {
            if (Components.TryGetValue(typeof(T), out var c))
            {
                component = c as T;
                return true;
            }
            component = null;
            return false;
        }

        public bool TryGet<T, T1>(out T component, out T1 component1) where T : Component  where T1 : Component
        {
            component = null;
            component1 = null;
            if (Components.TryGetValue(typeof(T), out var c)) component = c as T;
            if (Components.TryGetValue(typeof(T1), out var c1)) component1 = c1 as T1;
            if (component != null && component1 != null) return true;
            return false;
        }

        public bool TryGet<T, T1, T2>(out T component, out T1 component1, out T2 component2) where T : Component where T1 : Component where T2 : Component
        {
            component = null;
            component1 = null;
            component2 = null;
            if (Components.TryGetValue(typeof(T), out var c)) component = c as T;
            if (Components.TryGetValue(typeof(T1), out var c1)) component1 = c1 as T1;
            if (Components.TryGetValue(typeof(T2), out var c2)) component2 = c2 as T2;
            if (component != null && component1 != null && component2 != null) return true;
            return false;
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
            Children.Add(child);
            Id2Children.Add(child.Id, child);
            if (!Type2Children.ContainsKey(child.GetType())) Type2Children.Add(child.GetType(), new List<Entity>());
            Type2Children[child.GetType()].Add(child);
            child.SetParent(this);
        }

        public void RemoveChild(Entity child)
        {
            Children.Remove(child);
            if (Type2Children.ContainsKey(child.GetType())) Type2Children[child.GetType()].Remove(child);
        }

        public Entity AddChild(Type entityType)
        {
            var entity = NewEntity(entityType);
            if (EnableLog) Log.Debug($"AddChild {this.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, this);
            return entity;
        }

        public Entity AddChild(Type entityType, object initData)
        {
            var entity = NewEntity(entityType);
            if (EnableLog) Log.Debug($"AddChild {this.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, this, initData);
            return entity;
        }

        public T AddChild<T>() where T : Entity
        {
            return AddChild(typeof(T)) as T;
        }

        public T AddIdChild<T>(long id) where T : Entity
        {
            var entityType = typeof(T);
            var entity = NewEntity(entityType, id);
            if (EnableLog) Log.Debug($"AddChild {this.GetType().Name}, {entityType.Name}={entity.Id}");
            SetupEntity(entity, this);
            return entity as T;
        }

        public T AddChild<T>(object initData) where T : Entity
        {
            return AddChild(typeof(T), initData) as T;
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

        public Entity[] GetChildren()
        {
            return Children.ToArray();
        }

        public T[] GetTypeChildren<T>() where T : Entity
        {
            return Type2Children[typeof(T)].ConvertAll(x => x.As<T>()).ToArray();
        }

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

        public void FireEvent(string eventType)
        {
            FireEvent(eventType, this);
        }

        public void FireEvent(string eventType, Entity entity)
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.FireEvent(eventType, entity);
            }
        }

        public void OnEvent(string eventType, Action<Entity> action)
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            eventComponent.OnEvent(eventType, action);
        }

        public void OffEvent(string eventType, Action<Entity> action)
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.OffEvent(eventType, action);
            }
        }
        #endregion
    }
}


//public void FireEvent<T>(string eventType, T entity) where T : Entity
//{
//    var eventComponent = GetComponent<EventComponent>();
//    if (eventComponent != null)
//    {
//        eventComponent.FireEvent(eventType, entity);
//    }
//}

//public void OnEvent<T>(string eventType, Action<T> action) where T : Entity
//{
//    var eventComponent = GetComponent<EventComponent>();
//    if (eventComponent == null)
//    {
//        eventComponent = AddComponent<EventComponent>();
//    }
//    eventComponent.OnEvent(eventType, action);
//}

//public void OffEvent<T>(string eventType, Action<T> action) where T : Entity
//{
//    var eventComponent = GetComponent<EventComponent>();
//    if (eventComponent != null)
//    {
//        eventComponent.OffEvent(eventType, action);
//    }
//}