using System;
using System.Collections.Generic;

namespace EGamePlay
{
    public sealed class EntityFactory
    {
        public static GlobalEntity GlobalEntity { get; set; }


        ///// <summary>
        ///// 创建自治的实体，自治实体不需要依赖于父实体
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static T CreateIndependent<T>() where T : Entity, new()
        //{
        //    var entity = new T();
        //    if (!GlobalEntity.Entities.ContainsKey(typeof(T)))
        //    {
        //        GlobalEntity.Entities.Add(typeof(T), new List<Entity>());
        //    }
        //    GlobalEntity.Entities[typeof(T)].Add(entity);
        //    entity.IsIndependent = true;
        //    entity.Awake();
        //    return entity;
        //}

        private static T New<T>() where T : Entity, new()
        {
            var entity = new T();
            if (!GlobalEntity.Entities.ContainsKey(typeof(T)))
            {
                GlobalEntity.Entities.Add(typeof(T), new List<Entity>());
            }
            GlobalEntity.Entities[typeof(T)].Add(entity);
            entity.IsDispose = false;
            return entity;
        }

        public static T Create<T>() where T : Entity, new()
        {
            var entity = New<T>();
            GlobalEntity.AddChild(entity);
            entity.Awake();
            Log.Debug($"EntityFactory: Create {entity}");
            return entity;
        }

        public static T Create<T>(object paramObject) where T : Entity, new()
        {
            var entity = New<T>();
            GlobalEntity.AddChild(entity);
            entity.Awake(paramObject);
            Log.Debug($"EntityFactory: Create {entity} {paramObject}");
            return entity;
        }

        public static T CreateWithParent<T>(Entity parent) where T : Entity, new()
        {
            var entity = New<T>();
            parent.AddChild(entity);
            entity.Awake();
            Log.Debug($"EntityFactory: CreateWithParent {parent} {entity}");
            return entity;
        }

        public static T CreateWithParent<T>(Entity parent, object paramObject) where T : Entity, new()
        {
            var entity = New<T>();
            parent.AddChild(entity);
            entity.Awake(paramObject);
            Log.Debug($"EntityFactory: CreateWithParent {parent} {entity}");
            return entity;
        }
    }
}