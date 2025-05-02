using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;
using EGamePlay.Combat;

namespace ECSGame
{
    public class ModelViewSystem : AComponentSystem<EcsEntity, ModelViewComponent>,
IAwake<EcsEntity, ModelViewComponent>,
IEnable<EcsEntity, ModelViewComponent>,
IDisable<EcsEntity, ModelViewComponent>,
IDestroy<EcsEntity, ModelViewComponent>
    {
        public void Awake(EcsEntity entity, ModelViewComponent component)
        {

        }

        public void Enable(EcsEntity entity, ModelViewComponent component)
        {
            if (component.ModelTrans)
            {
                component.ModelTrans.gameObject.SetActive(true);
            }
        }

        public void Disable(EcsEntity entity, ModelViewComponent component)
        {
            if (component.ModelTrans)
            {
                component.ModelTrans.gameObject.SetActive(false);
            }
        }

        public void Destroy(EcsEntity entity, ModelViewComponent component)
        {
            var viewObj = component.ModelTrans;
            if (viewObj == null)
            {
                return;
            }
            GameObject.Destroy(component.ModelTrans.gameObject);
        }
    }
}
