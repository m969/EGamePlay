using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;
using ET;
using Log = EGamePlay.Log;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SkillAbility
    {
        public SkillExecutionData SkillExecutionData { get; set; }
        public ExecutionObject ExecutionObject { get; set; }


        public void Awake_Client()
        {
            LoadExecution();
        }

        public void LoadExecution()
        {
            ExecutionObject = AssetUtils.Load<ExecutionObject>($"Execution_{SkillConfig.Id}");
            if (ExecutionObject == null)
            {
                return;
            }
        }
    }
}
