using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行为概念，造成伤害、治疗英雄、赋给效果都属于战斗行为，需要继承自CombatAction
    /// </summary>
    public class CombatAction
    {
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }
    }
}