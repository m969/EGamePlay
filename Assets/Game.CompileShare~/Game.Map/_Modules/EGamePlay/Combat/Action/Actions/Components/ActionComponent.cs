using ECS;
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
    public class ActionComponent : EcsComponent
    {
        public Type ActionType;


        //public override void Awake(object initData)
        //{
        //    ActionType = initData as Type;
        //}
    }
}