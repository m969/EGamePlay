using EGamePlay.Combat;
using System.Collections.Generic;
using ET;

#if EGAMEPLAY_EXCEL
namespace EGamePlay.Combat
{
    public partial class StatusAbility : Entity, IAbilityEntity
    {
        /// 投放者、施术者
        public CombatEntity OwnerEntity { get; set; }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }
        public StatusConfig StatusConfig { get; set; }
        public bool IsChildStatus { get; set; }
        public int Duration { get; set; }
        public ChildStatus ChildStatusData { get; set; }
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            StatusConfig = initData as StatusConfig;
            Name = StatusConfig.ID;

            var Effects = new List<Effect>();

            /// 行为禁制
            if (StatusConfig.EnabledStateModify()) Effects.Add(StatusConfig.ParseActionControlEffect());
            /// 属性修饰
            if (StatusConfig.EnabledAttributeModify()) Effects.Add(StatusConfig.ParseAttributeModifyEffect());

            var effect = StatusConfigExtension.ParseEffect(StatusConfig, StatusConfig.Effect1);
            if (effect != null) Effects.Add(effect);
            effect = StatusConfigExtension.ParseEffect(StatusConfig, StatusConfig.Effect2);
            if (effect != null) Effects.Add(effect);
            effect = StatusConfigExtension.ParseEffect(StatusConfig, StatusConfig.Effect3);
            if (effect != null) Effects.Add(effect);

