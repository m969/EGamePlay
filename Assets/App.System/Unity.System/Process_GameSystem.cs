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
            ecsNode.AddComponent<EntityObjectComponent>();
            ecsNode.AddComponent<ReloadComponent>(x => x.SystemAssembly = assembly);
            ecsNode.AddComponent<ConfigManageComponent>(x => x.ConfigsCollector = StaticClient.ConfigsCollector);
            ecsNode.Init();

            var game = GameSystem.Create(ecsNode);
            game.AddComponent<PlayerInputComponent>();
            game.AddComponent<SpellPreviewComponent>();
            game.Init();

            StaticClient.Game = game;

            var canvasTrans = GameObject.Find("Hero").transform.Find("Canvas");
            var healthImage = canvasTrans.Find("Image").GetComponent<Image>();
            var actor = ActorSystem.CreateHero(game, ecsNode.NewInstanceId());
            actor.AddComponent<ModelViewComponent>(x => x.ModelTrans = GameObject.Find("Hero").transform);
            actor.AddComponent<AnimationComponent>();
            actor.AddComponent<HealthViewComponent>(x =>
            {
                x.CanvasTrans = canvasTrans;
                x.HealthBarImage = healthImage;
            });
            actor.GetComponent<CollisionComponent>().Layer = 1;
            actor.AddComponent<ActorCombatComponent>();
            actor.CombatEntity.IsHero = true;
            actor.Init();
            game.MyActor = actor;
            game.Object2Entities.Add(GameObject.Find("Hero"), actor.CombatEntity);
            StaticClient.Hero = actor.CombatEntity;

            var enemiesTrans = GameObject.Find("Enemies").transform;
            for (int i = 0; i < enemiesTrans.childCount; i++)
            {
                var monsterTrans = enemiesTrans.GetChild(i);
                canvasTrans = monsterTrans.Find("Canvas");
                healthImage = canvasTrans.Find("Image").GetComponent<Image>();

                actor = ActorSystem.CreateMonster(game, ecsNode.NewInstanceId());
                TransformSystem.ChangePosition(actor, monsterTrans.position);
                actor.AddComponent<ModelViewComponent>(x => x.ModelTrans = monsterTrans);
                actor.AddComponent<AnimationComponent>();
                actor.AddComponent<HealthViewComponent>(x =>
                {
                    x.CanvasTrans = canvasTrans;
                    x.HealthBarImage = healthImage;
                });
                actor.GetComponent<CollisionComponent>().Layer = 2;
                actor.AddComponent<ActorCombatComponent>();
                actor.AddComponent<AIComponent>();
                actor.Init();
                game.Object2Entities.Add(monsterTrans.gameObject, actor.CombatEntity);

                if (monsterTrans.name == "Monster")
                {
                    StaticClient.Boss = actor.CombatEntity;
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
