using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using DG.Tweening;
using ET;

//public class Skill1004Ability : SkillAbility
//{
//    public override AbilityExecution CreateAbilityExecution()
//    {
//        var abilityExecution = Entity.CreateWithParent<Skill1004Execution>(this.GetParent<CombatEntity>(), this);
//        return abilityExecution;
//    }
//}

//public class Skill1004Execution : SkillAbilityExecution
//{
//    public override async void BeginExecute()
//    {
//        base.BeginExecute();

//        Hero.Instance.StopMove();
//        Hero.Instance.PlayThenIdleAsync(Hero.Instance.AnimationComponent.SkillAnimation).Coroutine();
//        Hero.Instance.SkillPlaying = true;
//        Hero.Instance.transform.GetChild(0).eulerAngles = new Vector3(0, InputDirection, 0);

//        //var taskData = new CreateEffectTaskData();
//        //taskData.Position = GetParent<CombatEntity>().Position;
//        //taskData.Direction = InputDirection;
//        //taskData.LifeTime = 2000;
//        //taskData.EffectPrefab = GetAbility<Skill1004Ability>().SkillConfigObject.SkillEffectObject;
//        //var task = Entity.CreateWithParent<CreateEffectTask>(this, taskData);
//        //task.ExecuteTaskAsync().Coroutine();

//        await TimerComponent.Instance.WaitAsync(1500);

//        var taskData2 = new CreateColliderTaskData();
//        taskData2.Position = GetParent<CombatEntity>().Position;
//        taskData2.Direction = InputDirection;
//        taskData2.LifeTime = 200;
//        taskData2.ColliderPrefab = GetAbility<Skill1004Ability>().SkillConfigObject.AreaCollider;
//        taskData2.OnTriggerEnterCallback = (other) => {
//            AbilityEntity.ApplyAbilityEffectsTo(other.GetComponent<Monster>().CombatEntity);
//        };
//        var task2 = Entity.CreateWithParent<CreateCollidedrTask>(this, taskData2);
//        await task2.ExecuteTaskAsync();

//        EndExecute();
//        Hero.Instance.SkillPlaying = false;
//    }
//}