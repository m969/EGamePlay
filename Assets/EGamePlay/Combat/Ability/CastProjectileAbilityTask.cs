using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace EGamePlay.Combat.Ability
{
    public class CastProjectileAbilityTask : AbilityTask
    {
        public override async Task ExecuteTaskAsync()
        {
            var ability = Parent as AbilityEntity;
            var owner = ability.Parent as CombatEntity;
            var projectile = GameObject.Instantiate(ability.SkillConfigObject.SkillEffectObject);
            projectile.transform.position = owner.Position  + Vector3.up;
            projectile.transform.DOMove(ability.InputPoint + Vector3.up, 0.8f).SetEase(Ease.Linear);
            await Task.Delay(800);
            GameObject.Destroy(projectile);
        }
    }
}