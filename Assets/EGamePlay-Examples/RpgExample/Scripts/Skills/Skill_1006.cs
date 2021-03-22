using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using ET;
using Log = EGamePlay.Log;
using UnityEngine.Timeline;
using UnityEngine.Playables;


public class SkillAbility_1006 : SkillAbility
{
    public override AbilityExecution CreateExecution()
    {
        var abilityExecution = Parent.CreateChild<SkillExecution_1006>(this);
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
        for (int i = SkillTargets.Count - 1; i >= 0; i--)
        {
            var item = SkillTargets[i];
            var channel = GameObject.Instantiate(channelPrefab);
            var lineRenderer = channel.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, item.Position);
            EntityChannels.Add(item, lineRenderer);
        }
        foreach (var item in EntityChannels)
        {
#if !EGAMEPLAY_EXCEL
            SkillAbility.ApplyEffectTo(item.Key, SkillAbility.SkillConfig.Effects[2]);
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
        //for (int i = SkillTargets.Count - 1; i >= 0; i--)
        //{
        //    var item = SkillTargets[i];
        //    if (Vector3.Distance(item.Position, GetParent<CombatEntity>().Position) > 5)
        //    {
        //        SkillTargets.Remove(item);
        //    }
        //}
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
            SkillAbility.ApplyEffectTo(item.Key, SkillAbility.SkillConfig.Effects[0]);
            SkillAbility.ApplyEffectTo(item.Key, SkillAbility.SkillConfig.Effects[1]);
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
