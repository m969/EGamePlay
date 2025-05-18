
namespace ECS
{
    public interface IOnRemoveComponent
    {

    }

    public interface IOnRemoveComponent<T> : IOnRemoveComponent where T : EcsEntity
    {
        void OnRemoveComponent(T entity, EcsComponent component);
    }
}