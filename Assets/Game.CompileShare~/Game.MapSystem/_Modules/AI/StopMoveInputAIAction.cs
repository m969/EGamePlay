using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class StopMoveInputAIAction : IAIAction
    {
        public void Start(AINode aiNode)
        {
            //var game = aiNode.Entity.GetParent<Game>();
            //var actor = (Actor)aiNode.Entity;
            //var component = actor.GetComponent<AIComponent>();
            //var input = new PlayerInput() { InputType = InputType.StopMove, PlayerId = aiNode.Entity.Id };
            //ActorPredictPlaySystem.AddNetworkPlayerInput(actor, input, component.DetermineFrame);
            //AISystem.FinishAndNext(aiNode);
        }

        public void Run(AINode aiNode)
        {

        }
    }
}
