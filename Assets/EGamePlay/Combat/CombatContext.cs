using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ET;
using System.Linq;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战局上下文 
    /// 像回合制、moba这种战斗按局来分的，可以创建这个战局上下文，如果是mmo，那么战局上下文应该是在角色进入战斗才会创建，离开战斗就销毁
    /// </summary>
    public class CombatContext : Entity
    {
        public static CombatContext Instance { get; private set; }
#if !SERVER
        public Dictionary<GameObject, CombatEntity> GameObject2Entitys { get; set; } = new Dictionary<GameObject, CombatEntity>();
#endif


        public override void Awake()
        {
            base.Awake();
            Instance = this;
            AddComponent<CombatActionManageComponent>();
            AddComponent<UpdateComponent>();
        }

        #region 回合制战斗
        //public GameTimer TurnRoundTimer { get; set; } = new GameTimer(2f);
        public Dictionary<int, CombatEntity> HeroEntities { get; set; } = new Dictionary<int, CombatEntity>();
        public Dictionary<int, CombatEntity> MonsterEntities { get; set; } = new Dictionary<int, CombatEntity>();
        public List<TurnAction> TurnActions { get; set; } = new List<TurnAction>();


        public override void Update()
        {
            //if (TurnRoundTimer.IsRunning)
            //{
            //    TurnRoundTimer.UpdateAsFinish(Time.deltaTime, StartCombat);
            //}
        }

        public CombatEntity AddHeroEntity(int seat)
        {
            var entity = CreateChild<CombatEntity>();
            entity.IsHero = true;
            HeroEntities.Add(seat, entity);
            entity.SeatNumber = seat;
            return entity;
        }

        public CombatEntity AddMonsterEntity(int seat)
        {
            var entity = CreateChild<CombatEntity>();
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

        public void OnCombatEntityDead(CombatEntity combatEntity)
        {
            if (combatEntity.IsHero) HeroEntities.Remove(combatEntity.SeatNumber);
            else MonsterEntities.Remove(combatEntity.SeatNumber);
        }

        public async void StartCombat()
        {
            RefreshActions();
            foreach (var item in TurnActions)
            {
                if (item.Creator.CheckDead() || item.Target.CheckDead())
                {
                    continue;
                }
                await item.ApplyTurn();
            }
            await TimerComponent.Instance.WaitAsync(1000);
            if (HeroEntities.Count == 0 || MonsterEntities.Count == 0)
            {
                HeroEntities.Clear();
                MonsterEntities.Clear();
                await TimerComponent.Instance.WaitAsync(2000);
                this.Publish(new CombatEndEvent());
                return;
            }
            StartCombat();
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
                if (item.Value.TurnActionAbility.TryCreateAction(out var turnAction))
                {
                    if (MonsterEntities.ContainsKey(item.Key))
                    {
                        turnAction.Target = MonsterEntities[item.Key];
                    }
                    else
                    {
                        turnAction.Target = MonsterEntities.Values.ToArray().First();
                    }
                    TurnActions.Add(turnAction);
                }
            }
            foreach (var item in MonsterEntities)
            {
                if (item.Value.TurnActionAbility.TryCreateAction(out var turnAction))
                {
                    if (HeroEntities.ContainsKey(item.Key))
                    {
                        turnAction.Target = HeroEntities[item.Key];
                    }
                    else
                    {
                        turnAction.Target = HeroEntities.Values.ToArray().First();
                    }
                    TurnActions.Add(turnAction);
                }
            }
        }
        #endregion
    }

    public class CombatEndEvent
    {

    }
}