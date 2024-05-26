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

            var abilityItem = Entity.Create<AbilityItem>(Entity.GetParent<SkillExecution>());
            abilityItem.AddComponent<AbilityItemCollisionExecuteComponent>(clipData);

            if (clipData.CollisionExecuteData.MoveType == CollisionMoveType.PathFly) abilityItem.PathFlyProcess();
            if (clipData.CollisionExecuteData.MoveType == CollisionMoveType.SelectedDirectionPathFly) abilityItem.DirectionPathFlyProcess();
            if (clipData.CollisionExecuteData.MoveType == CollisionMoveType.TargetFly) abilityItem.TargetFlyProcess(Entity.GetParent<SkillExecution>().InputTarget);
            if (clipData.CollisionExecuteData.MoveType == CollisionMoveType.ForwardFly) abilityItem.ForwardFlyProcess(Entity.GetParent<SkillExecution>().InputDirection);
            if (clipData.CollisionExecuteData.MoveType == CollisionMoveType.SelectedPosition) abilityItem.FixedPositionProcess(Entity.GetParent<SkillExecution>().InputPoint);
            if (clipData.CollisionExecuteData.MoveType == CollisionMoveType.SelectedDirection) abilityItem.FixedDirectionProcess();
#if EGAMEPLAY_ET
            abilityItem.AddCollisionComponent();
#endif

#if UNITY
            abilityItem.CreateAbilityItemProxyObj();
#endif
        }

        public void OnTriggerEnd(Entity entity)
        {
            //Log.Debug("ExecutionAnimationComponent OnTriggerExecutionEffect");
            //Entity.GetParent<SkillExecution>().OwnerEntity.Publish(AnimationClip);
        }
    }
}