            AddComponent<AbilityEffectComponent>(Effects);
        }

        /// 激活
        public void ActivateAbility()
        {
            /// 子状态效果
            if (StatusConfig.EnableChildrenStatuses())
            {
                var childrenStatuses = StatusConfig.ParseChildStatus();
                foreach (var childStatusData in childrenStatuses)
                {
                    var status = ParentEntity.AttachStatus(childStatusData.StatusConfig);
                    status.OwnerEntity = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = childStatusData;
                    if (status.StatusConfig.EnabledLogicTrigger())
                    {
                        status.ProcessInputKVParams(childStatusData.Params);
                    }
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }

            Enable = true;
            Get<AbilityEffectComponent>().Enable = true;
        }

        /// 结束
        public void EndAbility()
        {
            /// 子状态效果
            if (StatusConfig.EnableChildrenStatuses())
            {
                foreach (var item in ChildrenStatuses)
                {
                    item.EndAbility();
                }
                ChildrenStatuses.Clear();
            }

            ParentEntity.OnStatusRemove(this);
            Entity.Destroy(this);
        }

        public int GetDuration()
        {
            return Duration;
        }
    }

    public static class StatusConfigExtension
    {
        public static bool EnabledAttributeModify(this StatusConfig statusConfig) => !string.IsNullOrEmpty(statusConfig.AttributeType);

        public static bool EnabledLogicTrigger(this StatusConfig statusConfig) => !string.IsNullOrEmpty(statusConfig.Effect1);

        public static bool EnableChildrenStatuses(this StatusConfig statusConfig) => ChildStatusConfigCategory.Instance.Get(statusConfig.Id) != null;

        public static bool EnabledStateModify(this StatusConfig statusConfig) => !string.IsNullOrEmpty(statusConfig.ActionControl);

        public static ActionControlType ActionControlType(this StatusConfig statusConfig)
        {
            var type = Combat.ActionControlType.None;
            if (!string.IsNullOrEmpty(statusConfig.ActionControl))
            {
                var arr = statusConfig.ActionControl.Split('|');
                foreach (var item in arr)
                {
                    if (item == "移动禁止") type = type | Combat.ActionControlType.MoveForbid;
                    if (item == "施法禁止") type = type | Combat.ActionControlType.SkillForbid;
                    if (item == "攻击禁止") type = type | Combat.ActionControlType.AttackForbid;
                }
            }
            //Log.Debug($"{type}");
            return type;
        }

        public static AttributeType AttributeType(this StatusConfig statusConfig)
        {
            var type = Combat.AttributeType.None;
            if (!string.IsNullOrEmpty(statusConfig.AttributeType))
            {
                var item = statusConfig.AttributeType;
                if (item == "攻击力") type = Combat.AttributeType.Attack;
                if (item == "法术强度") type = Combat.AttributeType.AbilityPower;
                if (item == "生命值") type = Combat.AttributeType.HealthPoint;
                if (item == "生命值上限") type = Combat.AttributeType.HealthPointMax;
                if (item == "移动速度") type = Combat.AttributeType.MoveSpeed;
                if (item == "造成伤害") type = Combat.AttributeType.CauseDamage;
            }
            //Log.Debug($"{type}");
            return type;
        }

        public static ModifyType ModifyType(this StatusConfig statusConfig)
        {
            var type = Combat.ModifyType.Add;
            if (statusConfig.AttributeParams.Contains("PercentAdd")) type = Combat.ModifyType.PercentAdd;
            return type;
        }

        public static UnityEngine.GameObject GetParticleEffect(this StatusConfig statusConfig)
        {
            return UnityEngine.Resources.Load<UnityEngine.GameObject>($"Status_{statusConfig.ID}");
        }

        public static List<ChildStatus> ParseChildStatus(this StatusConfig statusConfig)
        {
            var childrenStatuses = new List<ChildStatus>();
            var childStatusConfig = ChildStatusConfigCategory.Instance.Get(statusConfig.Id);
            if (childStatusConfig != null)
            {
                if (string.IsNullOrEmpty(childStatusConfig.ChildStatus1) == false)
                {
                    childrenStatuses.Add(ParseChildStatus(childStatusConfig.ChildStatus1, childStatusConfig.Status1KV1, childStatusConfig.Status1KV2));
                }
                if (string.IsNullOrEmpty(childStatusConfig.ChildStatus2) == false)
                {
                    childrenStatuses.Add(ParseChildStatus(childStatusConfig.ChildStatus2, childStatusConfig.Status2KV1, childStatusConfig.Status2KV2));
                }
            }
            return childrenStatuses;
        }

        public static ChildStatus ParseChildStatus(string childStatusType, string kv1, string kv2)
        {
            var childStatus = new ChildStatus();
            childStatus.StatusConfig = StatusConfigCategory.Instance.GetByName(childStatusType);
            if (string.IsNullOrEmpty(kv1) == false)
            {
                var arr = kv1.Split('=');
                childStatus.Params.Add(arr[0], arr[1]);
            }
            if (string.IsNullOrEmpty(kv2) == false)
            {
                var arr = kv2.Split('=');
                childStatus.Params.Add(arr[0], arr[1]);
            }
            return childStatus;
        }

        public static Effect ParseActionControlEffect(this StatusConfig statusConfig)
        {
            Effect effect = null;
            if (statusConfig.EnabledStateModify())
            {
                var actionControlEffect = new ActionControlEffect();
                effect = actionControlEffect;
                if (statusConfig.ActionControl.Contains("移动禁止")) actionControlEffect.ActionControlType |= Combat.ActionControlType.MoveForbid;
                if (statusConfig.ActionControl.Contains("施法禁止")) actionControlEffect.ActionControlType |= Combat.ActionControlType.SkillForbid;
                if (statusConfig.ActionControl.Contains("普攻禁止")) actionControlEffect.ActionControlType |= Combat.ActionControlType.AttackForbid;
            }
            return effect;
        }

        public static Effect ParseAttributeModifyEffect(this StatusConfig statusConfig)
        {
            Effect effect = null;
            if (statusConfig.AttributeType() != Combat.AttributeType.None && statusConfig.AttributeParams != "")
            {
                var attributeEffect = new AttributeModifyEffect();
                effect = attributeEffect;
                attributeEffect.AttributeType = statusConfig.AttributeType();
                attributeEffect.ModifyType = statusConfig.ModifyType();

                var numericValue = statusConfig.AttributeParams;
                numericValue = numericValue.Replace("PercentAdd=", "");
                numericValue = numericValue.Replace("Add=", "");
                numericValue = numericValue.Replace("%", "");
                attributeEffect.NumericValue = numericValue;
            }
            return effect;
        }

        public static Effect ParseEffect(StatusConfig statusConfig, string effectConfig)
        {
            Effect effect = null;
            if (!string.IsNullOrEmpty(effectConfig))
            {
                //Log.Debug($"ParseEffect {effectConfig}");
                //effectConfig = effectConfig.Replace("=Id", $"={statusConfig.Id}");
                var arr = effectConfig.Split('=');
                //var effectType = arr[0];
                var effectId = effectConfig;
                if (effectConfig == "Id") effectId = statusConfig.Id.ToString();
                var statusEffectConfig = ConfigHelper.Get<StatusEffectsConfig>(int.Parse(effectId));
                var effectType = statusEffectConfig.EffectType;
                var KVList = new List<string>(3);
                KVList.Add(statusEffectConfig.KV1);
                KVList.Add(statusEffectConfig.KV2);
                KVList.Add(statusEffectConfig.KV3);
                if (effectType == "AttributeModify")
                {
                    var type = statusEffectConfig.KV1;
                    var valueFormula = statusEffectConfig.KV2;
                    var attributeEffect = new AttributeModifyEffect();
                    effect = attributeEffect;
                    attributeEffect.NumericValue = valueFormula;
                    attributeEffect.ModifyType = Combat.ModifyType.Add;
                    if (type.Contains("百分比")) attributeEffect.ModifyType = Combat.ModifyType.PercentAdd;
                    type = type.Replace("百分比", "");
                    attributeEffect.TriggerProbability = statusEffectConfig.Probability;
                    if (statusEffectConfig.Target == "自身") attributeEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (statusEffectConfig.Target == "技能目标") attributeEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                    if (type == "造成伤害") attributeEffect.AttributeType = Combat.AttributeType.CauseDamage;
                    if (type == "移动速度") attributeEffect.AttributeType = Combat.AttributeType.MoveSpeed;
                }
                else if (effectType == "ActionControl")
                {
                    var actionControlEffect = new ActionControlEffect();
                    effect = actionControlEffect;
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("移动禁止")) actionControlEffect.ActionControlType |= Combat.ActionControlType.MoveForbid;
                        if (item.Contains("施法禁止")) actionControlEffect.ActionControlType |= Combat.ActionControlType.SkillForbid;
                        if (item.Contains("普攻禁止")) actionControlEffect.ActionControlType |= Combat.ActionControlType.AttackForbid;
                    }
                    actionControlEffect.TriggerProbability = statusEffectConfig.Probability;
                    if (statusEffectConfig.Target == "自身") actionControlEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (statusEffectConfig.Target == "技能目标") actionControlEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                }
                else if (effectType == "Damage")
                {
                    var Type = "";
                    var DamageValueFormula = "";
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("伤害类型=")) Type = item.Replace("伤害类型=", "");
                        if (item.Contains("伤害取值=")) DamageValueFormula = item.Replace("伤害取值=", "");
                    }
                    var damageEffect = new DamageEffect();
                    effect = damageEffect;
                    damageEffect.DamageValueFormula = DamageValueFormula;
                    damageEffect.TriggerProbability = statusEffectConfig.Probability;
                    if (statusEffectConfig.Target == "自身") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (statusEffectConfig.Target == "技能目标") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                    if (Type == "魔法伤害") damageEffect.DamageType = DamageType.Magic;
                    if (Type == "物理伤害") damageEffect.DamageType = DamageType.Physic;
                    if (Type == "真实伤害") damageEffect.DamageType = DamageType.Real;
                }
                else if (effectType == "Cure")
                {
                    var Type = "";
                    var CureValueFormula = "";
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("治疗类型=")) Type = item.Replace("治疗类型=", "");
                        if (item.Contains("治疗取值=")) CureValueFormula = item.Replace("治疗取值=", "");
                    }
                    var cureEffect = new CureEffect();
                    effect = cureEffect;
                    cureEffect.CureValueFormula = CureValueFormula;
                    cureEffect.TriggerProbability = statusEffectConfig.Probability;
                    if (statusEffectConfig.Target == "自身") cureEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (statusEffectConfig.Target == "技能目标") cureEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                }
                else if (effectType == "AddStatus")
                {
                    var StatusID = "";
                    var Duration = "";
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("状态类型=")) StatusID = item.Replace("状态类型=", "");
                        if (item.Contains("持续时间=")) Duration = item.Replace("持续时间=", "");
                    }
                    var addStatusEffect = new AddStatusEffect();
                    effect = addStatusEffect;
                    addStatusEffect.AddStatusConfig = StatusConfigCategory.Instance.GetByName(StatusID);
                    addStatusEffect.Duration = (uint)(float.Parse(Duration) * 1000);
                    ParseParam(statusEffectConfig.Param1);
                    ParseParam(statusEffectConfig.Param2);
                    void ParseParam(string paramStr)
                    {
                        if (!string.IsNullOrEmpty(paramStr))
                        {
                            arr = paramStr.Split('=');
                            addStatusEffect.Params.Add(arr[0], arr[1]);
                        }
                    }
                }
                else
                {
                    effect = new CustomEffect() { CustomEffectType = effectConfig };
                }
                if (statusEffectConfig.TriggerType == "间隔触发")
                {
                    effect.EffectTriggerType = EffectTriggerType.Interval;
                    effect.Interval = statusEffectConfig.TriggerParam;
                }
                if (statusEffectConfig.TriggerType == "条件触发")
                {
                    effect.EffectTriggerType = EffectTriggerType.Condition;
                    effect.ConditionParam = statusEffectConfig.TriggerParam;
                }
            }
            return effect;
        }
    }
}
#endif

