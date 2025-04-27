//using ECS;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace EGamePlay.Combat
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class ExecuteCollisionItemComponent : EcsComponent<ExecuteClip>
//    {
//        public ItemExecute CollisionExecuteData { get; set; }


//        //public override void Awake()
//        //{
//        //    Entity.OnEvent(nameof(ExecuteClip.TriggerEffect), OnTriggerExecutionEffect);
//        //    Entity.OnEvent(nameof(ExecuteClip.EndEffect), OnTriggerEnd);
//        //}

//        //public void OnTriggerExecutionEffect(Entity entity)
//        //{
//        //    //Log.Debug("ExecutionSpawnCollisionComponent OnTriggerExecutionEffect");
//        //    SpawnCollisionItem(GetEntity<ExecuteClip>().ExecutionEffectConfig);
//        //}

//        ///// <summary>   技能碰撞体生成   </summary>
//        //public void SpawnCollisionItem(ExecuteClipData clipData)
//        //{
//        //    var skillExecution = Entity.GetParent<AbilityExecution>();
//        //    var abilityItem = Entity.EcsNode.AddChild<AbilityItem>(beforeAwake:x=>x.AbilityExecution = skillExecution);
//        //    abilityItem.AddComponent<AbilityItemCollisionExecuteComponent>(x => x.ExecuteClipData = clipData as ExecuteClipData);
//        //}

//        //public void OnTriggerEnd(Entity entity)
//        //{

//        //}
//    }
//}