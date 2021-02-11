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


//public class Skill1001Ability : SkillAbility
//{
//    public override AbilityExecution CreateAbilityExecution()
//    {
//        var abilityExecution = Entity.CreateWithParent<Skill1001Execution>(this.GetParent<CombatEntity>(), this);
//        abilityExecution.AddComponent<UpdateComponent>();
//        return abilityExecution;
//    }
//}

//public class Skill1001Execution : SkillAbilityExecution
//{
//    public override async void BeginExecute()
//    {
//        base.BeginExecute();
//    }

//    public override void EndExecute()
//    {
//        base.EndExecute();
//    }
//}