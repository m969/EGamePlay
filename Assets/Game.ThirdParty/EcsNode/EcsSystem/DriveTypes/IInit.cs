using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
	public interface IInit
	{
	}

    public interface IInit<T> : IInit where T : EcsEntity
    {
        void Init(T entity);
    }

    public interface IInit<T, T2> : IInit where T : EcsEntity where T2 : EcsComponent
	{
		void Init(T entity, T2 component);
	}

    public interface IAfterInit
    {
    }

    public interface IAfterInit<T> : IAfterInit where T : EcsEntity
    {
        void AfterInit(T entity);
    }

    public interface IAfterInit<T, T2> : IAfterInit where T : EcsEntity where T2 : EcsComponent
    {
        void AfterInit(T entity, T2 component);
    }
}