using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;

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

        public static void Update(EcsEntity entity, ModelViewComponent component)
        {
            var viewObj = component.ModelTrans;
            if (viewObj == null)
            {
                return;
            }

            var transComp = entity.GetComponent<TransformComponent>();
            if (transComp == null)
            {
                return;
            }

            //if (entity is Actor)
            //{
            //    var newPos = entity.GetComponent<TransformComponent>().ForecastPosition;
            //    viewObj.transform.position = Vector3.Lerp(viewObj.transform.position, newPos, 0.5f);
            //    var newPos2 = entity.GetComponent<TransformComponent>().Position;
            //    viewObj.transform.GetChild(1).position = Vector3.Lerp(viewObj.transform.GetChild(1).position, newPos2, 0.5f);
            //}
            //else
            {
                var newPos = entity.GetComponent<TransformComponent>().Position;
                viewObj.transform.position = Vector3.Lerp(viewObj.transform.position, newPos, 0.5f);
                //ConsoleLog.Debug($"ModelViewSystem Update {newPos}");
            }
        }
    }
}
