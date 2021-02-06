using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗属性数值组件，在这里管理所有角色战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class AttributeComponent : EntityComponent<CombatEntity>
	{
        private readonly Dictionary<string, FloatNumeric> attributeNameNumerics = new Dictionary<string, FloatNumeric>();
        private readonly Dictionary<AttributeType, FloatNumeric> attributeTypeNumerics = new Dictionary<AttributeType, FloatNumeric>();
        public FloatNumeric MoveSpeed { get { return attributeNameNumerics[nameof(AttributeType.MoveSpeed)]; } }
        public FloatNumeric CauseDamage { get { return attributeNameNumerics[nameof(AttributeType.CauseDamage)]; } }
        public FloatNumeric HealthPoint { get { return attributeNameNumerics[nameof(AttributeType.HealthPoint)]; } }
        public FloatNumeric AttackPower { get { return attributeNameNumerics[nameof(AttributeType.AttackPower)]; } }
        public FloatNumeric AttackDefense { get { return attributeNameNumerics[nameof(AttributeType.AttackDefense)]; } }
        public FloatNumeric CriticalProbability { get { return attributeNameNumerics[nameof(AttributeType.CriticalProbability)]; } }


        public override void Setup()
        {
            Initialize();
        }

        public void Initialize()
        {
            AddNumeric(AttributeType.HealthPoint, nameof(AttributeType.HealthPoint), 99_999);
            AddNumeric(AttributeType.MoveSpeed, nameof(AttributeType.MoveSpeed), 1);
            AddNumeric(AttributeType.CauseDamage, nameof(AttributeType.CauseDamage), 1);
            AddNumeric(AttributeType.AttackPower, nameof(AttributeType.AttackPower), 1000);
            AddNumeric(AttributeType.AttackDefense, nameof(AttributeType.AttackDefense), 300);
            AddNumeric(AttributeType.CriticalProbability, nameof(AttributeType.CriticalProbability), 0.5f);
        }

        public FloatNumeric AddNumeric(AttributeType attributeType, string attributeName, float baseValue)
        {
            var numeric = new FloatNumeric();
            numeric.SetBase(baseValue);
            attributeNameNumerics.Add(attributeName, numeric);
            return numeric;
        }

        public FloatNumeric GetNumeric(string attributeName)
        {
            return attributeNameNumerics[attributeName];
        }
	}
}
