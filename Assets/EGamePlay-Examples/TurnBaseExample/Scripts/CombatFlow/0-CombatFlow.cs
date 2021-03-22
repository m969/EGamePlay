using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

public class CombatFlow : WorkFlow
{
    public int JumpToTime { get; set; }


    public override void Awake()
    {
        FlowSource = CreateChild<WorkFlowSource>();
        FlowSource.ToEnter<CombatCreateFlow>().ToEnter<CombatRunFlow>().ToEnter<CombatFinishFlow>().ToRestart();
    }

    public override void Startup()
    {
        FlowSource.Startup();
    }
}
