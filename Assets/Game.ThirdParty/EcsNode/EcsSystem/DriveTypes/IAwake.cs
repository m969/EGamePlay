using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public interface IAwake
    {
    }

    public interface IAwake<T> : IAwake where T : EcsEntity
    {
        void Awake(T entity);
    }

    public interface IAwake<T, T2> : IAwake where T : EcsEntity where T2 : EcsComponent
    {
        void Awake(T entity, T2 component);
    }
}
