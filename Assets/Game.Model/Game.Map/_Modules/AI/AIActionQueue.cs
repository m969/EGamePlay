using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
	public class AIActionQueue
    {
		public long ActionQueueId {  get; set; }
        public Queue<Type> ActionQueue { get; set; } = new();
        public bool EndRepeat {  get; set; }
        public IAIAction CurrentAction { get; set; }
    } 
}
