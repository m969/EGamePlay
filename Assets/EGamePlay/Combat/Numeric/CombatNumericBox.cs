using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗数值匣子，在这里管理所有角色战斗属性数值的存储、变更、刷新等
    /// </summary>
    public class CombatNumericBox
	{
        public IntNumeric PhysicAttack_I = new IntNumeric();
        public IntNumeric PhysicDefense_I = new IntNumeric();
        public FloatNumeric CriticalProb_F = new FloatNumeric();


        public void Initialize()
		{
            // 这里初始化base值
            PhysicAttack_I.SetBase(1000);
            PhysicDefense_I.SetBase(300);
            CriticalProb_F.SetBase(0.5f);
        }
	}
}
