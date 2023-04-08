using System;
using System.Collections.Generic;

namespace EGamePlay
{
    public class Component
    {
        public Entity Entity { get; set; }
        public bool IsDisposed { get; set; }
        public Dictionary<long, Entity> Id2Children { get; private set; } = new Dictionary<long, Entity>();
        //public List<long> EntityChildren { get; private set; } = new List<long>();
        public virtual bool DefaultEnable { get; set; } = true;
        private bool enable = false;
        public bool Enable
        {
            set
            {
                if (enable == value) return;
                enable = value;
                if (enable) OnEnable();
                else OnDisable();
            }
            get
            {
                return enable;
            }
        }
        public bool Disable => enable == false;


        public T GetEntity<T>() where T : Entity
        {
            return Entity as T;
        }

        public virtual void Awake()
        {

        }

        public virtual void Awake(object initData)
        {

        }

        public virtual void Setup()
        {

        }

        public virtual void Setup(object initData)
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
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
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->Dispose");
            Enable = false;
            IsDisposed = true;
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

        public T Publish<T>(T TEvent) where T : class
        {
            Entity.Publish(TEvent);
            return TEvent;
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            Entity.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            Entity.UnSubscribe(action);
        }

        //public Entity AddChild(Type entityType)
        //{
        //    var child = Entity.AddChild(entityType);
        //    Id2Children.Add(child.Id, child);
        //    return child;
        //}

        //public Entity AddChild(Type entityType, object initData)
        //{
        //    var child = Entity.AddChild(entityType, initData);
        //    Id2Children.Add(child.Id, child);
        //    return child;
        //}

        //public T AddChild<T>() where T : Entity
        //{
        //    return AddChild(typeof(T)) as T;
        //}

        //public T AddChild<T>(object initData) where T : Entity
        //{
        //    return AddChild(typeof(T), initData) as T;
        //}

        //public void RemoveChild(Entity child)
        //{
        //    Children.Remove(child);
        //}

        //public T AddIdChild<T>(long id) where T : Entity
        //{
        //    var entityType = typeof(T);
        //    var entity = NewEntity(entityType, id);
        //    if (EnableLog) Log.Debug($"AddChild {this.GetType().Name}, {entityType.Name}={entity.Id}");
        //    SetupEntity(entity, this);
        //    return entity as T;
        //}
    }
}