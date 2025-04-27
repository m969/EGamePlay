using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using UnityEngine.UI;

namespace ECSGame
{
	public class HealthViewComponent : EcsComponent
	{
		public Transform CanvasTrans { get; set; }
		public Image HealthBarImage { get; set; }
    } 
}
