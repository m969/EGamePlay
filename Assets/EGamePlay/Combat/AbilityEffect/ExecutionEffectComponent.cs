using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionEffectComponent : Component
    {
        public List<ExecutionEffect> ExecutionEffects { get; private set; } = new List<ExecutionEffect>();


        public override void Awake()
        {
            if (GetEntity<SkillExecution>().ExecutionObject == null)
            {
                return;
            }
            foreach (var effect in GetEntity<SkillExecution>().ExecutionObject.ExecuteClips)
            {
                var executionEffect = Entity.AddChild<ExecutionEffect>(effect);
                AddEffect(executionEffect);
            }
        }

        public void AddEffect(ExecutionEffect executionEffect)
        {
            ExecutionEffects.Add(executionEffect);
        }

        public void BeginExecute()
        {
            foreach (var item in ExecutionEffects)
            {
                item.BeginExecute();
            }
        }
    }
}