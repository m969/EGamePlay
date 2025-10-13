using ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    public interface IStateCheck
    {
        bool CheckWith(string affectCheck, EcsEntity target);
    }

    /// <summary>
    /// 状态判断组件
    /// </summary>
    public class TriggerStateCheckComponent : EcsComponent
    {
        public Dictionary<(string, bool), IStateCheck> StateCheckMap { get; set; } = new();
    }
}