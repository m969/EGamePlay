using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public interface IUpdate : IDrive
    {
    }

    public interface IUpdate<in T> : IUpdate where T : EcsEntity
    {
        void Update(T entity);
    }

    public interface IFixedUpdate : IDrive
    {
    }

    public interface IFixedUpdate<T> : IFixedUpdate where T : EcsEntity
    {
        void FixedUpdate(T entity);
    }

    public interface ISecondUpdate : IDrive
    {
    }

    public interface ISecondUpdate<T> : ISecondUpdate where T : EcsEntity
    {
        void SecondUpdate(T entity);
    }
}
