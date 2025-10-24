
namespace ECS
{
    public interface IEntityState
    {
        public void SetEntity(EcsEntity entity);
    }

    public class EntityState<T> : IEntityState where T : EcsEntity
    {
        public long InstanceId { get; private set; }
        private T entity;

        //public EntityState(T t)
        //{
        //    if (t == null)
        //    {
        //        this.InstanceId = 0;
        //        this.entity = null;
        //        return;
        //    }
        //    this.InstanceId = t.InstanceId;
        //    this.entity = t;
        //}

        public void SetEntity(EcsEntity entity)
        {
            if (entity == null)
            {
                this.InstanceId = 0;
                this.entity = null;
                return;
            }
            this.InstanceId = entity.InstanceId;
            this.entity = (T)entity;
        }

        private T UnWrap
        {
            get
            {
                if (this.entity == null)
                {
                    return null;
                }
                if (this.entity.InstanceId != this.InstanceId)
                {
                    // 这里instanceId变化了，设置为null，解除引用，好让runtime去gc
                    this.entity = null;
                }
                return this.entity;
            }
        }

        //public static implicit operator EntityState<T>(T t)
        //{
        //    return new EntityState<T>(t);
        //}

        public static implicit operator T(EntityState<T> v)
        {
            return v.UnWrap;
        }
    }
}