using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat;
using EGamePlay;


public class SkillAbility_1006 : SkillAbility
{
    public override AbilityExecution CreateExecution()
    {
        var abilityExecution = Parent.AddChild<SkillExecution_1006>(this);
        abilityExecution.AddComponent<UpdateComponent>();
        return abilityExecution;
    }
}

public class SkillExecution_1006 : SkillExecution
{
    public Dictionary<CombatEntity, LineRenderer> EntityChannels { get; set; } = new Dictionary<CombatEntity, LineRenderer>();
    public GameUtils.GameTimer LockTimer = new GameUtils.GameTimer(2);


    public override void BeginExecute()
    {
        var channelPrefab = SkillExecutionAsset.transform.Find("Channel");
        channelPrefab.gameObject.SetActive(false);
        for (int i = SkillTargets.Count - 1; i >= 0; i--)
        {
            var item = SkillTargets[i];
            var channel = GameObject.Instantiate(channelPrefab);
            var lineRenderer = channel.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, item.Position);
            EntityChannels.Add(item, lineRenderer);
            channel.gameObject.SetActive(true);
        }
        foreach (var item in EntityChannels)
        {
#if !EGAMEPLAY_EXCEL
            AbilityEffectComponent.ApplyEffectByIndex(item.Key, 2);
#endif
        }
    }

    public override void Update()
    {
        if (SkillTargets.Count == 0)
        {
            EndExecute();
            return;
        }
        foreach (var item in EntityChannels)
        {
            item.Value.SetPosition(0, OwnerEntity.Position);
            item.Value.SetPosition(1, item.Key.Position);
        }
        LockTimer.UpdateAsFinish(Time.deltaTime, OnLock);
    }

    public void OnLock()
    {
#if !EGAMEPLAY_EXCEL
        foreach (var item in EntityChannels)
        {
            AbilityEffectComponent.ApplyEffectByIndex(item.Key, 0);
            AbilityEffectComponent.ApplyEffectByIndex(item.Key, 1);
        }
#endif
        EndExecute();
    }

    public override void EndExecute()
    {
        foreach (var item in EntityChannels)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        EntityChannels.Clear();
        base.EndExecute();
    }
}
