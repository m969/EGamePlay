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
    /// 战斗数值匣子，在这里管理所有角色战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class CombatNumericBox
	{
        public Dictionary<string, FloatNumeric> TypeNumerics = new Dictionary<string, FloatNumeric>();
        public IntNumeric HealthPoint_I = new IntNumeric();
        public IntNumeric PhysicAttack_I = new IntNumeric();
        public IntNumeric PhysicDefense_I = new IntNumeric();
        public FloatNumeric CriticalProb_F = new FloatNumeric();


        public void Initialize()
        {
            // 这里初始化base值
            HealthPoint_I.SetBase(99_999);
            PhysicAttack_I.SetBase(1000);
            PhysicDefense_I.SetBase(300);
            CriticalProb_F.SetBase(0.5f);
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
