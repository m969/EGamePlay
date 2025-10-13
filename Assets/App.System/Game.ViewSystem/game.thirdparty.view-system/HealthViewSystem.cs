using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;
using EGamePlay.Combat;
using EGamePlay;
using System.ComponentModel;
using ECSUnity;
using UnityEngine.UI;
using DG.Tweening;

namespace ECSGame
{
    public class HealthViewSystem : AComponentSystem<EcsEntity, HealthViewComponent>,
IAwake<EcsEntity, HealthViewComponent>,
IEnable<EcsEntity, HealthViewComponent>,
IDisable<EcsEntity, HealthViewComponent>,
IDestroy<EcsEntity, HealthViewComponent>,
IAfterInit<EcsEntity, HealthViewComponent>
    {
        public void Awake(EcsEntity entity, HealthViewComponent component)
        {

        }

        public void AfterInit(EcsEntity entity, HealthViewComponent component)
        {
            if (entity is Actor actor)
            {
                component.HealthBarImage.fillAmount = HealthSystem.ToPercent(actor.CombatEntity);
            }
        }

        public void Enable(EcsEntity entity, HealthViewComponent component)
        {
            if (component.HealthBarImage)
            {
                component.HealthBarImage.gameObject.SetActive(true);
            }
        }

        public void Disable(EcsEntity entity, HealthViewComponent component)
        {
            if (component.HealthBarImage)
            {
                component.HealthBarImage.gameObject.SetActive(false);
            }
        }

        public void Destroy(EcsEntity entity, HealthViewComponent component)
        {
            var viewObj = component.HealthBarImage;
            if (viewObj == null)
            {
                return;
            }
            GameObject.Destroy(component.HealthBarImage.gameObject);
        }

        public static void OnReceiveDamage(EcsEntity entity, EcsEntity combatAction)
        {
            var combatEntity = entity.As<CombatEntity>();
            var component = combatEntity.Actor.GetComponent<HealthViewComponent>();
            var damageAction = combatAction as DamageAction;
            component.HealthBarImage.fillAmount = HealthSystem.ToPercent(combatEntity);
            var damageTextPrefab = StaticClient.PrefabsCollector.Get<GameObject>("DamageText");
            var damageText = GameObject.Instantiate(damageTextPrefab);
            damageText.transform.SetParent(component.CanvasTrans);
            damageText.transform.localPosition = Vector3.up * 120;
            damageText.transform.localScale = Vector3.one;
            damageText.transform.localEulerAngles = Vector3.zero;
            damageText.GetComponent<Text>().text = $"-{damageAction.DamageValue.ToString()}";
            damageText.GetComponent<DOTweenAnimation>().DORestart();
            GameObject.Destroy(damageText.gameObject, 0.5f);
        }

        public static void OnReceiveCure(EcsEntity entity, EcsEntity combatAction)
        {
            var combatEntity = entity.As<CombatEntity>();
            var component = combatEntity.Actor.GetComponent<HealthViewComponent>();
            var action = combatAction as CureAction;
            component.HealthBarImage.fillAmount = HealthSystem.ToPercent(combatEntity);
            var cureTextPrefab = StaticClient.PrefabsCollector.Get<GameObject>("CureText");
            var cureText = GameObject.Instantiate(cureTextPrefab);
            cureText.transform.SetParent(component.CanvasTrans);
            cureText.transform.localPosition = Vector3.up * 120;
            cureText.transform.localScale = Vector3.one;
            cureText.transform.localEulerAngles = Vector3.zero;
            cureText.GetComponent<Text>().text = $"+{action.CureValue.ToString()}";
            cureText.GetComponent<DOTweenAnimation>().DORestart();
            GameObject.Destroy(cureText.gameObject, 0.5f);
        }
    }
}
