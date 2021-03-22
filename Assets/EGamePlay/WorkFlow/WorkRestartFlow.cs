using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;


public class WorkRestartFlow : WorkFlow
{
    public override void Startup()
    {
        base.Startup();
        GetParent<WorkFlow>().Startup();
    }
}
