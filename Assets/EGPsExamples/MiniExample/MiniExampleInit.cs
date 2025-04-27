using EGamePlay.Combat;
using EGamePlay;
using ET;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ECS;
using ECSUnity;

public class MiniExampleInit : MonoBehaviour
{
    public bool EntityLog;
    public AbilityConfigObject SkillConfigObject;
    public ReferenceCollector ConfigsCollector;
    public EcsNode EcsNode { get; private set; }


    private void Awake()
    {
        //SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        //Entity.EnableLog = EntityLog;
        //ECSNode.Create();
        //Entity.Create<TimerManager>();
        //Entity.Create<CombatContext>();

        EcsNode = new EcsNode(1);
        EcsNode.AddChild<CombatContext>();
        EcsNode.AddComponent<ConfigManageComponent>(x => x.ConfigsCollector = ConfigsCollector);

        //await TimerManager.Instance.WaitAsync(2000);
        //创建怪物战斗实体
        var monster = StaticClient.Context.AddChild<CombatEntity>();
        //创建英雄战斗实体
        var hero = StaticClient.Context.AddChild<CombatEntity>();
        //给英雄挂载技能并加载技能执行体
        var heroSkillAbility = SkillSystem.Attach(hero, SkillConfigObject);

        Debug.Log($"1 monster.CurrentHealth={monster.GetComponent<HealthPointComponent>().Value}");
        //使用英雄技能攻击怪物
        SpellSystem.SpellWithTarget(hero, heroSkillAbility, monster);
        //await TimerManager.Instance.WaitAsync(2000);
        Debug.Log($"2 monster.CurrentHealth={monster.GetComponent<HealthPointComponent>().Value}");
        //--示例结束--
    }

    private void Update()
    {
        EcsNode.DriveEntityUpdate();
        //TimerManager.Instance?.Update();
    }

    private void OnApplicationQuit()
    {
        EcsObject.Destroy(EcsNode);
    }
}
