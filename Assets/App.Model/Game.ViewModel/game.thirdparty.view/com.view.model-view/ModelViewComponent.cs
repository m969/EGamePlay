using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;

namespace ECSGame
{
	public class ModelViewComponent : EcsComponent
	{
		public Transform ModelTrans { get; set; }
	} 
}
