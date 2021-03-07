using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ET;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战局上下文 
    /// 像回合制、moba这种战斗按局来分的，可以创建这个战局上下文，如果是mmo，那么战局上下文应该是在角色进入战斗才会创建，离开战斗就销毁
    /// </summary>
    public class CombatContext : Entity
    {
        public static CombatContext Instance { get; private set; }


        public override void Awake()
        {
            base.Awake();
            Instance = this;
            AddComponent<CombatActionManageComponent>();
            AddComponent<UpdateComponent>();
        }

        #region 回合制战斗
        public GameTimer TurnRoundTimer { get; set; } = new GameTimer(2f);
        public Dictionary<int, CombatEntity> HeroEntities { get; set; } = new Dictionary<int, CombatEntity>();
        public Dictionary<int, CombatEntity> MonsterEntities { get; set; } = new Dictionary<int, CombatEntity>();
        public List<TurnAction> TurnActions { get; set; } = new List<TurnAction>();
        public int CurrentActionIndex { get; set; } = 0;


        public override void Update()
        {
            if (TurnRoundTimer.IsRunning)
            {
                TurnRoundTimer.UpdateAsFinish(Time.deltaTime, StartCombat);
            }
        }

        public CombatEntity AddHeroEntity(int seat)
        {
            var entity = AddChild<CombatEntity>();
            entity.IsHero = true;
            HeroEntities.Add(seat, entity);
            entity.SeatNumber = seat;
            return entity;
        }

        public CombatEntity AddMonsterEntity(int seat)
        {
            var entity = AddChild<CombatEntity>();
            entity.IsHero = false;
            MonsterEntities.Add(seat, entity);
            entity.SeatNumber = seat;
            return entity;
        }

        public CombatEntity GetHero(int seat)
        {
            return HeroEntities[seat];
        }

        public CombatEntity GetMonster(int seat)
        {
            return MonsterEntities[seat];
        }

        public async void StartCombat()
        {
            RefreshActions();
            foreach (var item in TurnActions)
            {
                await item.ApplyTurn();
            }
            await TimerComponent.Instance.WaitAsync(1000);
            StartCombat();
        }

        public async ET.ETTask OnTurnAction()
        {
            if (CurrentActionIndex < TurnActions.Count)
            {
                var turnAction = TurnActions[CurrentActionIndex];
                turnAction.ApplyTurn();
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
                var turnAction = item.Value.CreateAction<TurnAction>();
                turnAction.Target = MonsterEntities[item.Key];
                TurnActions.Add(turnAction);
            }
            foreach (var item in MonsterEntities)
            {
                var turnAction = item.Value.CreateAction<TurnAction>();
                turnAction.Target = HeroEntities[item.Key];
                TurnActions.Add(turnAction);
            }
        }
        #endregion
    }
}