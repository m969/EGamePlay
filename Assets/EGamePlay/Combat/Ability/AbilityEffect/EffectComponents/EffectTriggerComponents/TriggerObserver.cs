using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 效果触发事件绑定
    /// </summary>
    public class TriggerObserver : Entity, ICombatObserver
    {
        public CombatEntity OwnerEntity => GetParent<AbilityEffect>().OwnerEntity;
        public IAbilityEntity OwnerAbility => GetParent<AbilityEffect>().OwnerAbility as IAbilityEntity;
        public SkillAbility SkillAbility => (OwnerAbility as SkillAbility);
        public string AffectCheck { get; set; }
        public List<Entity> ActionPointBinds { get; set; } = new List<Entity>();


        public override void Awake()
        {
            //Log.Debug($"EffectTriggerEventBind Awake {affectCheck}");
            var effectConfig = GetParent<AbilityEffect>().EffectConfig;

            ///// 行动点事件触发
            //var isAction = effectConfig.EffectTriggerType == EffectTriggerType.Action;
            //if (isAction) AddComponent<EffectActionPointTriggerComponent>();

            ///// 计时状态事件触发
            //var isCondition = effectConfig.EffectTriggerType == EffectTriggerType.Condition && !string.IsNullOrEmpty(effectConfig.ConditionParam);
            //if (isCondition) AddComponent<EffectTimeStateEventTriggerComponent>();

            /// 状态条件判断
            var haveStateCondition = !string.IsNullOrEmpty(effectConfig.ConditionParam) && effectConfig.ConditionParam.Contains('&');
            if (haveStateCondition) AddComponent<EffectTargetStateCheckComponent>();
        }

        public override void OnDestroy()
        {
            foreach (var item in ActionPointBinds)
            {
                Entity.Destroy(item);
            }
            ActionPointBinds.Clear();
            base.OnDestroy();
        }

        public void EnableTriggerBind()
        {
            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }

            /// 立即触发
            var effectConfig = GetParent<AbilityEffect>().EffectConfig;
            if (effectConfig.EffectTriggerType == EffectTriggerType.Instant)
            {
                TriggerEffectToParent();
            }
        }

        public void TriggerEffectToParent()
        {
            TriggerEffectCheckWithTarget(OwnerAbility.ParentEntity);
        }

        /// <summary>
        /// 触发技能效果
        /// </summary>
        public void TriggerEffectCheck(SkillExecution skillExecution)
        {
            TriggerEffectCheckWithTarget(skillExecution);
        }

        /// <summary>
        /// 触发对目标施加效果
        /// </summary>
        public void TriggerEffectCheckWithTarget(Entity target)
        {
            /// 这里是状态条件判断，状态条件判断是判断目标的状态是否满足条件，满足则触发效果
            var conditionCheckResult = true;
            if (TryGet(out EffectTargetStateCheckComponent component))
            {
                conditionCheckResult = component.CheckTargetState(target);
            }

            /// 条件满足则触发效果
            if (conditionCheckResult)
            {
                var effectAssign = GetParent<AbilityEffect>().CreateAssignAction(target);
                effectAssign.AssignEffect();
            }
        }

        public void OnTrigger(Entity source)
        {
            GetParent<AbilityEffect>().OnObserverTrigger(new TriggerContext(this, GetParent<AbilityEffect>().ParentEntity));
        }
    }
}