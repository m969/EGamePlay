using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;


public class TurnBaseInit : SerializedMonoBehaviour
{
    public static TurnBaseInit Instance { get; private set; }
    public int JumpToTime;


    private void Start()
    {
        Instance = this;

        var combatFlow = MasterEntity.Instance.CreateChild<CombatFlow>();
        combatFlow.ToEnd();
        combatFlow.JumpToTime = JumpToTime;
        combatFlow.Startup();
    }
}
