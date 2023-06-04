using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat;
using EGamePlay;
using GameUtils;

public class Skill1006Component : EGamePlay.Component
{
    public override void Awake()
    {
        Entity.OnEvent(nameof(SkillAbility.CreateExecution), PostCreateExecution);
    }

    public void PostCreateExecution(Entity execution)
    {
        execution.AddComponent<SkillExecution1006Component>();
    }
}

public class SkillExecution1006Component : EGamePlay.Component
{
    public Dictionary<CombatEntity, LineRenderer> EntityChannels { get; set; } = new Dictionary<CombatEntity, LineRenderer>();
    public GameUtils.GameTimer LockTimer = new GameUtils.GameTimer(2);
    public override bool DefaultEnable => false;


    public override void Awake()
    {
        Entity.OnEvent(nameof(SkillExecution.BeginExecute), OnBeginExecute);
    }

    public void OnBeginExecute(Entity entity)
    {
        GetEntity<SkillExecution>().ActionOccupy = false;
        var channelPrefab = AssetUtils.Load<GameObject>("AbilityItems/Channel");
        channelPrefab.gameObject.SetActive(false);
        for (int i = GetEntity<SkillExecution>().SkillTargets.Count - 1; i >= 0; i--)
        {
            var item = GetEntity<SkillExecution>().SkillTargets[i];
            var channel = GameObject.Instantiate(channelPrefab);
            var lineRenderer = channel.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, item.Position);
            EntityChannels.Add(item, lineRenderer);
            channel.gameObject.SetActive(true);
        }
        foreach (var item in EntityChannels)
        {
#if !EGAMEPLAY_EXCEL
            var effectAssign = GetEntity<SkillExecution>().SkillAbility.GetComponent<AbilityEffectComponent>().CreateAssignActionByIndex(item.Key, 2);
            effectAssign.AssignEffect();
#endif
        }
        Enable = true;
    }

    public override void Update()
    {
        if (GetEntity<SkillExecution>().SkillTargets.Count == 0)
        {
            EndExecute();
            return;
        }
        foreach (var item in EntityChannels)
        {
            item.Value.SetPosition(0, GetEntity<SkillExecution>().OwnerEntity.Position);
            item.Value.SetPosition(1, item.Key.Position);
            //Log.Debug($"{GetEntity<SkillExecution>().OwnerEntity.Position}    {item.Key.Position}");
        }
        LockTimer.UpdateAsFinish(Time.deltaTime, OnLock);
    }

    public void OnLock()
    {
        //Log.Debug("OnLock");
#if !EGAMEPLAY_EXCEL
        foreach (var item in EntityChannels)
        {
            var effectAssign = GetEntity<SkillExecution>().SkillAbility.GetComponent<AbilityEffectComponent>().CreateAssignActionByIndex(item.Key, 0);
            effectAssign.AssignEffect();
            effectAssign = GetEntity<SkillExecution>().SkillAbility.GetComponent<AbilityEffectComponent>().CreateAssignActionByIndex(item.Key, 1);
            effectAssign.AssignEffect();
        }
#endif
        EndExecute();
    }

    public void EndExecute()
    {
        GetEntity<SkillExecution>().EndExecute();
        foreach (var item in EntityChannels)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        EntityChannels.Clear();
        //Entity.Destroy(this);
        //base.EndExecute();
    }
}
