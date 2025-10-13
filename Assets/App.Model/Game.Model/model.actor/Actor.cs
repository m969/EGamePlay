using ECS;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
	public class Actor : EcsEntity
	{
		public int Type { get; set; }
		public CombatEntity CombatEntity { get; set; }
	}
}
