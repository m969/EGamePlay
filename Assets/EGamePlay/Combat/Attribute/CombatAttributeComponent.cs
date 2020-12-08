using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗属性数值匣子，在这里管理所有角色战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class CombatAttributeComponent : Component
	{
        public Dictionary<string, FloatNumeric> TypeNumerics = new Dictionary<string, FloatNumeric>();
        public FloatNumeric HealthPoint { get { return TypeNumerics[nameof(AttributeType.HealthPoint)]; } }
        public FloatNumeric AttackPower { get { return TypeNumerics[nameof(AttributeType.AttackPower)]; } }
        public FloatNumeric AttackDefense { get { return TypeNumerics[nameof(AttributeType.AttackDefense)]; } }
        public FloatNumeric CriticalProb { get { return TypeNumerics[nameof(AttributeType.CriticalProb)]; } }


        public override void Setup()
        {
            Initialize();
        }

        public void Initialize()
        {
            AddNumeric(nameof(AttributeType.HealthPoint), 99_999);
            AddNumeric(nameof(AttributeType.AttackPower), 1000);
            AddNumeric(nameof(AttributeType.AttackDefense), 300);
            AddNumeric(nameof(AttributeType.CriticalProb), 0.5f);
        }

        public FloatNumeric AddNumeric(string type, float baseValue)
        {
            var numeric = new FloatNumeric();
            numeric.SetBase(baseValue);
            TypeNumerics.Add(type, numeric);
            return numeric;
        }
	}
}
