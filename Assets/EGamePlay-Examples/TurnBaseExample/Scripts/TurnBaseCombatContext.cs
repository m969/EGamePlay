using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using GameUtils;

public class TurnBaseCombatContext : Entity
{
    public static TurnBaseCombatContext Instance { get; private set; }
    public GameTimer TurnRoundTimer { get; set; } = new GameTimer(1);
    public List<CombatEntity> HeroEntities { get; set; } = new List<CombatEntity>();
    public List<CombatEntity> MonsterEntities { get; set; } = new List<CombatEntity>();
    public List<TurnAction> TurnActions { get; set; } = new List<TurnAction>();
    public int CurrentActionIndex { get; set; } = 0;


    public override void Awake()
    {
        base.Awake();
        Instance = this;
        AddComponent<CombatActionManageComponent>();
        AddComponent<UpdateComponent>();
    }

    public override void Update()
    {
        TurnRoundTimer.UpdateAsRepeat(Time.deltaTime, OnTurnAction);
    }

    public CombatEntity AddHeroEntity()
    {
        var entity = AddChild<CombatEntity>();
        HeroEntities.Add(entity);
        return entity;
    }

    public CombatEntity AddMonsterEntity()
    {
        var entity = AddChild<CombatEntity>();
        MonsterEntities.Add(entity);
        return entity;
    }

    public void OnTurnAction()
    {
        if (CurrentActionIndex < TurnActions.Count)
        {
            var turnAction = TurnActions[CurrentActionIndex];
            turnAction.ApplyTurnAction();
            CurrentActionIndex++;
        }
        else
        {
            RefreshActions();
            CurrentActionIndex = 0;
        }
    }

    public void RefreshActions()
    {
        foreach (var item in TurnActions)
        {
            Entity.Destroy(item);
        }
        TurnActions.Clear();

        foreach (var item in HeroEntities)
        {
            var turnAction = item.CreateAction<TurnAction>();
            turnAction.Target = MonsterEntities[HeroEntities.IndexOf(item)];
            TurnActions.Add(turnAction);
        }
        foreach (var item in MonsterEntities)
        {
            var turnAction = item.CreateAction<TurnAction>();
            turnAction.Target = HeroEntities[MonsterEntities.IndexOf(item)];
            TurnActions.Add(turnAction);
        }
    }
}
