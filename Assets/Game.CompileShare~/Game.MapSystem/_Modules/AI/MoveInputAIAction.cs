using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class MoveInputAIAction: IAIAction
    {
        public void Start(AINode aiNode)
        {
            //var actor = (Actor)aiNode.Entity;
            //var game = aiNode.Entity.GetParent<Game>();
            //var r = game.TSRandom.Next(1, 5);
            //var vector = TSVector.left;
            //if (r == 1) vector = TSVector.left;
            //if (r == 2) vector = TSVector.right;
            //if (r == 3) vector = TSVector.forward;
            //if (r == 4) vector = TSVector.back;
            //actor.GetComponent<AIComponent>().MoveDirection = vector;
        }

        public void Run(AINode aiNode)
        {
            //var game = aiNode.Entity.GetParent<Game>();
            //var actor = (Actor)aiNode.Entity;
            //var component = actor.GetComponent<AIComponent>();
            //var input = new PlayerInput() { InputType = InputType.Move, InputVector = component.MoveDirection, PlayerId = aiNode.Entity.Id };
            //ActorPredictPlaySystem.AddNetworkPlayerInput(actor, input, component.DetermineFrame);
            //var timerProgress = TimerSystem.FrameTimer(aiNode.Entity, TimerType.RunAIAction_FrameTimer, 40);
            //if (timerProgress == TimerProgress.Ended)
            //{
            //    AISystem.FinishAndNext(aiNode);
            //}
        }
    }
}
