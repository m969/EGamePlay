using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ET;
using System.Linq;
using ECS;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战局上下文
    /// 像回合制、moba这种战斗按局来分的，可以创建这个战局上下文，如果是mmo，那么战局上下文应该是在角色进入战斗才会创建，离开战斗就销毁
    /// </summary>
    public class CombatContext : EcsEntity
    {
        public CombatEntity HeroEntity { get; set; }
#if !SERVER
        public Dictionary<GameObject, CombatEntity> Object2Entities { get; set; } = new Dictionary<GameObject, CombatEntity>();
        public Dictionary<GameObject, AbilityItem> Object2Items { get; set; } = new Dictionary<GameObject, AbilityItem>();
#endif
    }
}