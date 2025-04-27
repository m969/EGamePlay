using ECS;
using ET;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
	public interface IAIAction
	{
        void Run(AINode aiNode);
		void Start(AINode aiNode);
	} 
}
