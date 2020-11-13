using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace EGamePlay.Combat.Ability
{
    public class CastProjectileAbilityTask : AbilityTask
    {
        public Vector3 TargetPoint { get; set; }


        public override void Awake(object paramObject)
        {
            TargetPoint = (Vector3)paramObject;
        }

        public override async Task ExecuteTaskAsync()
        {
            var abilityExecution = Parent as AbilityExecution;
            var executionOwner = abilityExecution.Parent as CombatEntity;
            var projectile = GameObject.Instantiate(abilityExecution.AbilityEntity.SkillConfigObject.SkillEffectObject);
            projectile.transform.position = executionOwner.Position  + Vector3.up;
            projectile.transform.DOMove(abilityExecution.InputPoint + Vector3.up, 0.8f).SetEase(Ease.Linear);
            await Task.Delay(800);
            GameObject.Destroy(projectile);
        }
    }
}