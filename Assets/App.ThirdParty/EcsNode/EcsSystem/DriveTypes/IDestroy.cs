using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
	public interface IPreDestroy
    {
	}

	public interface IDestroy
    {
	}

    public interface IDestroy<T> : IDestroy where T : EcsEntity
    {
        void Destroy(T entity);
    }

    public interface IDestroy<T, T2> : IDestroy where T : EcsEntity where T2 : EcsComponent
	{
		void Destroy(T entity, T2 component);
	}
}