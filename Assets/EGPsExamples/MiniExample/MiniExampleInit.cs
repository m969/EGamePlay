using EGamePlay.Combat;
using EGamePlay;
using ET;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MiniExampleInit : MonoBehaviour
{
    public bool EntityLog;
    public AbilityConfigObject SkillConfigObject;
    public ReferenceCollector ConfigsCollector;


    private async void Awake()
    {
        SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        Entity.EnableLog = EntityLog;
        ECSNode.Create();
        Entity.Create<TimerManager>();
        Entity.Create<CombatContext>();
        ECSNode.Instance.AddComponent<ConfigManageComponent>(ConfigsCollector);

        await TimerManager.Instance.WaitAsync(2000);
        //创建怪物战斗实体
        var monster = CombatContext.Instance.AddChild<CombatEntity>();
        //创建英雄战斗实体
        var hero = CombatContext.Instance.AddChild<CombatEntity>();
        //给英雄挂载技能并加载技能执行体
        var heroSkillAbility = hero.GetComponent<SkillComponent>().AttachSkill(SkillConfigObject);

        Debug.Log($"1 monster.CurrentHealth={monster.CurrentHealth.Value}");
        //使用英雄技能攻击怪物
        hero.GetComponent<SpellComponent>().SpellWithTarget(heroSkillAbility, monster);
        await TimerManager.Instance.WaitAsync(2000);
        Debug.Log($"2 monster.CurrentHealth={monster.CurrentHealth.Value}");
        //--示例结束--
    }

    private void Update()
    {
        ECSNode.Instance?.Update();
        TimerManager.Instance?.Update();
    }

    private void OnApplicationQuit()
    {
        ECSNode.Destroy();
    }
}
