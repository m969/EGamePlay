
namespace ECS
{
    public interface IOnAddComponent
    {

    }

    public interface IOnAddComponent<T> : IOnAddComponent where T : EcsEntity
    {
        void OnAddComponent(T entity, EcsComponent component);
    }
}