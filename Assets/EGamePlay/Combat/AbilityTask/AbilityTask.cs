using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    public class AbilityTask : Entity
    {
        public object taskInitData { get; set; }


        public override void Awake(object initData)
        {
            taskInitData = initData;
        }

        public virtual async ETTask ExecuteTaskAsync()
        {
            await Task.Delay(1000);
        }
    }
}