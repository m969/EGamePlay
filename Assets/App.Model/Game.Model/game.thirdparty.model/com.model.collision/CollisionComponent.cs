using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
	public class CollisionComponent : EcsComponent
	{
		public uint Layer { get; set; }
	} 
}
