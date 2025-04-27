using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class WaitAIAction : IAIAction
    {
        public void Start(AINode aiNode)
        {
            //ConsoleLog.Debug("WaitAIAction Start");
        }

        public void Run(AINode aiNode)
        {
            //var timerProgress = TimerSystem.FrameTimer(aiNode.Entity, TimerType.WaitAIAction_FrameTimer, 20);
            //if (timerProgress == TimerProgress.Ended)
            //{
            //    AISystem.FinishAndNext(aiNode);
            //}
        }
    }
}
