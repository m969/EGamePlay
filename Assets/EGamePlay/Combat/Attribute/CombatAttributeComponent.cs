using System.Collections.Generic;

namespace EGamePlay.Combat
{
    public class AttributeType
    {
        public const string HealthPoint = "生命值";
        public const string AttackPower = "攻击力";
        public const string AttackDefense = "护甲";
        public const string CriticalProb = "暴击概率";
        public const string SpellPower = "法术强度";
        public const string SpellDefense = "魔法抗性";
    }
    /// <summary>
    /// 战斗属性数值匣子，在这里管理所有角色战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class CombatAttributeComponent : Component
	{
        public Dictionary<string, FloatNumeric> TypeNumerics = new Dictionary<string, FloatNumeric>();
        public FloatNumeric HealthPoint { get { return TypeNumerics[AttributeType.HealthPoint]; } }
        public FloatNumeric AttackPower { get { return TypeNumerics[AttributeType.AttackPower]; } }
        public FloatNumeric AttackDefense { get { return TypeNumerics[AttributeType.AttackDefense]; } }
        public FloatNumeric CriticalProb { get { return TypeNumerics[AttributeType.CriticalProb]; } }


        public override void Setup()
        {
            Initialize();
        }

        public void Initialize()
        {
            AddNumeric(AttributeType.HealthPoint, 99_999);
            AddNumeric(AttributeType.AttackPower, 1000);
            AddNumeric(AttributeType.AttackDefense, 300);
            AddNumeric(AttributeType.CriticalProb, 0.5f);
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
