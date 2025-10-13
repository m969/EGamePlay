using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ExecuteEffectEvent
    {
        public ExecuteClip ExecutionEffect;
    }

    /// <summary>
    /// 执行片段，针对于技能执行体的效果，如播放动作、生成碰撞体、位移等这些和技能表现相关的效果
    /// </summary>
    public partial class ExecuteClip : EcsEntity
    {
        public string Name { get; set; }
        public ExecuteClipData ClipData { get; set; }
        public AbilityExecution ParentExecution => GetParent<AbilityExecution>();
    }
}
