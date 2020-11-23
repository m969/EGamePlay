using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    public class AbilityTask : Entity
    {
        public object taskInitData { get; set; }


        public override void Awake(object initData)
        {
            taskInitData = initData;
        }

        public virtual async ET.ETTask ExecuteTaskAsync()
        {
            await Task.Delay(1000);
        }
    }
}