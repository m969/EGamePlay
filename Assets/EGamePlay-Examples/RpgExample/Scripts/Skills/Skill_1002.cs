using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;

//public class Skill1002Ability : SkillAbility
//{
//    public override AbilityExecution CreateAbilityExecution()
//    {
//        var abilityExecution = Entity.CreateWithParent<Skill1002Execution>(this.GetParent<CombatEntity>(), this);
//        return abilityExecution;
//    }
//}

//public class Skill1002Execution : SkillAbilityExecution
//{
//    public override async void BeginExecute()
//    {
//        base.BeginExecute();

//        var taskData2 = new CreateColliderTaskData();
//        taskData2.Position = InputPoint;
//        taskData2.ColliderPrefab = (AbilityEntity as Skill1002Ability).SkillConfigObject.AreaCollider;
//        taskData2.OnTriggerEnterCallback = (other) => {
//            AbilityEntity.ApplyAbilityEffectsTo(other.GetComponent<Monster>().CombatEntity);
//        };
//        var task2 = Entity.CreateWithParent<CreateCollidedrTask>(this, taskData2);

//        task2.ExecuteTaskAsync().Coroutine();

//        //var taskData = new CreateExplosionTaskData();
//        //taskData.TargetPoint = InputPoint;
//        //taskData.ExplosionPrefab = (AbilityEntity as Skill1002Ability).SkillConfigObject.SkillEffectObject;
//        //var task = Entity.CreateWithParent<CreateExplosionTask>(this, taskData);
//        //await task.ExecuteTaskAsync();

//        EndExecute();
//    }
//}