using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionComponent : Component
    {
        private Type ActionType;


        public override void Awake(object initData)
        {
            ActionType = initData as Type;
        }

        //public bool TryMakeAction(out Entity action)
        //{
        //    if (Enable == false)
        //    {
        //        action = null;
        //    }
        //    else
        //    {
        //        action = Entity.GetParent<CombatEntity>().AddChild(ActionType, Entity);
        //    }
        //    return Enable;
        //}
    }
}