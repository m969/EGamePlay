using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    public class PlayAnimationTask : AbilityTask
    {
        public override async ETTask ExecuteTaskAsync()
        {
            //播放动作
            await TimerComponent.Instance.WaitAsync(1000);
            Entity.Destroy(this);
        }
    }
}