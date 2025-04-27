using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public interface IEnable
    {
    }

    public interface IEnable<T> : IEnable where T : EcsEntity
    {
        void Enable(T entity);
    }

    public interface IEnable<T, T2> : IEnable where T : EcsEntity where T2 : EcsComponent
    {
        void Enable(T entity, T2 component);
    }

    public interface IDisable
    {
    }

    public interface IDisable<T> : IDisable where T : EcsEntity
    {
        void Disable(T entity);
    }

    public interface IDisable<T, T2> : IDisable where T : EcsEntity where T2 : EcsComponent
    {
        void Disable(T entity, T2 component);
    }
}
