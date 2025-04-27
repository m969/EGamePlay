//using System;
//using System.Collections.Generic;

//namespace EGamePlay
//{
//    public class Component
//    {
//        public Entity Entity { get; set; }
//        public bool IsDisposed { get; set; }
//        public Dictionary<long, Entity> Id2Children { get; private set; } = new Dictionary<long, Entity>();
//        public virtual bool DefaultEnable { get; set; } = true;
//        private bool enable = false;
//        public bool Enable
//        {
//            set
//            {
//                if (enable == value) return;
//                enable = value;
//                if (enable) OnEnable();
//                else OnDisable();
//            }
//            get
//            {
//                return enable;
//            }
//        }
//        public bool Disable => enable == false;


//        public T GetEntity<T>() where T : Entity
//        {
//            return Entity as T;
//        }

//        public virtual void Awake()
//        {

//        }

//        public virtual void Awake(object initData)
//        {

//        }

//        public virtual void Setup()
//        {

//        }

//        public virtual void Setup(object initData)
//        {

//        }

//        public virtual void OnEnable()
//        {

//        }

//        public virtual void OnDisable()
//        {

//        }

//        public virtual void Update()
//        {

//        }

//        public virtual void FixedUpdate()
//        {

//        }

//        public virtual void OnDestroy()
//        {
            
//        }

//        private void Dispose()
//        {
//            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->Dispose");
//            Enable = false;
//            IsDisposed = true;
//        }

//        public static void Destroy(Component entity)
//        {
//            try
//            {
//                entity.OnDestroy();
//            }
//            catch (Exception e)
//            {
//                Log.Error(e);
//            }
//            entity.Dispose();
//        }

//        public T Publish<T>(T TEvent) where T : class
//        {
//            Entity.Publish(TEvent);
//            return TEvent;
//        }

//        public void Subscribe<T>(Action<T> action) where T : class
//        {
//            Entity.Subscribe(action);
//        }

//        public void UnSubscribe<T>(Action<T> action) where T : class
//        {
//            Entity.UnSubscribe(action);
//        }
//    }
//}