///// 这里处理技能传入的参数数值替换
//public void ProccessInputKVParams(Dictionary<string, string> Params)
//{
//    //foreach (var aInputKVItem in Params)
//    //{
//    //    Log.Debug($"ProccessInputKVParams {aInputKVItem}");
//    //}
//    var effects = AbilityEffectComponent.AbilityEffects;
//    for (int i = 0; i < effects.Count; i++)
//    {
//        var abilityEffect = AbilityEffectComponent.GetEffect(i);
//        var effectConfig = abilityEffect.EffectConfig;
//        if (effectConfig.EffectTriggerType == EffectTriggerType.Interval)
//        {
//            if (!string.IsNullOrEmpty(effectConfig.Interval))
//            {
//                var intervalComponent = abilityEffect.GetComponent<EffectIntervalTriggerComponent>();
//                intervalComponent.IntervalValue = effectConfig.Interval;
//                foreach (var aInputKVItem in Params)
//                {
//                    intervalComponent.IntervalValue = intervalComponent.IntervalValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
//                }
//            }
//        }
//        if (effectConfig.EffectTriggerType == EffectTriggerType.Condition)
//        {
//            if (!string.IsNullOrEmpty(effectConfig.ConditionParam))
//            {
//                var conditionComponent = abilityEffect.GetComponent<EffectConditionTriggerComponent>();
//                conditionComponent.ConditionParamValue = effectConfig.ConditionParam;
//                foreach (var aInputKVItem in Params)
//                {
//                    conditionComponent.ConditionParamValue = conditionComponent.ConditionParamValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
//                }
//            }
//        }

//        if (effectConfig is DamageEffect damage)
//        {
//            var damageComponent = abilityEffect.GetComponent<EffectDamageComponent>();
//            damageComponent.DamageValueProperty = damage.DamageValueFormula;
//            foreach (var aInputKVItem in Params)
//            {
//                if (!string.IsNullOrEmpty(damageComponent.DamageValueProperty))
//                {
//                    damageComponent.DamageValueProperty = damageComponent.DamageValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
//                }
//            }
//        }
//        if (effectConfig is CureEffect cure)
//        {
//            var cureComponent = abilityEffect.GetComponent<EffectCureComponent>();
//            cureComponent.CureValueProperty = cure.CureValueFormula;
//            foreach (var aInputKVItem in Params)
//            {
//                if (!string.IsNullOrEmpty(cureComponent.CureValueProperty))
//                {
//                    cureComponent.CureValueProperty = cureComponent.CureValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
//                }
//            }
//        }
//    }
//}
