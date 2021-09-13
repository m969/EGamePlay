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
        public Dictionary<GameObject, CombatEntity> Object2Entities { get; set; } = new Dictionary<GameObject, CombatEntity>();
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
        public Dictionary<int, CombatEntity> EnemyEntities { get; set; } = new Dictionary<int, CombatEntity>();
        public List<RoundAction> RoundActions { get; set; } = new List<RoundAction>();


        public override void Update()
        {

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
            EnemyEntities.Add(seat, entity);
            entity.SeatNumber = seat;
            return entity;
        }

        public CombatEntity GetHero(int seat)
        {
            return HeroEntities[seat];
        }

        public CombatEntity GetMonster(int seat)
        {
            return EnemyEntities[seat];
        }

        public void OnCombatEntityDead(CombatEntity combatEntity)
        {
            if (combatEntity.IsHero) HeroEntities.Remove(combatEntity.SeatNumber);
            else EnemyEntities.Remove(combatEntity.SeatNumber);
        }

        public async void StartCombat()
        {
            RefreshRoundActions();
            foreach (var item in RoundActions)
            {
                if (item.Creator.CheckDead() || item.Target.CheckDead())
                {
                    continue;
                }
                await item.ApplyRound();
            }
            await TimerComponent.Instance.WaitAsync(1000);
            if (HeroEntities.Count == 0 || EnemyEntities.Count == 0)
            {
                HeroEntities.Clear();
                EnemyEntities.Clear();
                await TimerComponent.Instance.WaitAsync(2000);
                this.Publish(new CombatEndEvent());
                return;
            }
            StartCombat();
        }

        public void RefreshRoundActions()
        {
            foreach (var item in RoundActions)
            {
                Entity.Destroy(item);
            }
            RoundActions.Clear();

            foreach (var item in HeroEntities)
            {
                if (item.Value.RoundActionAbility.TryCreateAction(out var turnAction))
                {
                    if (EnemyEntities.ContainsKey(item.Key))
                    {
                        turnAction.Target = EnemyEntities[item.Key];
                    }
                    else
                    {
                        turnAction.Target = EnemyEntities.Values.ToArray().First();
                    }
                    RoundActions.Add(turnAction);
                }
            }
            foreach (var item in EnemyEntities)
            {
                if (item.Value.RoundActionAbility.TryCreateAction(out var roundAction))
                {
                    if (HeroEntities.ContainsKey(item.Key))
                    {
                        roundAction.Target = HeroEntities[item.Key];
                    }
                    else
                    {
                        roundAction.Target = HeroEntities.Values.ToArray().First();
                    }
                    RoundActions.Add(roundAction);
                }
            }
        }
        #endregion
    }

    public class CombatEndEvent
    {

    }
}