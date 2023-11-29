using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 执行片段组件，一个技能执行体可以包含多个执行片段
    /// </summary>
    public class ExecuteClipComponent : Component
    {
        public List<ExecuteClip> ExecuteClips { get; private set; } = new List<ExecuteClip>();


        public override void Awake()
        {
            if (GetEntity<SkillExecution>().ExecutionObject == null)
            {
                return;
            }
            foreach (var clip in GetEntity<SkillExecution>().ExecutionObject.ExecuteClips)
            {
                var executeClip = Entity.AddChild<ExecuteClip>(clip);
                AddClip(executeClip);
            }
        }

        public void AddClip(ExecuteClip executeClip)
        {
            ExecuteClips.Add(executeClip);
        }

        public void BeginExecute()
        {
            foreach (var item in ExecuteClips)
            {
                item.BeginExecute();
            }
        }
    }
}