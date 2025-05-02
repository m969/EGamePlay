using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;
using EGamePlay.Combat;

namespace ECSGame
{
    public class TransformViewSystem : AComponentSystem<EcsEntity, TransformComponent>,
        IAwake<EcsEntity, TransformComponent>,
        IOnChange<EcsEntity, TransformComponent>
    {
        public void Awake(EcsEntity entity, TransformComponent component)
        {

        }

        public void OnChange(EcsEntity entity, TransformComponent component)
        {
            if (entity.GetComponent<ModelViewComponent>() is { } modelComp)
            {
                modelComp.ModelTrans.position = component.Position;
                modelComp.ModelTrans.GetChild(0).forward = component.Forward;
            }
        }

        public static void Update(EcsEntity entity, TransformComponent component)
        {
        }
    }
}
