using ECS;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
	public class ActorCombatComponent : EcsComponent
	{
		public CombatEntity CombatEntity { get; set; }
	} 
}
