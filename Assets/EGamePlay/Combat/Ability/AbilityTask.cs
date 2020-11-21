using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    public class AbilityTask : Entity
    {
        public virtual async ET.ETTask ExecuteTaskAsync()
        {
            await Task.Delay(1000);
        }
    }
}