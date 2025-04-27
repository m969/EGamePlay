using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class AbilityExecutionSystem : AEntitySystem<AbilityExecution>,
        IAwake<AbilityExecution>,
        IUpdate<AbilityExecution>
    {
        public void Awake(AbilityExecution entity)
        {
            entity.OwnerEntity = entity.GetParent<CombatEntity>();
        }

        public void Update(AbilityExecution entity)
        {
            if (entity.ExecutionObject.TotalTime > 0)
            {
                var nowTicks = TimeHelper.ClientNow() - entity.OriginTime;
                var nowSeconds = nowTicks / 1000f;
                if (nowSeconds >= entity.ExecutionObject.TotalTime)
                {
                    EndExecute(entity);
                }
            }
        }

        public static void LoadExecutionEffects(AbilityExecution entity)
        {
            if (entity.ExecutionObject == null)
            {
                return;
            }
            foreach (var clip in entity.ExecutionObject.ExecuteClips)
            {
                var executeClip = entity.AddChild<ExecuteClip>(x => x.ClipData = clip);
                AddClip(entity, executeClip);
            }
        }

        public static void AddClip(AbilityExecution entity, ExecuteClip executeClip)
        {
            entity.ExecuteClips.Add(executeClip);
        }

#if EGAMEPLAY_ET
        public ItemUnit CreateItemUnit()
        {
            var scene = OwnerEntity.GetComponent<CombatUnitComponent>().Unit.GetParent<Scene>();
            var itemUnit = scene.AddChild<ItemUnit, Action<ItemUnit>>((x) => { x.ItemEntity = this; });
            return itemUnit;
        }
#endif

        public static void BeginExecute(AbilityExecution entity)
        {
            entity.OriginTime = TimeHelper.ClientNow();
            entity.GetParent<CombatEntity>().SpellingExecution = entity;
            if (entity.SkillAbility != null)
            {
                entity.SkillAbility.Spelling = true;
            }

            foreach (var item in entity.ExecuteClips)
            {
                //ExecuteClipSystem.BeginExecute(item);
                item.Enable = true;
            }

            if (entity.ExecutionObject != null)
            {
                entity.Enable = true;
            }

            //entity.FireEvent(nameof(BeginExecute));
        }

        public static void EndExecute(AbilityExecution entity)
        {
            //Log.Debug("SkillExecution EndExecute");
            entity.GetParent<CombatEntity>().SpellingExecution = null;
            foreach (var item in entity.ExecuteClips)
            {
                item.Enable = false;
            }
            if (entity.SkillAbility != null)
            {
                entity.SkillAbility.Spelling = false;
            }
            entity.SkillTargets.Clear();
            EcsObject.Destroy(entity);
        }
    }
}