using ECS;
using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力等级组件
    /// </summary>
    public class AbilityLevelComponent : EcsComponent
	{
        public int Level { get; set; } = 1;
    }
}
