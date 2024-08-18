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
        Entity.OnEvent("CreateExecution", PostCreateExecution);
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
        Entity.OnEvent(nameof(AbilityExecution.BeginExecute), OnBeginExecute);
    }

    public void OnBeginExecute(Entity entity)
    {
        GetEntity<AbilityExecution>().ActionOccupy = false;
        var channelPrefab = AssetUtils.LoadObject<GameObject>("AbilityItems/Channel");
        channelPrefab.gameObject.SetActive(false);
        for (int i = GetEntity<AbilityExecution>().SkillTargets.Count - 1; i >= 0; i--)
        {
            var item = GetEntity<AbilityExecution>().SkillTargets[i];
            var channel = GameObject.Instantiate(channelPrefab);
            var lineRenderer = channel.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, item.Position);
            EntityChannels.Add(item, lineRenderer);
            channel.gameObject.SetActive(true);
        }
        foreach (var item in EntityChannels)
        {
//#if !EGAMEPLAY_EXCEL
            var abilityTriggerComp = GetEntity<AbilityExecution>().SkillAbility.GetComponent<AbilityTriggerComponent>();
            var effects = abilityTriggerComp.AbilityTriggers;
            var effect = effects[2];
            effect.OnTrigger(new TriggerContext() { Target = item.Key });
//#endif
        }
        Enable = true;
    }

    public override void Update()
    {
        if (GetEntity<AbilityExecution>().SkillTargets.Count == 0)
        {
            EndExecute();
            return;
        }
        foreach (var item in EntityChannels)
        {
            item.Value.SetPosition(0, GetEntity<AbilityExecution>().OwnerEntity.Position);
            item.Value.SetPosition(1, item.Key.Position);
            //Log.Debug($"{GetEntity<SkillExecution>().OwnerEntity.Position}    {item.Key.Position}");
        }
        LockTimer.UpdateAsFinish(Time.deltaTime, OnLock);
    }

    public void OnLock()
    {
        //Log.Debug("OnLock");
//#if !EGAMEPLAY_EXCEL
        foreach (var item in EntityChannels)
        {
            var abilityTriggerComp = GetEntity<AbilityExecution>().SkillAbility.GetComponent<AbilityTriggerComponent>();
            var effects = abilityTriggerComp.AbilityTriggers;
            for (int i = 0; i < effects.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    var effect = effects[i];
                    effect.OnTrigger(new TriggerContext() { Target = item.Key });
                }
            }
        }
//#endif
        EndExecute();
    }

    public void EndExecute()
    {
        GetEntity<AbilityExecution>().EndExecute();
        foreach (var item in EntityChannels)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        EntityChannels.Clear();
        //Entity.Destroy(this);
        //base.EndExecute();
    }
}
