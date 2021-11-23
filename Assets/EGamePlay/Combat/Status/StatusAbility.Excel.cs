using EGamePlay.Combat;
using System.Collections.Generic;
using ET;

#if EGAMEPLAY_EXCEL
namespace EGamePlay.Combat
{
    public partial class StatusAbility : AbilityEntity
    {
        //投放者、施术者
        public override CombatEntity OwnerEntity { get; set; }
        public StatusConfig StatusConfig { get; set; }
        public FloatModifier NumericModifier { get; set; }
        public bool IsChildStatus { get; set; }
        public int Duration { get; set; }
        public ChildStatus ChildStatusData { get; set; }
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            StatusConfig = initData as StatusConfig;
            Name = StatusConfig.ID;

            //子状态效果
            if (StatusConfig.EnableChildrenStatuses())
            {

            }
            //行为禁制
            if (StatusConfig.EnabledStateModify())
            {

            }
            //属性修饰
            if (StatusConfig.EnabledAttributeModify())
            {
                AddComponent<StatusAttributeModifyComponent>();
            }
            //逻辑触发
            if (StatusConfig.EnabledLogicTrigger())
            {
                var Effects = new List<Effect>();
                var effect = StatusConfigExtension.ParseEffect(StatusConfig, StatusConfig.Effect1);
                if (effect != null) Effects.Add(effect);
                effect = StatusConfigExtension.ParseEffect(StatusConfig, StatusConfig.Effect2);
                if (effect != null) Effects.Add(effect);
                effect = StatusConfigExtension.ParseEffect(StatusConfig, StatusConfig.Effect3);
                if (effect != null) Effects.Add(effect);
                AddComponent<AbilityEffectComponent>(Effects);
            }
        }

