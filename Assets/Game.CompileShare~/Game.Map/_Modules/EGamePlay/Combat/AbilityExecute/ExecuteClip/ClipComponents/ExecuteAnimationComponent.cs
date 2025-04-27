//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//#if UNITY
//namespace EGamePlay.Combat
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class ExecuteAnimationComponent : ECS.EcsComponent
//    {
//        public AnimationClip AnimationClip { get; set; }


//        public override void Awake()
//        {
//            Entity.OnEvent(nameof(ExecuteClip.TriggerEffect), OnTriggerExecutionEffect);
//        }

//        public void OnTriggerExecutionEffect(Entity entity)
//        {
//            Entity.GetParent<AbilityExecution>().OwnerEntity.Publish(AnimationClip);
//        }
//    }
//}
//#endif