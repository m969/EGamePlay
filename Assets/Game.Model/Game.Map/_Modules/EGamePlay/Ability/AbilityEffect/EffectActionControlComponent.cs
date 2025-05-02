using ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动禁制效果组件
    /// </summary>
    public class EffectActionControlComponent : EcsComponent
    {
        public ActionControlEffect ActionControlEffect { get; set; }
        public ActionControlType ActionControlType { get; set; }
    }
}