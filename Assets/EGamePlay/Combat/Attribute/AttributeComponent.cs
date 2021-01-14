using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗属性数值组件，在这里管理所有角色战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class AttributeComponent : Component
	{
        private readonly Dictionary<string, FloatNumeric> attributeNumerics = new Dictionary<string, FloatNumeric>();
        public FloatNumeric HealthPoint { get { return attributeNumerics[nameof(AttributeType.HealthPoint)]; } }
        public FloatNumeric AttackPower { get { return attributeNumerics[nameof(AttributeType.AttackPower)]; } }
        public FloatNumeric AttackDefense { get { return attributeNumerics[nameof(AttributeType.AttackDefense)]; } }
        public FloatNumeric CriticalProbability { get { return attributeNumerics[nameof(AttributeType.CriticalProbability)]; } }


        public override void Setup()
        {
            Initialize();
        }

        public void Initialize()
        {
            AddNumeric(nameof(AttributeType.HealthPoint), 99_999);
            AddNumeric(nameof(AttributeType.AttackPower), 1000);
            AddNumeric(nameof(AttributeType.AttackDefense), 300);
            AddNumeric(nameof(AttributeType.CriticalProbability), 0.5f);
        }

        public FloatNumeric AddNumeric(string type, float baseValue)
        {
            var numeric = new FloatNumeric();
            numeric.SetBase(baseValue);
            attributeNumerics.Add(type, numeric);
            return numeric;
        }
	}
}
