using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat;
using EGamePlay;


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


    public void BeginExecute()
    {
        var channelPrefab = GetEntity<SkillExecution>().SkillExecutionAsset.transform.Find("Channel");
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
            GetEntity<SkillExecution>().SkillAbility.Get<AbilityEffectComponent>().TryAssignEffectByIndex(item.Key, 2);
#endif
        }
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
        }
        LockTimer.UpdateAsFinish(Time.deltaTime, OnLock);
    }

    public void OnLock()
    {
#if !EGAMEPLAY_EXCEL
        foreach (var item in EntityChannels)
        {
            GetEntity<SkillExecution>().SkillAbility.Get<AbilityEffectComponent>().TryAssignEffectByIndex(item.Key, 0);
            GetEntity<SkillExecution>().SkillAbility.Get<AbilityEffectComponent>().TryAssignEffectByIndex(item.Key, 1);
        }
#endif
        EndExecute();
    }

    public void EndExecute()
    {
        foreach (var item in EntityChannels)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        EntityChannels.Clear();
        Entity.Destroy(this);
        //base.EndExecute();
    }
}
