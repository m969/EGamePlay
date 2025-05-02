
namespace ECS
{
    public interface IOnChange
    {

    }

    public interface IOnChange<T> : IOnChange where T : EcsEntity
    {
        void OnChange(T entity);
    }

    public interface IOnChange<T, T2> : IOnChange where T : EcsEntity where T2 : EcsComponent
    {
        void OnChange(T entity, T2 component);
    }
}