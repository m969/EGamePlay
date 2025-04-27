using ECS;
using ECSUnity;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using EGamePlay.Combat;
using EGamePlay;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

namespace ECSGame
{
    public class Process_GameSystem
    {
        public static void RpgInit(EcsNode ecsNode, Assembly assembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Init");

            var allTypes = assembly.GetTypes();
            var typeList = new List<Type>();
            typeList.AddRange(allTypes);

            ecsNode.AddSystems(typeList.ToArray());

            EcsNodeSystem.Create(ecsNode);
            ecsNode.Init();

            ecsNode.AddComponent<ReloadComponent>(x => x.SystemAssembly = assembly);
            ecsNode.AddComponent<ConfigManageComponent>(x => x.ConfigsCollector = StaticClient.ConfigsCollector);

            var game = GameSystem.Create(ecsNode);
            game.AddComponent<PlayerInputComponent>();
            game.AddComponent<SpellPreviewComponent>();
            game.Init();

            StaticClient.Game = game;

            var combatContext = game.AddChild<CombatContext>();
            game.CombatContext = combatContext;
            StaticClient.Context = combatContext;

            var actor = ActorSystem.CreateHero(game, ecsNode.NewInstanceId());
            actor.AddComponent<ModelViewComponent>(x => x.ModelTrans = GameObject.Find("Hero").transform);
            actor.AddComponent<AnimationComponent>();
            actor.AddComponent<HealthViewComponent>(x =>
            {
                x.CanvasTrans = GameObject.Find("Hero").transform.Find("Canvas");
                x.HealthBarImage = x.CanvasTrans.Find("Image").GetComponent<Image>();
            });
            actor.GetComponent<CollisionComponent>().Layer = 1;
            actor.CombatEntity = CombatEntitySystem.Create(game, actor);
            actor.Init();
            CombatEntitySystem.HeroInit(actor.CombatEntity);
            game.MyActor = actor;
            combatContext.Object2Entities.Add(GameObject.Find("Hero"), actor.CombatEntity);

            var enemiesTrans = GameObject.Find("Enemies").transform;
            for (int i = 0; i < enemiesTrans.childCount; i++)
            {
                var monsterTrans = enemiesTrans.GetChild(i);
                actor = ActorSystem.CreateMonster(game, ecsNode.NewInstanceId());
                TransformSystem.ChangePosition(actor, monsterTrans.position);
                actor.AddComponent<ModelViewComponent>(x => x.ModelTrans = monsterTrans);
                actor.AddComponent<AnimationComponent>();
                actor.AddComponent<HealthViewComponent>(x =>
                {
                    x.CanvasTrans = monsterTrans.Find("Canvas");
                    x.HealthBarImage = x.CanvasTrans.Find("Image").GetComponent<Image>();
                });
                actor.GetComponent<CollisionComponent>().Layer = 2;
                actor.AddComponent<AIComponent>();
                actor.CombatEntity = CombatEntitySystem.Create(game, actor);
                actor.Init();
                combatContext.Object2Entities.Add(monsterTrans.gameObject, actor.CombatEntity);
                if (game.OtherActor == null)
                {
                    game.OtherActor = actor;
                }
            }
        }

        //public static void MiniInit(EcsNode ecsNode, Assembly assembly, ReferenceCollector configsCollector, AbilityConfigObject abilityConfig)
        //{
        //    ConsoleLog.Debug($"Process_GameSystem Init");

        //    var allTypes = assembly.GetTypes();
        //    var typeList = new List<Type>();
        //    typeList.AddRange(allTypes);

        //    ecsNode.AddSystems(typeList.ToArray());

        //    EcsNodeSystem.Create(ecsNode);
        //    ecsNode.Init();

        //    ecsNode.AddComponent<ReloadComponent>(x => x.SystemAssembly = assembly);
        //    ecsNode.AddComponent<ConfigManageComponent>(x => x.ConfigsCollector = configsCollector);

        //    var game = GameSystem.Create(ecsNode);
        //    game.AddComponent<PlayerInputComponent>();
        //    game.Init();

        //    StaticClient.Game = game;

        //    var combatContext = game.AddChild<CombatContext>();
        //    StaticClient.Context = combatContext;

        //    //创建怪物战斗实体
        //    var monster = combatContext.AddChild<CombatEntity>();
        //    //创建英雄战斗实体
        //    var hero = combatContext.AddChild<CombatEntity>();
        //    //给英雄挂载技能并加载技能执行体
        //    var heroSkillAbility = SkillSystem.Attach(hero, abilityConfig);

        //    ConsoleLog.Debug($"1 monster.CurrentHealth={monster.GetComponent<HealthPointComponent>().Value}");
        //    //使用英雄技能攻击怪物
        //    SpellSystem.SpellWithTarget(hero, heroSkillAbility, monster);
        //    ConsoleLog.Debug($"2 monster.CurrentHealth={monster.GetComponent<HealthPointComponent>().Value}");
        //    //--示例结束--
        //}

        public static void Reload(EcsNode ecsNode, Assembly assembly)
        {
            ConsoleLog.Debug($"Process_GameSystem Reload");

            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = assembly;

            var allTypes = assembly.GetTypes();
            var typeList = new List<Type>();
            typeList.AddRange(allTypes);

            ecsNode.AddSystems(typeList.ToArray());

            EventSystem.Reload(ecsNode);
        }
    }
}
