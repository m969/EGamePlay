using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    //public class InputEvent : AEventRun<InputEvent, Game, InputType, Vector3>
    //{
    //    protected override async ETTask Run(Game game, InputType inputType, Vector3 direction)
    //    {
    //        var component = game.GetComponent<PlayerInputComponent>();
    //        var myActor = game.MyActor;
    //        var advanceFrame = game.DetermineFrame + Game.ForecastFrame;

    //        if (inputType == InputType.Fire)
    //        {
    //            component.FireVector = direction;
    //            var input = new PlayerInput()
    //            {
    //                Frame = advanceFrame,
    //                PlayerId = myActor.Id,
    //                InputType = InputType.Fire,
    //                InputVector = direction.ToTSVector(),
    //            };

    //            ActorAdvancePlaySystem.AddLocalPlayerInput(myActor, input, advanceFrame);
    //        }
    //    }
    //}
}
