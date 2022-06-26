using System.Collections.Generic;

namespace EGamePlay.Combat
{
    public class AttributeUpdateEvent { public FloatNumeric Numeric; }

    /// <summary>
    /// 战斗属性数值组件，在这里管理角色所有战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class AttributeComponent : Component
	{
        private readonly Dictionary<string, FloatNumeric> attributeNameNumerics = new Dictionary<string, FloatNumeric>();
        //private readonly Dictionary<AttributeType, FloatNumeric> attributeTypeNumerics = new Dictionary<AttributeType, FloatNumeric>();
        private readonly AttributeUpdateEvent attributeUpdateEvent = new AttributeUpdateEvent();
        public FloatNumeric MoveSpeed { get { return attributeNameNumerics[nameof(AttributeType.MoveSpeed)]; } }//移动速度
        public FloatNumeric HealthPoint { get { return attributeNameNumerics[nameof(AttributeType.HealthPoint)]; } }//当前生命值
        public FloatNumeric HealthPointMax { get { return attributeNameNumerics[nameof(AttributeType.HealthPointMax)]; } }//生命值上限
        public FloatNumeric Attack { get { return attributeNameNumerics[nameof(AttributeType.Attack)]; } }//攻击力
        public FloatNumeric Defense { get { return attributeNameNumerics[nameof(AttributeType.Defense)]; } }//防御力（护甲）
        public FloatNumeric AbilityPower { get { return attributeNameNumerics[nameof(AttributeType.AbilityPower)]; } }//法术强度
        public FloatNumeric SpellResistance { get { return attributeNameNumerics[nameof(AttributeType.SpellResistance)]; } }//魔法抗性
        public FloatNumeric CriticalProbability { get { return attributeNameNumerics[nameof(AttributeType.CriticalProbability)]; } }//暴击概率
        public FloatNumeric CauseDamage { get { return attributeNameNumerics[nameof(AttributeType.CauseDamage)]; } }//暴击概率


        public override void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            AddNumeric(AttributeType.HealthPointMax, 99_999);
            AddNumeric(AttributeType.HealthPoint, 99_999);
            AddNumeric(AttributeType.MoveSpeed, 1);
            AddNumeric(AttributeType.Attack, 1000);
            AddNumeric(AttributeType.Defense, 300);
            AddNumeric(AttributeType.CriticalProbability, 0.5f);
            AddNumeric(AttributeType.CauseDamage, 1);
        }

        public FloatNumeric AddNumeric(AttributeType attributeType, float baseValue)
        {
            var numeric = Entity.AddChild<FloatNumeric>();
            numeric.Name = attributeType.ToString();
            numeric.SetBase(baseValue);
            attributeNameNumerics.Add(attributeType.ToString(), numeric);
            return numeric;
        }

        public FloatNumeric GetNumeric(string attributeName)
        {
            return attributeNameNumerics[attributeName];
        }

        public void OnNumericUpdate(FloatNumeric numeric)
        {
            attributeUpdateEvent.Numeric = numeric;
            Entity.Publish(attributeUpdateEvent);
        }
	}
}
