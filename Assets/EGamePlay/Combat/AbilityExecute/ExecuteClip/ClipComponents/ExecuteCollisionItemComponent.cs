using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteCollisionItemComponent : Component
    {
        public CollisionExecuteData CollisionExecuteData { get; set; }


        public override void Awake()
        {
            Entity.OnEvent(nameof(ExecuteClip.TriggerEffect), OnTriggerExecutionEffect);
            Entity.OnEvent(nameof(ExecuteClip.EndEffect), OnTriggerEnd);
        }

        public void OnTriggerExecutionEffect(Entity entity)
        {
            //Log.Debug("ExecutionSpawnCollisionComponent OnTriggerExecutionEffect");
            SpawnCollisionItem(GetEntity<ExecuteClip>().ExecutionEffectConfig);
        }

        /// <summary>   技能碰撞体生成   </summary>
        public void SpawnCollisionItem(ExecuteClipData clipData)
        {
            //Log.Console($"ExecuteCollisionItemComponent SpawnCollisionItem {clipData.CollisionExecuteData.MoveType}");

            var skillExecution = Entity.GetParent<SkillExecution>();
            var abilityItem = Entity.Create<AbilityItem>(skillExecution);
            abilityItem.AddComponent<AbilityItemCollisionExecuteComponent>(clipData);

            if (clipData.ItemData.MoveType == CollisionMoveType.PathFly) abilityItem.PathFlyProcess(skillExecution.InputPoint);
            if (clipData.ItemData.MoveType == CollisionMoveType.SelectedDirectionPathFly) abilityItem.DirectionPathFlyProcess(skillExecution.InputPoint, skillExecution.InputRadian);
            if (clipData.ItemData.MoveType == CollisionMoveType.TargetFly) abilityItem.TargetFlyProcess(skillExecution.InputTarget);
            if (clipData.ItemData.MoveType == CollisionMoveType.ForwardFly) abilityItem.ForwardFlyProcess(skillExecution.InputRadian);
            if (clipData.ItemData.MoveType == CollisionMoveType.SelectedPosition) abilityItem.SelectedPositionProcess(skillExecution.InputPoint);
            if (clipData.ItemData.MoveType == CollisionMoveType.FixedPosition) abilityItem.FixedPositionProcess();
            if (clipData.ItemData.MoveType == CollisionMoveType.SelectedDirection) abilityItem.SelectedDirectionProcess();
#if EGAMEPLAY_ET
            abilityItem.AddCollisionComponent();
#else
#if UNITY
            abilityItem.CreateAbilityItemProxyObj();
#endif
#endif
        }

        public void OnTriggerEnd(Entity entity)
        {
            //Log.Debug("ExecutionAnimationComponent OnTriggerExecutionEffect");
            //Entity.GetParent<SkillExecution>().OwnerEntity.Publish(AnimationClip);
        }
    }
}