        //这里处理技能传入的参数数值替换
        public void ProccessInputKVParams(Dictionary<string, string> Params)
        {
            //foreach (var aInputKVItem in Params)
            //{
            //    Log.Debug($"ProccessInputKVParams {aInputKVItem}");
            //}
            var effects = AbilityEffectComponent.AbilityEffects;
            for (int i = 0; i < effects.Count; i++)
            {
                var abilityEffect = AbilityEffectComponent.GetEffect(i);
                var effectConfig = abilityEffect.EffectConfig;
                if (effectConfig.EffectTriggerType == EffectTriggerType.Interval)
                {
                    if (!string.IsNullOrEmpty(effectConfig.Interval))
                    {
                        var intervalComponent = abilityEffect.GetComponent<EffectIntervalTriggerComponent>();
                        intervalComponent.IntervalValue = effectConfig.Interval;
                        foreach (var aInputKVItem in Params)
                        {
                            intervalComponent.IntervalValue = intervalComponent.IntervalValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
                if (effectConfig.EffectTriggerType == EffectTriggerType.Condition)
                {
                    if (!string.IsNullOrEmpty(effectConfig.ConditionParam))
                    {
                        var conditionComponent = abilityEffect.GetComponent<EffectConditionTriggerComponent>();
                        conditionComponent.ConditionParamValue = effectConfig.ConditionParam;
                        foreach (var aInputKVItem in Params)
                        {
                            conditionComponent.ConditionParamValue = conditionComponent.ConditionParamValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }

                if (effectConfig is DamageEffect damage)
                {
                    var damageComponent = abilityEffect.GetComponent<EffectDamageComponent>();
                    damageComponent.DamageValueProperty = damage.DamageValueFormula;
                    foreach (var aInputKVItem in Params)
                    {
                        if (!string.IsNullOrEmpty(damageComponent.DamageValueProperty))
                        {
                            damageComponent.DamageValueProperty = damageComponent.DamageValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
                if (effectConfig is CureEffect cure)
                {
                    var cureComponent = abilityEffect.GetComponent<EffectCureComponent>();
                    cureComponent.CureValueProperty = cure.CureValueFormula;
                    foreach (var aInputKVItem in Params)
                    {
                        if (!string.IsNullOrEmpty(cureComponent.CureValueProperty))
                        {
                            cureComponent.CureValueProperty = cureComponent.CureValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
            }
        }

        //激活
        public override void ActivateAbility()
        {
            base.ActivateAbility();

            //子状态效果
            if (StatusConfig.EnableChildrenStatuses())
            {
                var childrenStatuses = StatusConfig.ParseChildStatus();
                foreach (var childStatusData in childrenStatuses)
                {
                    var status = ParentEntity.AttachStatus<StatusAbility>(childStatusData.StatusConfig);
                    status.OwnerEntity = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = childStatusData;
                    if (status.StatusConfig.EnabledLogicTrigger())
                    {
                        status.ProccessInputKVParams(childStatusData.Params);
                    }
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }
            //行为禁制
            if (StatusConfig.EnabledStateModify())
            {
                ParentEntity.ActionControlType = ParentEntity.ActionControlType | StatusConfig.ActionControlType();
                //Log.Debug($"{OwnerEntity.ActionControlType}");
                if (ParentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid))
                {
                    ParentEntity.GetComponent<MotionComponent>().Enable = false;
                }
            }
            //属性修饰
            if (StatusConfig.EnabledAttributeModify())
            {
                if (StatusConfig.AttributeType() != AttributeType.None && StatusConfig.AttributeParams != "")
                {
                    var numericValue = StatusConfig.AttributeParams;
                    numericValue = numericValue.Replace("PercentAdd=", "");
                    numericValue = numericValue.Replace("Add=", "");
                    if (IsChildStatus)
                    {
                        foreach (var paramItem in ChildStatusData.Params)
                        {
                            numericValue = numericValue.Replace(paramItem.Key, paramItem.Value);
                        }
                    }
                    numericValue = numericValue.Replace("%", "");
                    var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
                    var value = (float)expression.Value;
                    NumericModifier = new FloatModifier() { Value = value };

                    var attributeType = StatusConfig.AttributeType().ToString();
                    if (StatusConfig.ModifyType() == ModifyType.Add)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfig.ModifyType() == ModifyType.PercentAdd)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfig.EnabledLogicTrigger())
            {
                AbilityEffectComponent.Enable = true;
            }
        }

        //结束
        public override void EndAbility()
        {
            //子状态效果
            if (StatusConfig.EnableChildrenStatuses())
            {
                foreach (var item in ChildrenStatuses)
                {
                    item.EndAbility();
                }
                ChildrenStatuses.Clear();
            }
            //行为禁制
            if (StatusConfig.EnabledStateModify())
            {
                ParentEntity.ActionControlType = ParentEntity.ActionControlType & (~StatusConfig.ActionControlType());
                //Log.Debug($"{OwnerEntity.ActionControlType}");
                if (ParentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid) == false)
                {
                    ParentEntity.GetComponent<MotionComponent>().Enable = true;
                }
            }
            //属性修饰
            if (StatusConfig.EnabledAttributeModify())
            {
                if (StatusConfig.AttributeType() != AttributeType.None && StatusConfig.AttributeParams != "")
                {
                    var attributeType = StatusConfig.AttributeType().ToString();
                    if (StatusConfig.ModifyType() == ModifyType.Add)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfig.ModifyType() == ModifyType.PercentAdd)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfig.EnabledLogicTrigger())
            {

            }

            NumericModifier = null;
            ParentEntity.OnStatusRemove(this);
            base.EndAbility();
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

        public static Effect ParseEffect(StatusConfig statusConfig, string effectConfig)
        {
            Effect effect = null;
            if (!string.IsNullOrEmpty(effectConfig) && effectConfig.Contains("="))
            {
                //Log.Debug($"ParseEffect effectConfig={effectConfig}");
                effectConfig = effectConfig.Replace("=Id", $"={statusConfig.Id}");
                var arr = effectConfig.Split('=');
                var effectType = arr[0];
                var effectId = arr[1];
                var skillEffectConfig = ConfigHelper.Get<SkillEffectsConfig>(int.Parse(effectId));
                var KVList = new List<string>(3);
                KVList.Add(skillEffectConfig.KV1);
                KVList.Add(skillEffectConfig.KV2);
                KVList.Add(skillEffectConfig.KV3);
                //var effectJsonStr = "{";
                //foreach (var item in paramList)
                //{
                //    var fieldStr = item.Replace("=", ":");
                //    fieldStr = fieldStr.Replace("伤害类型", "DamageType");
                //    fieldStr = fieldStr.Replace("伤害取值", "DamageValueFormula");
                //    effectJsonStr += $"{fieldStr},";
                //}
                //effectJsonStr += "}";
                //Log.Debug(effectJsonStr);
                if (effectType == "Damage")
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
                    damageEffect.TriggerProbability = skillEffectConfig.Probability;
                    if (skillEffectConfig.Target == "自身") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (skillEffectConfig.Target == "技能目标") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
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
                    cureEffect.TriggerProbability = skillEffectConfig.Probability;
                    if (skillEffectConfig.Target == "自身") cureEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (skillEffectConfig.Target == "技能目标") cureEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                }
                else if (effectType == "AddStatus")
                {
                    //var skillEffectConfig = ConfigHelper.Get<SkillEffectsConfig>(int.Parse(effectId));
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
                    ParseParam(skillEffectConfig.Param1);
                    ParseParam(skillEffectConfig.Param2);
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
                if (skillEffectConfig.TriggerType == "间隔触发")
                {
                    effect.EffectTriggerType = EffectTriggerType.Interval;
                    effect.Interval = skillEffectConfig.TriggerParam;
                }
                if (skillEffectConfig.TriggerType == "条件触发")
                {
                    effect.EffectTriggerType = EffectTriggerType.Condition;
                    effect.ConditionParam = skillEffectConfig.TriggerParam;
                }
            }
            return effect;
        }
    }
}
#endif
