using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public interface IUpdate
    {
    }

    public interface IUpdate<in T> : IUpdate where T : EcsEntity
    {
        void Update(T entity);
    }

    public interface IFixedUpdate
    {
    }

    public interface IFixedUpdate<T> : IFixedUpdate where T : EcsEntity
    {
        void FixedUpdate(T entity);
    }

    public interface ISecondUpdate
    {
    }

    public interface ISecondUpdate<T> : ISecondUpdate where T : EcsEntity
    {
        void SecondUpdate(T entity);
    }

    //public interface IUpdate<T, T2> : IUpdate where T : EcsEntity where T2 : EcsComponent
    //{
    //    void Update(T entity, T2 component);
    //}
}
