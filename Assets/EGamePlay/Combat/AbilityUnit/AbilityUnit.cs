using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 能力单位
    /// </summary>
    public class AbilityUnit : AbilityEntity
    {
        public object unitInitData { get; set; }


        public override void Awake(object initData)
        {
            unitInitData = initData;
        }
    }
}