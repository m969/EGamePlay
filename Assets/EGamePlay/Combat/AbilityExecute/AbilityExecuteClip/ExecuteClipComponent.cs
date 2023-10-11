using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
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
            foreach (var effect in GetEntity<SkillExecution>().ExecutionObject.ExecuteClips)
            {
                var executionEffect = Entity.AddChild<ExecuteClip>(effect);
                AddEffect(executionEffect);
            }
        }

        public void AddEffect(ExecuteClip executionEffect)
        {
            ExecuteClips.Add(executionEffect);
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