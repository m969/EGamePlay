using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    public class PlayAnimationTask : AbilityTask
    {
        public override async ET.ETTask ExecuteTaskAsync()
        {
            //播放动作
            await ET.TimerComponent.Instance.WaitAsync(1000);
            Entity.Destroy(this);
        }
    }
}