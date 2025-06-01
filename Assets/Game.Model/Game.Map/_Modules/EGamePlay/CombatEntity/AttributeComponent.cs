using ECS;
using System.Collections.Generic;
#if EGAMEPLAY_ET
using AO;
using AO.EventType;
using ET.EventType;
#endif

namespace EGamePlay.Combat
{
    public class AttributeUpdateEvent { public FloatNumeric Numeric; }

    /// <summary>
    /// 战斗属性数值组件，在这里管理角色所有战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class AttributeComponent : EcsComponent<CombatEntity>
    {
        public readonly Dictionary<string, FloatNumeric> attributeNameNumerics = new Dictionary<string, FloatNumeric>();
        public FloatNumeric MoveSpeed { get { return attributeNameNumerics[nameof(AttributeType.MoveSpeed)]; } }//移动速度
        public FloatNumeric HealthPoint { get { return attributeNameNumerics[nameof(AttributeType.HealthPoint)]; } }//当前生命值
        public FloatNumeric HealthPointMax { get { return attributeNameNumerics[nameof(AttributeType.HealthPointMax)]; } }//生命值上限
        public FloatNumeric Attack { get { return attributeNameNumerics[nameof(AttributeType.Attack)]; } }//攻击力
        public FloatNumeric Defense { get { return attributeNameNumerics[nameof(AttributeType.Defense)]; } }//防御力（护甲）
        public FloatNumeric AbilityPower { get { return attributeNameNumerics[nameof(AttributeType.AbilityPower)]; } }//法术强度
        public FloatNumeric SpellResistance { get { return attributeNameNumerics[nameof(AttributeType.SpellResistance)]; } }//魔法抗性
        public FloatNumeric CriticalProbability { get { return attributeNameNumerics[nameof(AttributeType.CriticalProbability)]; } }//暴击概率
        public FloatNumeric CauseDamage { get { return attributeNameNumerics[nameof(AttributeType.CauseDamage)]; } }//暴击概率
    }
}
