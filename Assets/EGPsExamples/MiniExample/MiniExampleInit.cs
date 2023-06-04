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
    public SkillConfigObject SkillConfigObject;


    private void Awake()
    {
        Entity.EnableLog = EntityLog;
        MasterEntity.Create();
        Entity.Create<TimerManager>();
        Entity.Create<CombatContext>();

        //创建怪物战斗实体
        var monster = CombatContext.Instance.AddChild<CombatEntity>();
        //创建英雄战斗实体
        var hero = CombatContext.Instance.AddChild<CombatEntity>();
        //给英雄挂上技能
        var heroSkillAbility = hero.AttachSkill(SkillConfigObject);
        Debug.Log($"1 monster.CurrentHealth={monster.CurrentHealth.Value}");
        //使用英雄技能攻击怪物
        hero.GetComponent<SpellComponent>().SpellWithTarget(heroSkillAbility, monster);
        Debug.Log($"2 monster.CurrentHealth={monster.CurrentHealth.Value}");
        //--示例结束--
    }

    private void Update()
    {
        MasterEntity.Instance.Update();
        TimerManager.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        MasterEntity.Destroy();
    }
}
